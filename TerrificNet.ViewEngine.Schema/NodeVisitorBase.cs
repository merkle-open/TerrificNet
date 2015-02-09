using System;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace TerrificNet.ViewEngine.Schema
{
    public abstract class NodeVisitorBase<TResult>
        where TResult : class
    {
        public TResult Visit(SyntaxTreeNode node)
        {
            if (node == null)
                return null;

            var blockNode = node as BlockNode;
            if (blockNode != null)
            {
                return VisitBlockNode(blockNode);
            }

            var conditionalNode = node as ConditionalNode;
            if (conditionalNode != null)
            {
                return VisitConditionalNode(conditionalNode);
            }

            var iterateNode = node as IterateNode;
            if (iterateNode != null)
            {
                return VisitIterateNode(iterateNode);
            }

            var writeExpressionNode = node as WriteExpressionNode;
            if (writeExpressionNode != null)
            {
                return VisitWriteExpressionNode(writeExpressionNode);
            }

            var writeLiteralNode = node as WriteLiteralNode;
            if (writeLiteralNode != null)
            {
                return VisitWriteLiteralNode(writeLiteralNode);
            }

	        var helperNode = node as HelperExpressionNode;
			if (helperNode != null)
			{
				return VisitHelperNode(helperNode);
			}

            throw new NotSupportedException(string.Format("The given node type '{0}' isn't supported.", node.GetType()));
        }

	    protected virtual TResult VisitHelperNode(HelperExpressionNode helperNode)
	    {
			return null;
	    }

	    protected virtual TResult VisitWriteLiteralNode(WriteLiteralNode writeLiteralNode)
        {
            return null;
        }

        protected virtual TResult VisitWriteExpressionNode(WriteExpressionNode writeExpressionNode)
        {
            return VisitExpressionNode(writeExpressionNode.Expression);
        }

        protected virtual TResult VisitIterateNode(IterateNode iterateNode)
        {
            var schema = this.VisitExpressionNode(iterateNode.Collection);
            this.VisitBlockNode(iterateNode.Body);
            this.VisitBlockNode(iterateNode.EmptyBody);

            return schema;
        }

        protected virtual TResult VisitConditionalNode(ConditionalNode node)
        {
            var expressionResult = this.VisitExpressionNode(node.Expression);
            if (node.TrueBlock != null)
                this.VisitBlockNode(node.TrueBlock);

            if (node.FalseBlock != null)
                this.VisitBlockNode(node.FalseBlock);

            return expressionResult;
        }

        protected virtual TResult VisitExpressionNode(ExpressionNode expression)
        {
            if (expression == null)
                return null;

            var lateboundExpression = expression as LateBoundExpressionNode;
            if (lateboundExpression != null)
                return VisitLateBoundExpression(lateboundExpression);

            var modelExpressionNode = expression as SubModelExpressionNode;
            if (modelExpressionNode != null)
                return VisitSubModelExpressionNode(modelExpressionNode);

            var helperExpressionNode = expression as HelperExpressionNode;
            if (helperExpressionNode != null)
                return VisitHelperExpressionNode(helperExpressionNode);

            return null;
        }

        private TResult VisitHelperExpressionNode(HelperExpressionNode helperExpressionNode)
        {
            return null;
        }

        protected virtual TResult VisitSubModelExpressionNode(SubModelExpressionNode subModuleExpression)
        {
            this.VisitExpressionNode(subModuleExpression.ModelExpression);
            this.VisitExpressionNode(subModuleExpression.SubModelExpression);

            return null;
        }

        protected virtual TResult VisitLateBoundExpression(LateBoundExpressionNode lateboundExpression)
        {
            return null;
        }

        protected virtual TResult VisitBlockNode(BlockNode blockNode)
        {
            foreach (var node in blockNode.Nodes)
            {
                Visit(node);
            }
            return null;
        }
    }
}