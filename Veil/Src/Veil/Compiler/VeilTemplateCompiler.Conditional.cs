using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression HandleConditional(ConditionalNode node)
        {
            var hasTrueBlock = node.TrueBlock != null && node.TrueBlock.Nodes.Any();
            var hasFalseBlock = node.FalseBlock != null && node.FalseBlock.Nodes.Any();

            if (!hasTrueBlock && !hasFalseBlock)
            {
                throw new VeilCompilerException("Conditionals must have a True or False block");
            }

            var valueToCheck = ParseExpression(node.Expression);
            var booleanCheck = BoolifyExpression(valueToCheck);

            if (!hasFalseBlock)
            {
                return Expression.IfThen(booleanCheck, HandleNode(node.TrueBlock));
            }
            else if (!hasTrueBlock)
            {
                return Expression.IfThen(Expression.IsFalse(booleanCheck), HandleNode(node.FalseBlock));
            }

            var returnTarget = Expression.Label(typeof(Task));

            return Expression.Block(
                Expression.IfThenElse(booleanCheck, 
                    Expression.Return(returnTarget, HandleNode(node.TrueBlock)), 
                    Expression.Return(returnTarget, HandleNode(node.FalseBlock))),
                Expression.Label(returnTarget, Expression.Constant(null, typeof(Task))));
        }

        private static readonly MethodInfo boolify = typeof(Helpers).GetMethod("Boolify");

        private static Expression BoolifyExpression(Expression expression)
        {
            if (expression.Type == typeof(bool))
            {
                return expression;
            }
            return Expression.Call(null, boolify, expression);
        }
    }
}