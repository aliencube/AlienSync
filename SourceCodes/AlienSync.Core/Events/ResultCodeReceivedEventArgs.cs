using System;

namespace AlienSync.Core.Events
{
	/// <summary>
	/// This provides data for resultcode received event.
	/// </summary>
	public class ResultCodeReceivedEventArgs : EventArgs
	{
		/// <summary>
		/// Initialises a new instance of the ResultCodeReceivedEventArgs object.
		/// </summary>
		/// <param name="resultCode">The result code when a process exists with exit code.</param>
		public ResultCodeReceivedEventArgs(int resultCode)
		{
			this.ResultCode = resultCode;
		}

		/// <summary>
		/// Gets the result code when a process exits with exit code.
		/// </summary>
		public int ResultCode { get; private set; }
	}
}