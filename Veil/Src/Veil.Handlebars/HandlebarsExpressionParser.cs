using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Veil.Compiler;
using Veil.Parser;

namespace Veil.Handlebars
{
    internal static class HandlebarsExpressionParser
    {
        public static ExpressionNode Parse(HandlebarsBlockStack blockStack, string expression, IMemberLocator memberLocator)
        {
            expression = expression.Trim();

            if (expression == "this")
            {
                return SyntaxTreeExpression.Self(blockStack.GetCurrentModelType(), ExpressionScope.CurrentModelOnStack);
            }
            if (expression.StartsWith("../"))
            {
                return ParseAgainstModel(blockStack.GetParentModelType(), expression.Substring(3), ExpressionScope.ModelOfParentScope, memberLocator);
            }

            return ParseAgainstModel(blockStack.GetCurrentModelType(), expression, ExpressionScope.CurrentModelOnStack, memberLocator);
        }

        private static ExpressionNode ParseAgainstModel(Type modelType, string expression, ExpressionScope expressionScope, IMemberLocator memberLocator)
        {
            var dotIndex = expression.IndexOf('.');
            if (dotIndex >= 0)
            {
                var subModel = HandlebarsExpressionParser.ParseAgainstModel(modelType, expression.Substring(0, dotIndex), expressionScope, memberLocator);
                return SyntaxTreeExpression.SubModel(
                    subModel,
                    HandlebarsExpressionParser.ParseAgainstModel(subModel.ResultType, expression.Substring(dotIndex + 1), ExpressionScope.CurrentModelOnStack, memberLocator)
                );
            }

            if (expression.EndsWith("()"))
            {
                var func = memberLocator.FindMember(modelType, expression.Substring(0, expression.Length - 2), MemberTypes.Method);
                if (func != null) return SyntaxTreeExpression.Function(modelType, func.Name, expressionScope);
            }

            var prop = memberLocator.FindMember(modelType, expression, MemberTypes.Property | MemberTypes.Field);
            if (prop != null)
            {
                switch (prop.MemberType)
                {
                    case MemberTypes.Property: return SyntaxTreeExpression.Property(modelType, prop.Name, expressionScope);
                    case MemberTypes.Field: return SyntaxTreeExpression.Field(modelType, prop.Name, expressionScope);
                }
            }

            var parts = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                return SyntaxTreeExpression.Helper(parts);
            }

            if (IsLateBoundAcceptingType(modelType)) return SyntaxTreeExpression.LateBound(expression, false, expressionScope);

            throw new VeilParserException(String.Format("Unable to parse model expression '{0}' againt model '{1}'", expression, modelType.Name));
        }

        private static bool IsLateBoundAcceptingType(Type type)
        {
            return type == typeof(object) 
                || type.IsDictionary()
                || type.GetInterfaces().Any(IsDictionary)
                || type.GetProperties().Any(p => p.GetIndexParameters().Any());
        }

        private static bool IsDictionary(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDictionary<,>);
        }
    }
}