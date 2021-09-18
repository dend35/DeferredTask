using System;

namespace DeferredTask.Models
{
	public abstract class AbstractDeferredTask
	{
		public DateTime Created { get; set; }

		public DateTime Start { get; set; }

		public DateTime? End { get; set; }

		public bool IsCompleted { get; set; }

		public bool IsSuccess { get; set; }

		public bool InProgress { get; set; }

		public void StartTask()
		{
			Start = DateTime.UtcNow;
			InProgress = true;
		}
		
		public void EndTask()
		{
			End = DateTime.UtcNow;
			InProgress = false;
			IsCompleted = true;
		}
	}
}