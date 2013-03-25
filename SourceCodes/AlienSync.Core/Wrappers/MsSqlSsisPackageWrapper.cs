using System;
using Microsoft.SqlServer.Dts.Runtime;
using AlienSync.Core.Enums;
using AlienSync.Core.Events;

namespace AlienSync.Core.Wrappers
{
	/// <summary>
	/// This represents the MS-SQL SSIS package wrapper entity.
	/// </summary>
	public class MsSqlSsisPackageWrapper
	{
		#region Constructors
		/// <summary>
		/// Initialises a new instance of the MsSqlSsisPackageWrapper object.
		/// </summary>
		/// <param name="settings">Configuration settings.</param>
		public MsSqlSsisPackageWrapper(Settings settings)
		{
			this._settings = settings;
		}
		#endregion

		#region Properties
		private readonly Settings _settings;
		#endregion

		#region Methods
		/// <summary>
		/// Executes the SSIS package for MS-SQL database synchronization.
		/// </summary>
		/// <returns>Returns exit code. For successful execution will return 0.</returns>
		public int Execute()
		{
			var processName = Convert.ToString(Database.MsSql);
			this.OnProcessStarted(new ProcessStartedEventArgs(processName));

			int exitCode;
			var el = new SsisPackageEventListener();
			var app = new Application();
			using (var package = app.LoadPackage(this._settings.SsisPackagePath, el))
			{
				var result = package.Execute(null, null, el, null, null);
				exitCode = Convert.ToInt32(result);
			}

			//this.OnOutputDataReceived(new OutputDataReceivedEventArgs(process.StandardOutput));

			this.OnProcessCompleted(new ProcessCompletedEventArgs(processName, exitCode));
			return exitCode;
		}
		#endregion

		#region Events
		/// <summary>
		/// Occurs when the process is started.
		/// </summary>
		public event EventHandler<ProcessStartedEventArgs> ProcessStarted;

		/// <summary>
		/// Occurs when the process is completed.
		/// </summary>
		public event EventHandler<ProcessCompletedEventArgs> ProcessCompleted;

		/// <summary>
		/// Occurs when the instance receives output stream.
		/// </summary>
		public event EventHandler<OutputDataReceivedEventArgs> OutputDataReceived;
		#endregion

		#region Event Handlers
		/// <summary>
		/// Occurs when the process is started.
		/// </summary>
		/// <param name="e">Provides data for process started event.</param>
		protected virtual void OnProcessStarted(ProcessStartedEventArgs e)
		{
			if (this.ProcessStarted != null)
				this.ProcessStarted(this, e);
		}

		/// <summary>
		/// Occurs when the process is completed.
		/// </summary>
		/// <param name="e">Provides data for process completed event.</param>
		protected virtual void OnProcessCompleted(ProcessCompletedEventArgs e)
		{
			if (this.ProcessCompleted != null)
				this.ProcessCompleted(this, e);
		}

		/// <summary>
		/// Occurs when the instance receives output stream.
		/// </summary>
		/// <param name="e">Provides data for output data received event.</param>
		protected virtual void OnOutputDataReceived(OutputDataReceivedEventArgs e)
		{
			if (this.OutputDataReceived != null)
				this.OutputDataReceived(this, e);
		}
		#endregion
	}
}
