using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Veil
{
    internal static class DelegateBuilder
    {
        private static readonly MethodInfo GetValueFromDictionaryMethod = typeof(DelegateBuilder).GetMethod("GetValueFromDictionary");

        public static Func<object, object> FunctionCall(Type modelType, MethodInfo function)
        {
            var model = Expression.Parameter(typeof(object));
            var castModel = Expression.Convert(model, modelType);
            var call = Expression.Call(castModel, function);
            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(call, typeof(object)),
                model
            ).Compile();
        }

        public static Func<object, object> Property(Type modelType, PropertyInfo property)
        {
            var model = Expression.Parameter(typeof(object));
            var castModel = Expression.Convert(model, modelType);
            var call = Expression.Property(castModel, property);
            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(call, typeof(object)),
                model
            ).Compile();
        }

        public static Func<object, object> Field(Type modelType, FieldInfo field)
        {
            var model = Expression.Parameter(typeof(object));
            var castModel = Expression.Convert(model, modelType);
            var call = Expression.Field(castModel, field);
            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(call, typeof(object)),
                model
            ).Compile();
        }

        public static Func<object, object> Dictionary(Type modelType, string key)
        {
            var model = Expression.Parameter(typeof(object));
            var castModel = Expression.Convert(model, modelType);

            var call = Expression.Call(GetValueFromDictionaryMethod, castModel, Expression.Constant(key));

            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(call, typeof(object)),
                model
            ).Compile();
        }

        public static object GetValueFromDictionary(IDictionary<string, object> dict, string key)
        {
            object value;
            if (!dict.TryGetValue(key, out value))
                return null;

            return value;
        } 
    }
}