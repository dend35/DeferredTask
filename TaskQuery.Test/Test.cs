using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
		
		[Fact]
		public async Task Test1()
		{
			var thread = new DeferredTaskBuilder()
				.Add<ConcreteDeferredTaskWithDate>(i => Console.WriteLine(i.Date))
				.Add<ConcreteDeferredTaskWithLink>(i => Console.WriteLine(i.Link))
				.Build();
			Assert.NotNull(thread);
			
			await Task.Delay(TimeSpan.FromSeconds(1));
			IDeferredTaskService.DeferredTasks.Add(new ConcreteDeferredTaskWithLink
			{
				Link = "New Link"
			});

			await Task.Delay(TimeSpan.FromSeconds(10));
			Assert.True(IDeferredTaskService.DeferredTasks.First(i => i is ConcreteDeferredTaskWithLink { Link: "New Link" }).IsCompleted);
		}
	}
}