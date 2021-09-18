using System;
using DeferredTask.Models;

namespace DeferredTask.Test.Models
{
	public class ConcreteDeferredTaskWithDate : AbstractDeferredTask
	{
		public DateTime Date { get; set; }
	}
}