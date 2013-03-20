using System;

namespace AlienSync.Core.Events
{
	/// <summary>
	/// This provides data for directory synchronization started event.
	/// </summary>
	public class DirectorySynchronizationStartedEventArgs : EventArgs
	{
		/// <summary>
		/// Initialises a new instance of the DirectorySynchronizationStartedEventArgs object.
		/// </summary>
		/// <param name="localDirectory">Local directory path.</param>
		/// <param name="remoteDirectory">Remote directory path.</param>
		public DirectorySynchronizationStartedEventArgs(string localDirectory, string remoteDirectory)
		{
			this.LocalDirectory = localDirectory;
			this.RemoteDirectory = remoteDirectory;
		}

		/// <summary>
		/// Gets the local directory path.
		/// </summary>
		public String LocalDirectory { get; private set; }

		/// <summary>
		/// Gets the remote directory path.
		/// </summary>
		public String RemoteDirectory { get; private set; }
	}
}
