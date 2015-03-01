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
		private readonly IDictionary<string, SyntaxTreeNode> overrideSections = new Dictionary<string, Veil.Parser.SyntaxTreeNode>();
		private readonly IHelperHandler[] _helperHandlers;

		public VeilTemplateCompiler(params IHelperHandler[] helperHandlers)
		{
			_helperHandlers = helperHandlers;
		}

		public Action<TextWriter, T> Compile(SyntaxTreeNode templateSyntaxTree)
		{
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
	}
}