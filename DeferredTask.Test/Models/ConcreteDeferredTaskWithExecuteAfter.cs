using System;
using DeferredTask.Models;

namespace DeferredTask.Test.Models
{
	public class ConcreteDeferredTaskWithExecuteAfter : AbstractDeferredTask
	{
		public DateTime ExecuteAfter { get; set; }
	}
}