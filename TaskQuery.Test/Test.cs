using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TaskQuery.Models;
using TaskQuery.Services;
using Xunit;

namespace TaskQuery.Test
{
	public class Test
	{
		public Test()
		{
			IDeferredTaskService.DeferredTasks.AddRange(new List<AbstractDeferredTask>()
			{
				new ConcreteDeferredTaskWithDate()
				{
					Date = new DateTime(2010, 10, 10)
				},
				new ConcreteDeferredTaskWithLink()
				{
					Link = "Link - 123 321 123 321"
				}
			});
		}

		
		
		public class DeferredTaskBuilder
		{
			private List<(Type type, Action<AbstractDeferredTask> action)> Config { get; } = new();
			
			public DeferredTaskBuilder Add<T>(Action<T> action) where T : AbstractDeferredTask
			{
				Config.Add((typeof(T), i => action((T)i)));
				return this;
			}

			public void Build()
			{
				IDeferredTaskService deferredTaskService = new DeferredTaskService(Config);
				deferredTaskService.Process();
			}

		}
		
		[Fact]
		public void Test1()
		{
			var cfg = new DeferredTaskBuilder();
			
			cfg.Add<ConcreteDeferredTaskWithDate>(i => Console.WriteLine(i.Date))
				.Add<ConcreteDeferredTaskWithLink>(i => Console.WriteLine(i.Link));
			
			cfg.Build();
		}
	}
}