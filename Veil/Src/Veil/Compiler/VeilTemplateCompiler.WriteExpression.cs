using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static readonly MethodInfo writeMethod = typeof(TextWriter).GetMethod("WriteAsync", new[] { typeof(string) });
        private static readonly MethodInfo writeMethodObject = typeof(TextWriter).GetMethod("WriteAsync", new[] { typeof(object) });
        private static readonly MethodInfo encodeMethod = typeof(Helpers).GetMethod("HtmlEncodeAsync", new[] { typeof(TextWriter), typeof(string) });
        private static readonly MethodInfo encodeMethodObject = typeof(Helpers).GetMethod("HtmlEncodeAsync", new[] { typeof(TextWriter), typeof(object) });
        private static readonly MethodInfo chainMethod = typeof(TaskHelper).GetMethod("Chain", new[] { typeof(Task), typeof(Action) });
        private static readonly MethodInfo chainTaskMethod = typeof(TaskHelper).GetMethod("ChainTask", new[] { typeof(Task), typeof(Func<Task>) });

        private Expression HandleWriteExpression(WriteExpressionNode node)
        {
            bool escapeHtml;
            var expression = ParseExpression(node.Expression, out escapeHtml);

            if (node.HtmlEncode && escapeHtml)
            {
                if (expression.Type == typeof(string))
                    return Expression.Call(encodeMethod, this._writer, expression);
                
                return Expression.Call(encodeMethodObject, this._writer, expression);
            }

            if (expression.Type == typeof(string))
                return Expression.Call(this._writer, writeMethod, expression);

            if (expression.Type == typeof(void))
               return expression;

            // TODO: find better solution
            return Expression.Call(this._writer, writeMethod, Expression.Call(expression, typeof(object).GetMethod("ToString")));
        }
    }
}