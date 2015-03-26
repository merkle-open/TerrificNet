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
        private static readonly MethodInfo writeMethod = typeof(Helpers).GetMethod("WriteAsync", new[] { typeof(Task), typeof(TextWriter), typeof(string) });
        //private static readonly MethodInfo writeMethodObject = typeof(Helpers).GetMethod("WriteAsync", new[] { typeof(object) });
        private static readonly MethodInfo encodeMethod = typeof(Helpers).GetMethod("HtmlEncodeAsync", new[] { typeof(Task), typeof(TextWriter), typeof(string) });
        private static readonly MethodInfo encodeMethodObject = typeof(Helpers).GetMethod("HtmlEncodeAsync", new[] { typeof(Task), typeof(TextWriter), typeof(object) });
        //private static readonly MethodInfo chainMethod = typeof(TaskHelper).GetMethod("ChainTask", new[] { typeof(Task), typeof(Func<Task>) });

        private Expression HandleWriteExpression(WriteExpressionNode node)
        {
            bool escapeHtml;
            var expression = ParseExpression(node.Expression, out escapeHtml);

            if (node.HtmlEncode && escapeHtml)
            {
                if (expression.Type == typeof(string))
                    return Expression.Call(encodeMethod, _task, this._writer, expression);
                
                return Expression.Call(encodeMethodObject, _task, this._writer, Expression.Convert(expression, typeof(object)));
            }

            if (expression.Type == typeof(string))
                return Expression.Call(writeMethod, _task, _writer, expression);

            if (expression.Type == typeof(void))
               return expression;

            // TODO: find better solution
            return Expression.Call(writeMethod, _task, _writer, Expression.Call(expression, typeof(object).GetMethod("ToString")));
        }

        //private Expression CompileToTask(Expression expression)
        //{
        //    Expression.Lambda<Func<Task>>(
        //}
    }
}