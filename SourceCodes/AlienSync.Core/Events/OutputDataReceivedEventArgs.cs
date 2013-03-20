using System;
using System.IO;

namespace AlienSync.Core.Events
{
	/// <summary>
	/// This provides data for output data received event.
	/// </summary>
	public class OutputDataReceivedEventArgs : EventArgs
	{
		/// <summary>
		/// Initialises a new instance of the OutputDataReceivedEventArgs object.
		/// </summary>
		/// <param name="output">The stream used to read the output from the wrapper.</param>
		public OutputDataReceivedEventArgs(StreamReader output)
		{
			this.Output = output;
		}

		/// <summary>
		/// Gets the stream used to read the output from the wrapper.
		/// </summary>
		public StreamReader Output { get; private set; }
	}
}