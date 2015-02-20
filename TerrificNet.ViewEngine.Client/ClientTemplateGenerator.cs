using System.Collections.Generic;
using System.IO;
using TerrificNet.ViewEngine.Schema;
using TerrificNet.ViewEngine.ViewEngines;
using Veil;
using Veil.Compiler;
using Veil.Helper;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace TerrificNet.ViewEngine.Client
{
    public class ClientTemplateGenerator
    {
	    public void GenerateForTemplate(TemplateInfo templateInfo, IClientContext clientContext, IClientModel clientModel)
	    {
		    using (var stream = new StreamReader(templateInfo.Open()))
		    {
			    GenerateForTemplate(stream.ReadToEnd(), clientContext, clientModel);
		    }
	    }

		internal void GenerateForTemplate(string template, IClientContext clientContext, IClientModel model)
	    {
			ITemplateParser parser = VeilStaticConfiguration.GetParserInstance("handlebars");
			IMemberLocator memberLocator = new MemberLocatorFromNamingRule(new NamingRule());
		    IHelperHandler[] helperHandlers = null;

		    var tree = parser.Parse(new StringReader(template), typeof (object), memberLocator, helperHandlers);
		    new ClientNodeVisitor(clientContext, model).Visit(tree);
	    }

		private class ClientNodeVisitor : NodeVisitorBase<IClientModel>
	    {
		    private readonly IClientContext _clientContext;
		    private readonly Stack<IClientModel> _modelStack = new Stack<IClientModel>(); 

		    public ClientNodeVisitor(IClientContext clientContext, IClientModel model)
		    {
			    _clientContext = clientContext;
				_modelStack.Push(model);
		    }

			protected override IClientModel VisitWriteLiteralNode(WriteLiteralNode writeLiteralNode)
		    {
			    _clientContext.WriteLiteral(writeLiteralNode.LiteralContent);
				return base.VisitWriteLiteralNode(writeLiteralNode);
			}

			protected override IClientModel VisitIterateNode(IterateNode iterateNode)
			{
				var result = this.VisitExpressionNode(iterateNode.Collection);
				
				_modelStack.Push(_clientContext.BeginIterate(result));
				this.Visit(iterateNode.Body);

				_modelStack.Pop();
				_clientContext.EndIterate();

				return result;
			}

			protected override IClientModel VisitWriteExpressionNode(WriteExpressionNode writeExpressionNode)
			{
				var result = this.VisitExpressionNode(writeExpressionNode.Expression);
				_clientContext.WriteExpression(result);

				return result;
			}

			protected override IClientModel VisitLateBoundExpression(LateBoundExpressionNode lateboundExpression)
			{
				return _modelStack.Peek().Get(lateboundExpression.ItemName);
			}

			protected override IClientModel VisitSubModelExpressionNode(SubModelExpressionNode subModuleExpression)
			{
				_modelStack.Push(this.VisitExpressionNode(subModuleExpression.ModelExpression));
				var result = this.VisitExpressionNode(subModuleExpression.SubModelExpression);

				_modelStack.Pop();
				return result;
			}

			protected override IClientModel VisitConditionalNode(ConditionalNode node)
			{
				var result = this.VisitExpressionNode(node.Expression);

				_clientContext.BeginIf(result);
				this.VisitBlockNode(node.TrueBlock);
				if (node.FalseBlock != null)
				{
					_clientContext.ElseIf();
					this.VisitBlockNode(node.FalseBlock);
				}

				_clientContext.EndIf();
				return null;
			}
	    }
    }
}
