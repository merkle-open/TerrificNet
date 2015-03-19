using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression HandleWriteLiteral(WriteLiteralNode node)
        {
            var callExpression = Expression.Call(this.writer, writeMethod, Expression.Constant(node.LiteralContent, typeof(string)));
            return callExpression;
        }

        private static Expression HandleAsync(Expression taskExpression, Expression callExpression)
        {
            if (callExpression.Type == typeof (Task))
            {
                var exprTask = Expression.Lambda<Func<Task>>(callExpression);
                return Expression.Call(chainTaskMethod, taskExpression, exprTask);                
            }

            var expr = Expression.Lambda<Action>(callExpression);
            return Expression.Call(chainMethod, taskExpression, expr);
        }
    }
}