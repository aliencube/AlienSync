using System;

namespace AlienSync.Core.Events
{
	/// <summary>
	/// This provides data for process completed event.
	/// </summary>
	public class ProcessCompletedEventArgs : EventArgs
	{
		/// <summary>
		/// Initialises a new instance of the ProcessCompletedEventArgs object.
		/// </summary>
		/// <param name="processName">Process name.</param>
		/// <param name="exitCode">The exit code when a process is completed with its exit code.</param>
		public ProcessCompletedEventArgs(string processName, int exitCode)
		{
			this.ProcessName = processName;
			this.ExitCode = exitCode;
		}

		/// <summary>
		/// Gets the process name.
		/// </summary>
		public string ProcessName { get; private set; }

		/// <summary>
		/// Gets the exit code when a process is completed with its exit code.
		/// </summary>
		public int ExitCode { get; private set; }
	}
}