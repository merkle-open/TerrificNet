using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Veil.Compiler;
using Veil.Parser.Nodes;

namespace Veil
{
    internal static class Helpers
    {
        public static Task HtmlEncodeAsync(TextWriter writer, object value)
        {
            if (value != null)
                return HtmlEncodeAsync(writer, value.ToString());

            return HtmlEncodeAsync(writer, string.Empty);
        }

        public static Task HtmlEncodeAsync(TextWriter writer, string value)
        {
            if (string.IsNullOrEmpty(value)) 
                return Task.FromResult(false);

            var startIndex = 0;
            var currentIndex = 0;
            var valueLength = value.Length;
            char currentChar;

            if (valueLength < 50)
            {
                // For short string, we can just pump each char directly to the writer
                for (; currentIndex < valueLength; ++currentIndex)
                {
                    currentChar = value[currentIndex];
                    switch (currentChar)
                    {
                        case '&': writer.Write("&amp;"); break;
                        case '<': writer.Write("&lt;"); break;
                        case '>': writer.Write("&gt;"); break;
                        case '"': writer.Write("&quot;"); break;
                        case '\'': writer.Write("&#39;"); break;
                        default: writer.Write(currentChar); break;
                    }
                }
            }
            else
            {
                // For longer strings, the number of Write calls becomes prohibitive, so sacrifice a call to ToCharArray to allos us to buffer the Write calls
                char[] chars = null;
                for (; currentIndex < valueLength; ++currentIndex)
                {
                    currentChar = value[currentIndex];
                    switch (currentChar)
                    {
                        case '&':
                        case '<':
                        case '>':
                        case '"':
                        case '\'':
                            if (chars == null) chars = value.ToCharArray();
                            if (currentIndex != startIndex) writer.Write(chars, startIndex, currentIndex - startIndex);
                            startIndex = currentIndex + 1;

                            switch (currentChar)
                            {
                                case '&': writer.Write("&amp;"); break;
                                case '<': writer.Write("&lt;"); break;
                                case '>': writer.Write("&gt;"); break;
                                case '"': writer.Write("&quot;"); break;
                                case '\'': writer.Write("&#39;"); break;
                            }
                            break;
                    }
                }

                if (startIndex == 0) 
                    return writer.WriteAsync(value);

                if (currentIndex != startIndex) 
                    return writer.WriteAsync(chars, startIndex, currentIndex - startIndex);
            }
            return Task.FromResult(false);
        }

        public static Task HtmlEncodeLateBoundAsync(TextWriter writer, object value)
        {
            if (value is string)
            {
                return HtmlEncodeAsync(writer, (string)value);
            }
            
            if (value != null)
                return writer.WriteAsync(value.ToString());

            return Task.FromResult(false);
        }

        public static bool Boolify(object o)
        {
            if (o is bool) return (bool)o;
            return o != null;
        }

        private static ConcurrentDictionary<Tuple<Type, string>, Func<object, object>> lateBoundCache = new ConcurrentDictionary<Tuple<Type, string>, Func<object, object>>();

        public static object RuntimeBind(object model, LateBoundExpressionNode node)
        {
	        var itemName = node.ItemName;
	        var memberLocator = node.MemberLocator;

            if (model == null)
                return null;

            var runtimeModel = model as IRuntimeModel;
            if (runtimeModel != null && runtimeModel.Data != null)
                model = runtimeModel.Data;

            var binder = lateBoundCache.GetOrAdd(Tuple.Create(model.GetType(), itemName), new Func<Tuple<Type, string>, Func<object, object>>(pair =>
            {
                var type = pair.Item1;
                var name = pair.Item2;

                if (name.EndsWith("()"))
                {
                    var function = memberLocator.FindMember(type, name.Substring(0, name.Length - 2), MemberTypes.Method) as MethodInfo;
                    if (function != null) return DelegateBuilder.FunctionCall(type, function);
                }

                var property = memberLocator.FindMember(type, name, MemberTypes.Property) as PropertyInfo;
                if (property != null) return DelegateBuilder.Property(type, property);

                var field = memberLocator.FindMember(type, name, MemberTypes.Field) as FieldInfo;
                if (field != null) return DelegateBuilder.Field(type, field);

                var dictionaryType = type.GetDictionaryTypeWithKey<string>();
                if (dictionaryType != null) return DelegateBuilder.Dictionary(dictionaryType, name);

                return null;
            }));

            if (binder == null) 
				throw new VeilCompilerException("Unable to late-bind '{0}' against model {1}".FormatInvariant(itemName, model.GetType().Name), node);

            var result = binder(model);
            return result;
        }

        private static BindingFlags GetBindingFlags(bool isCaseSensitive)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (!isCaseSensitive)
            {
                flags = flags | BindingFlags.IgnoreCase;
            }
            return flags;
        }
    }

    public interface IRuntimeModel
    {
        object Data { get; }
    }
}