using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Veil.Helper;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
	internal partial class VeilTemplateCompiler<T>
	{
		private readonly ParameterExpression writer = Expression.Parameter(typeof(TextWriter), "writer");
		private readonly ParameterExpression model = Expression.Parameter(typeof(T), "model");
		private LinkedList<Expression> modelStack = new LinkedList<Expression>();
		private readonly Func<string, Type, SyntaxTreeNode> includeParser;
		private readonly IDictionary<string, SyntaxTreeNode> overrideSections = new Dictionary<string, Veil.Parser.SyntaxTreeNode>();
		private readonly IHelperHandler _helperHandler;

		public VeilTemplateCompiler(Func<string, Type, SyntaxTreeNode> includeParser, IHelperHandler helperHandler)
		{
			this.includeParser = includeParser;
			_helperHandler = helperHandler;
		}

		public Action<TextWriter, T> Compile(SyntaxTreeNode templateSyntaxTree)
		{
			while (templateSyntaxTree is ExtendTemplateNode)
			{
				templateSyntaxTree = this.Extend((ExtendTemplateNode)templateSyntaxTree);
			}

			this.PushScope(this.model);
			return Expression.Lambda<Action<TextWriter, T>>(this.HandleNode(templateSyntaxTree), this.writer, this.model).Compile();
		}

		private void PushScope(Expression scope)
		{
			this.modelStack.AddFirst(scope);
		}

		private void PopScope()
		{
			this.modelStack.RemoveFirst();
		}

		private SyntaxTreeNode Extend(ExtendTemplateNode extendNode)
		{
			foreach (var o in extendNode.Overrides)
			{
				if (this.overrideSections.ContainsKey(o.Key)) continue;

				this.overrideSections.Add(o.Key, o.Value);
			}
			return includeParser(extendNode.TemplateName, typeof(T));
		}
	}
}