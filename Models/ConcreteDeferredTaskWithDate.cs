using System;

namespace TaskQuery.Models
{
	public class ConcreteDeferredTaskWithDate : AbstractDeferredTask
	{
		public DateTime Date { get; set; }
	}
}