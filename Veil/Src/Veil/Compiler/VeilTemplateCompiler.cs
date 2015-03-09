using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Veil.Helper;
using Veil.Parser;

namespace Veil.Compiler
{
	internal partial class VeilTemplateCompiler<T>
	{
        private readonly ParameterExpression context = Expression.Parameter(typeof(RenderingContext), "context");
	    private readonly Expression writer;

		private readonly ParameterExpression model = Expression.Parameter(typeof(T), "model");
		private LinkedList<Expression> modelStack = new LinkedList<Expression>();
		private readonly IDictionary<string, SyntaxTreeNode> overrideSections = new Dictionary<string, SyntaxTreeNode>();
		private readonly IHelperHandler[] _helperHandlers;

		public VeilTemplateCompiler(params IHelperHandler[] helperHandlers)
		{
			_helperHandlers = helperHandlers;
		    this.writer = Expression.Property(context, "Writer");
		}

        public Action<RenderingContext, T> Compile(SyntaxTreeNode templateSyntaxTree)
		{
			this.PushScope(this.model);
            return Expression.Lambda<Action<RenderingContext, T>>(this.HandleNode(templateSyntaxTree), this.context, this.model).Compile();
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