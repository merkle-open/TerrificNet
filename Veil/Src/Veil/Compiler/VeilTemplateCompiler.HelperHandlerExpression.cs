using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Veil.Helper;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    public static class HelperWrapper
    {
        public async static Task EvaluateAsync(Task before, IHelperHandler handler, object model, RenderingContext renderingContext, HelperExpressionNode node)
        {
            await before.ConfigureAwait(false);

            try
            {
                await handler.EvaluateAsync(model, renderingContext, node.Parameters).ConfigureAwait(false);
            }
            catch (VeilCompilerException)
            {
                throw;
            }
            catch (VeilParserException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new VeilCompilerException(string.Format("Error on execute helper '{0}'. Message is: {1}", node.Name, ex.Message), ex, node);
            }
        }

        public async static Task Leave(Task before, IBlockHelperHandler handler, object model, RenderingContext renderingContext, HelperExpressionNode node)
        {
            await before.ConfigureAwait(false);
            handler.Leave(model, renderingContext, node.Name, node.Parameters);
        }
    }

    internal partial class VeilTemplateCompiler<T>
    {
        private static readonly MethodInfo HelperFunction = typeof(HelperWrapper).GetMethod("EvaluateAsync");
        private static readonly MethodInfo HelperFunctionLeave = typeof(HelperWrapper).GetMethod("Leave");

		private Expression HandleHelperExpression(HelperExpressionNode node)
		{
			return HandleHelperExpressionWithMethod(node, HelperFunction);
		}

	    private Expression HandleHelperExpressionWithMethod(HelperExpressionNode node, MethodInfo helperFunction)
	    {
		    var helper = EvaluateHelper(node);
		    var modelExpression = EvaluateScope(node.Scope, node);

	        var expression = Expression.Call(helperFunction,
                _task,
                Expression.Constant(helper),
	            modelExpression, _context, Expression.Constant(node));

	        return expression;
	    }

	    private IHelperHandler EvaluateHelper(HelperExpressionNode node)
		{
			var helper = _helperHandlers.FirstOrDefault(h => h.IsSupported(node.Name));
			if (helper == null)
				throw new VeilCompilerException(string.Format("Could not find a helper with the name '{0}'.", node.Name), node);

			return helper;
		}

	    private Expression HandleHelperBlockNode(HelperBlockNode node)
	    {
            return HandleBlock(
				HandleHelperExpression(node.HelperExpression), 
				HandleBlock(node.Block),
                HandleHelperExpressionWithMethod(node.HelperExpression, HelperFunctionLeave));
	    }
    }
}