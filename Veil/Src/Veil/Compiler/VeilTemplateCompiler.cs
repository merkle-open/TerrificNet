using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Veil.Helper;
using Veil.Parser;

namespace Veil.Compiler
{
    public static class TaskHelper
    {
        public static async Task Chain(Task before, Action after)
        {
            if (before == null)
            {
                after();
                return;
            }

            await before.ConfigureAwait(false);
            after();

            //return before.ContinueWith(t => after());
        }

        public static async Task ChainTask(Task before, Func<Task> after)
        {
            if (before == null)
            {
                await Handle(after);
                return;
            }
                //before = Task.FromResult(false);

            await before.ConfigureAwait(false);
            await Handle(after);

            //return before.ContinueWith(t =>
            //{
            //    Console.WriteLine("continue");
            //    after();
            //});
        }

        private static async Task Handle(Func<Task> after)
        {
            var afterTask = after();
            if (afterTask != null)
                await afterTask.ConfigureAwait(false);
        }
    }

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

        public Func<RenderingContext, T, Task> Compile(SyntaxTreeNode templateSyntaxTree)
		{
			this.PushScope(this.model);

            Expression<Func<RenderingContext, Task>> test = r => r.Writer.WriteAsync("sss");

            ParameterExpression task = Expression.Variable(typeof (Task));
            var ret = Expression.Label(typeof(Task));
            Expression bodyExpression = this.HandleNode(templateSyntaxTree);

            var expression = Expression.Lambda<Func<RenderingContext, T, Task>>(bodyExpression, this.context, this.model);
            
            return expression.Compile();
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