using System;

namespace AlienSync.Core.Events
{
	/// <summary>
	/// This provides data for synchronization started event.
	/// </summary>
	public class SynchronizationStartedEventArgs : EventArgs
	{
		/// <summary>
		/// Initialises a new instance of the SynchronizationStartedEventArgs object.
		/// </summary>
		/// <param name="dateStarted">Date when the synchronization process is started.</param>
		public SynchronizationStartedEventArgs(DateTime dateStarted)
		{
			this.DateStarted = dateStarted;
		}

		/// <summary>
		/// Gets the date when the synchronization process is started.
		/// </summary>
		public DateTime DateStarted { get; private set; }
	}
}