using System;
using System.Threading.Tasks;

namespace Veil.Compiler
{
	public static class TaskHelper
	{
		private static readonly Task CompletedTask = Task.FromResult(true); 

		public static async Task Chain(Task before, Action after)
		{
			if (before == null)
			{
				after();
				return;
			}

			await before.ConfigureAwait(false);
			after();
		}

		public static async Task ChainTask(Task before, Func<Task> after)
		{
			if (before == null)
			{
				await Handle(after).ConfigureAwait(false);
				return;
			}

			await before.ConfigureAwait(false);
			await Handle(after).ConfigureAwait(false);
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