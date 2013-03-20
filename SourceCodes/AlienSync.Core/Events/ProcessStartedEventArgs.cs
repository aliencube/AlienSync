using System;

namespace AlienSync.Core.Events
{
	/// <summary>
	/// This provides data for process started event.
	/// </summary>
	public class ProcessStartedEventArgs : EventArgs
	{
		/// <summary>
		/// Initialises a new instance of the ProcessStartedEventArgs object.
		/// </summary>
		/// <param name="processName">Process name.</param>
		public ProcessStartedEventArgs(string processName)
		{
			this.ProcessName = processName;
		}

		/// <summary>
		/// Gets the process name.
		/// </summary>
		public string ProcessName { get; private set; }
	}
}