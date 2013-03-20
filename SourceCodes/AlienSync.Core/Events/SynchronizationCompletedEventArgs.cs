using System;

namespace AlienSync.Core.Events
{
	/// <summary>
	/// This provides data for synchronization completed event.
	/// </summary>
	public class SynchronizationCompletedEventArgs : EventArgs
	{
		/// <summary>
		/// Initialises a new instance of the SynchronizationCompletedEventArgs object.
		/// </summary>
		/// <param name="dateCompleted">Date when the synchronization process is completed.</param>
		public SynchronizationCompletedEventArgs(DateTime dateCompleted)
		{
			this.DateCompleted = dateCompleted;
		}

		/// <summary>
		/// Gets the date when the synchronization process is completed.
		/// </summary>
		public DateTime DateCompleted { get; private set; }
	}
}