using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeferredTask.Models;
using DeferredTask.Services;
using DeferredTask.Test.Models;
using Xunit;

namespace DeferredTask.Test
{
	public class Test
	{
		private readonly IDeferredTaskService _deferredTaskService;
		public Test()
		{
			_deferredTaskService = new DeferredTaskBuilder()
				.Add<ConcreteDeferredTaskWithDate>(i => Console.WriteLine(i.Date))
				.Add<ConcreteDeferredTaskWithLink>(i => Console.WriteLine(i.Link))
				.Add<ConcreteDeferredTaskWithExecuteAfter>(i =>
				{
					if(i.ExecuteAfter < DateTime.Now)
					{
						i.StartTask();
						Console.WriteLine(i.ExecuteAfter);
						i.EndTask();
					}
				}, false)
				.Build();
			
			_deferredTaskService.AddTask(new List<AbstractDeferredTask>()
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
		
		[Fact]
		public async Task Test1()
		{
			_deferredTaskService.AddTask(new ConcreteDeferredTaskWithLink
			{
				Link = "New Link"
			});
			_deferredTaskService.AddTask(new ConcreteDeferredTaskWithExecuteAfter()
			{
				ExecuteAfter = DateTime.Now.AddSeconds(3)
			});
			
			Assert.NotNull(_deferredTaskService);
			await Task.Delay(TimeSpan.FromSeconds(1));
			
			await Task.Delay(TimeSpan.FromSeconds(10));
			Assert.True(((DeferredTaskService)_deferredTaskService).DeferredTasks.First(i => i is ConcreteDeferredTaskWithLink { Link: "New Link" }).IsCompleted);
		}
	}
}