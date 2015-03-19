using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression HandleBlock(BlockNode block)
        {
            if (!block.Nodes.Any())
            {
                return Expression.Empty();
            }

            var task = Expression.Variable(typeof (Task));
            var blockNodes = (from node in block.Nodes
                              select this.HandleNode(node)).ToArray();
            if (blockNodes.Length == 1)
            {
                return blockNodes[0];
            }

            return HandleBlock(blockNodes);

            //var ret = Expression.Label(typeof(Task));
            //return Expression.Block(typeof(Task), new[] { task }, 
            //    blockNodes.Union(new Expression[] { Expression.Return(ret, task), Expression.Label(ret, Expression.Constant(null, typeof(Task))) }));
        }

        private static Expression HandleBlock(params Expression[] blockNodes)
        {
            Expression ex = blockNodes[0];
            ex = HandleAsync(Expression.Constant(null, typeof(Task)), ex);
            for (int i = 1; i < blockNodes.Length; i++)
            {
                ex = HandleAsync(ex, blockNodes[i]);
            }
            return ex;
        }
    }
}