using System;

namespace TaskQuery.Models
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
			Start = DateTime.Now;
			InProgress = true;
		}
		
		public void EndTask()
		{
			End = DateTime.Now;
			InProgress = false;
			IsCompleted = true;
		}
	}
}