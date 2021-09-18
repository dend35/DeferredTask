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

		void Stop();
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

		public void Stop()
		{
			Task.Run(() => _cancellationToken.Cancel());
		}

		private void Process()
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
	
	
	public class DeferredTaskBuilder
	{
		private List<(Type type, Action<AbstractDeferredTask> action)> Config { get; } = new();
		private IDeferredTaskService _deferredTaskService;
			
		public DeferredTaskBuilder Add<T>(Action<T> action) where T : AbstractDeferredTask
		{
			Config.Add((typeof(T), i => action((T)i)));
			return this;
		}

		public Thread Build()
		{
			_deferredTaskService = new DeferredTaskService(Config);
			var thread = new Thread(() =>
			{
				_deferredTaskService.Start();
			})
			{
				Name = "DeferredTaskService"
			};
			thread.Start();
			return thread;
		}

	}
}