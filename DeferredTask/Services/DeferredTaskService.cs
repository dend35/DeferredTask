using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeferredTask.Models;

namespace DeferredTask.Services
{
	public interface IDeferredTaskService
	{
		void AddTask(params AbstractDeferredTask[] tasks);
		void AddTask(IEnumerable<AbstractDeferredTask> tasks);

		IEnumerable<AbstractDeferredTask> GetCompletedTasks();
		
		IEnumerable<AbstractDeferredTask> GetUncompletedTasks();
		
		Task Start();

		Task Stop();
	}
	
	public class DeferredTaskService : IDeferredTaskService
	{
		private CancellationTokenSource _cancellationToken;
		private List<(Type type, Action<IAbstractDeferredTask> action, bool autoStart)> _config;
		public readonly List<AbstractDeferredTask> DeferredTasks = new();
		
		public DeferredTaskService(List<(Type type, Action<IAbstractDeferredTask> action, bool autoStart)> config)
		{
			_config = config;
		}

		public void AddTask(params AbstractDeferredTask[] tasks)
		{
			DeferredTasks.AddRange(tasks.ToList());
		}
		
		public void AddTask(IEnumerable<AbstractDeferredTask> tasks)
		{
			DeferredTasks.AddRange(tasks);
		}
		
		public IEnumerable<AbstractDeferredTask> GetCompletedTasks()
		{
			return DeferredTasks.Where(i => i.IsCompleted);
		}
		
		public IEnumerable<AbstractDeferredTask> GetUncompletedTasks()
		{
			return DeferredTasks.Where(i => !i.IsCompleted);
		}

		public async Task Start()
		{
			_cancellationToken = new CancellationTokenSource();
			while(!_cancellationToken.IsCancellationRequested)
			{
				Process();
				await Task.Delay(TimeSpan.FromSeconds(1), _cancellationToken.Token);
			}
			Console.WriteLine("DeferredTaskService Stopped");
		}

		public Task Stop()
		{
			return Task.Run(() => _cancellationToken.Cancel());
		}

		private void Process()
		{
			var tasks = DeferredTasks.Where(i => !i.InProgress && !i.IsCompleted);
			foreach (var deferredTask in tasks)
			{
				var (type, action, autoStart) = _config.FirstOrDefault(i => i.type == deferredTask.GetType());
				if(action == null || type == null)
				{
					continue;
				}
				try
				{
					if(autoStart)
					{
						deferredTask.StartTask();
					}
					action(deferredTask);
				}
				catch
				{
					deferredTask.IsSuccess = false;
				}
				finally
				{
					if(autoStart)
					{
						deferredTask.EndTask();
					}
				}
			}
		}
	}
	
	
	public class DeferredTaskBuilder
	{
		private List<(Type type, Action<IAbstractDeferredTask> action, bool autoStart)> Config { get; } = new();
		private IDeferredTaskService _deferredTaskService;
			
		public DeferredTaskBuilder Add<T>(Action<T> action, bool autoStart = true) where T : IAbstractDeferredTask
		{
			Config.Add((typeof(T), i => action((T)i), autoStart));
			return this;
		}

		public IDeferredTaskService Build()
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
			return _deferredTaskService;
		}

	}
}