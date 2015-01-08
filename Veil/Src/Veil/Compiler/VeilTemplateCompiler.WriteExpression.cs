using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static readonly MethodInfo writeMethod = typeof(TextWriter).GetMethod("Write", new[] { typeof(string) });
        private static readonly MethodInfo writeMethodObject = typeof(TextWriter).GetMethod("Write", new[] { typeof(object) });
        private static readonly MethodInfo encodeMethod = typeof(Helpers).GetMethod("HtmlEncode", new[] { typeof(TextWriter), typeof(string) });
        private static readonly MethodInfo encodeMethodObject = typeof(Helpers).GetMethod("HtmlEncode", new[] { typeof(TextWriter), typeof(object) });

        private Expression HandleWriteExpression(WriteExpressionNode node)
        {
	        bool escapeHtml;
	        var expression = ParseExpression(node.Expression, out escapeHtml);

			if (node.HtmlEncode && escapeHtml)
			{
                if (expression.Type == typeof(string))
				    return Expression.Call(encodeMethod, this.writer, expression);

                return Expression.Call(encodeMethodObject, this.writer, expression);
			}

            if (expression.Type == typeof(string))
                return Expression.Call(this.writer, writeMethod, expression);

            return Expression.Call(this.writer, writeMethodObject, expression);
        }
    }
}