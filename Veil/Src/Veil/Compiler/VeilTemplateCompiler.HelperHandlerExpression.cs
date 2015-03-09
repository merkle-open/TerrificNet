using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Veil.Helper;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
		private static readonly MethodInfo HelperFunction = typeof(IHelperHandler).GetMethod("Evaluate");
		private static readonly MethodInfo HelperFunctionLeave = typeof(IBlockHelperHandler).GetMethod("Leave");

		private Expression HandleHelperExpression(HelperExpressionNode node)
		{
			return HandleHelperExpressionWithMethod(node, HelperFunction);
		}

	    private Expression HandleHelperExpressionWithMethod(HelperExpressionNode node, MethodInfo helperFunction)
	    {
		    var helper = EvaluateHelper(node);
		    var modelExpression = EvaluateScope(node.Scope);

		    return Expression.Call(Expression.Constant(helper), helperFunction,
			    modelExpression, context, Expression.Constant(node.Name), Expression.Constant(node.Parameters));
	    }

	    private IHelperHandler EvaluateHelper(HelperExpressionNode node)
		{
			var helper = _helperHandlers.FirstOrDefault(h => h.IsSupported(node.Name));
			if (helper == null)
				throw new NotSupportedException(string.Format("Could not find a helper with the name '{0}'.", node.Name));

			return helper;
		}

	    private Expression HandleHelperBlockNode(HelperBlockNode node)
	    {
		    return Expression.Block(
				HandleHelperExpression(node.HelperExpression), 
				HandleBlock(node.Block), 
				HandleHelperExpressionWithMethod(node.HelperExpression, HelperFunctionLeave));
	    }
    }
}