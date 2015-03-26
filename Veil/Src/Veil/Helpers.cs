using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil
{
    internal static class Helpers
    {
        public static Task HtmlEncodeAsync(Task before, TextWriter writer, object value)
        {
            if (value != null)
                return HtmlEncodeAsync(before, writer, value.ToString());

            return HtmlEncodeAsync(before, writer, string.Empty);
        }

        public static async Task WriteAsync(Task before, TextWriter writer, string value)
        {
            await before.ConfigureAwait(false);
            await writer.WriteAsync(value).ConfigureAwait(false);
            //return before.ContinueWith(t => writer.WriteAsync(value), TaskContinuationOptions.AttachedToParent).Unwrap();
        }

        public static async Task HtmlEncodeAsync(Task before, TextWriter writer, string value)
        {
            await before.ConfigureAwait(false);

            if (string.IsNullOrEmpty(value)) 
                return;

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
                {
                    await writer.WriteAsync(value).ConfigureAwait(false);
                    return;
                }

                if (currentIndex != startIndex)
                    await writer.WriteAsync(chars, startIndex, currentIndex - startIndex).ConfigureAwait(false);
            }
        }

        public static Task HtmlEncodeLateBoundAsync(Task before, TextWriter writer, object value)
        {
            if (value is string)
            {
                return HtmlEncodeAsync(before, writer, (string)value);
            }
            
            if (value != null)
                return Unwrap(before, writer, value);

            return before;
        }

        private static async Task Unwrap(Task before, TextWriter writer, object value)
        {
            await before.ConfigureAwait(false);
            await writer.WriteAsync(value.ToString());
        }

        public static bool Boolify(object o)
        {
            if (o is bool) return (bool)o;
            return o != null;
        }

        public static void CheckNotNull(string message, object obj, SyntaxTreeNode node)
        {
            if (obj == null)
                throw new VeilCompilerException(message, node);
        }

        private static ConcurrentDictionary<Tuple<Type, string>, Func<object, object>> lateBoundCache = new ConcurrentDictionary<Tuple<Type, string>, Func<object, object>>();

        public static object RuntimeBind(object model, LateBoundExpressionNode node)
        {
	        var itemName = node.ItemName;
	        var memberLocator = node.MemberLocator;

            CheckNotNull(string.Format("Could not bind expression with name '{0}'. The value is null.", node.ItemName), 
                model, node);

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