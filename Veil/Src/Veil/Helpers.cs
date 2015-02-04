using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using Veil.Compiler;

namespace Veil
{
    internal static class Helpers
    {
        public static void HtmlEncode(TextWriter writer, object value)
        {
            if (value != null)
                HtmlEncode(writer, value.ToString());

            HtmlEncode(writer, string.Empty);
        }

        public static void HtmlEncode(TextWriter writer, string value)
        {
            if (value == null || value.Length == 0) return;
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

                if (startIndex == 0) writer.Write(value);
                else if (currentIndex != startIndex) writer.Write(chars, startIndex, currentIndex - startIndex);
            }
        }

        public static void HtmlEncodeLateBound(TextWriter writer, object value)
        {
            if (value is string)
            {
                HtmlEncode(writer, (string)value);
            }
            else
            {
                writer.Write(value);
            }
        }

        public static bool Boolify(object o)
        {
            if (o is bool) return (bool)o;
            return o != null;
        }

        private static ConcurrentDictionary<Tuple<Type, string>, Func<object, object>> lateBoundCache = new ConcurrentDictionary<Tuple<Type, string>, Func<object, object>>();

        public static object RuntimeBind(object model, string itemName, bool isCaseSensitive, IMemberLocator memberLocator)
        {
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

            if (binder == null) throw new VeilCompilerException("Unable to late-bind '{0}' against model {1}".FormatInvariant(itemName, model.GetType().Name));
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