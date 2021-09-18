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

		void Process();
	}
	
	public class DeferredTaskService : IDeferredTaskService
	{
		private CancellationTokenSource _cancellationToken;
		private List<(Type type, Action<AbstractDeferredTask> action)> _config;

		public DeferredTaskService(List<(Type type, Action<AbstractDeferredTask> action)> config)
		{
			_config = config;
		}

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

		public void Process()
		{
			foreach (var deferredTask in IDeferredTaskService.DeferredTasks.Where(i=> !i.InProgress && !i.IsCompleted))
			{
				try
				{
					deferredTask.StartTask();
					_config.FirstOrDefault(i => i.type == deferredTask.GetType()).action(deferredTask);
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