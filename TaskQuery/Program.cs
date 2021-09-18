using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskQuery.Models;
using TaskQuery.Services;

namespace TaskQuery
{
	internal static class Program
	{
		private static async Task Main(string[] args)
		{
			
			
			
			Console.WriteLine("Before");
			var myThread = new Thread(() =>
			{
				IDeferredTaskService deferredTaskService = new DeferredTaskService(null);
				deferredTaskService.Start();
			});
			myThread.Start();
			Console.WriteLine("After");
			await Task.Delay(TimeSpan.FromSeconds(3));
			Console.WriteLine("End");
		}
	}
}