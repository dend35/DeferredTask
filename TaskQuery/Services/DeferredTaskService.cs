using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskQuery.Models;

namespace TaskQuery.Services
{
	public interface IDeferredTaskService
	{
		public static readonly List<AbstractDeferredTask> DeferredTasks = new();

		Task Start();

		Task Stop();
	}
	
	public class DeferredTaskService : IDeferredTaskService
	{
		private CancellationTokenSource _cancellationToken;

		public async Task Start()
		{
			_cancellationToken = new CancellationTokenSource();
			while(!_cancellationToken.IsCancellationRequested)
			{
				Process();
				await Task.Delay(TimeSpan.FromSeconds(5), _cancellationToken.Token);
			}
			Console.WriteLine("DeferredTaskService Stopped");
		}

		public Task Stop()
		{
			return Task.Run(() => _cancellationToken.Cancel());
		}

		private static void Process()
		{
			foreach (var deferredTask in IDeferredTaskService.DeferredTasks.Where(i=> !i.InProgress && !i.IsCompleted))
			{
				try
				{
					deferredTask.StartTask();
					switch (deferredTask)
					{
						case ConcreteDeferredTaskWithLink task:
						{
							Console.WriteLine(task.Link);
							deferredTask.IsSuccess = true;
							break;
						}
						case ConcreteDeferredTaskWithDate task:
						{
							Console.WriteLine(task.Date);
							deferredTask.IsSuccess = true;
							break;
						}
					}
				}
				catch
				{
					deferredTask.IsSuccess = false;
				}
				finally
				{
					deferredTask.EndTask();
				}
			}
		}
	}
}