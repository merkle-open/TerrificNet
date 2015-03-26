using System;
using System.Threading.Tasks;

namespace Veil.Compiler
{
	public static class TaskHelper
	{
		private static readonly Task CompletedTask = Task.FromResult(true); 

		public static Task Chain(Task before, Action after)
		{
			if (before == null)
				return new Task(after);

			return before.ContinueWith(t => after());
		}

		public static Task ChainTask(Task before, Func<Task> after)
		{
			if (before == null)
				return CompletedTask.ContinueWith(t => after()).Unwrap();

			return before.ContinueWith(t => after()).Unwrap();
		}

		private static Task Handle(Func<Task> after)
		{
			var afterTask = after();
			if (afterTask != null)
				return afterTask;

			return CompletedTask;
		}
	}
}