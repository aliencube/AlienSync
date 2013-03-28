using System;
using System.Diagnostics;
using AlienSync.Core.Enums;
using AlienSync.Core.Events;

namespace AlienSync.Core.Wrappers
{
	/// <summary>
	/// This represents the MS-SQL wrapper entity.
	/// </summary>
	public class MsSqlWrapper
	{
		#region Constructors
		/// <summary>
		/// Initialises a new instance of the MsSqlWrapper object.
		/// </summary>
		/// <param name="settings">Configuration settings.</param>
		public MsSqlWrapper(Settings settings)
		{
			this._settings = settings;
		}
		#endregion

		#region Properties
		private readonly Settings _settings;
		#endregion

		#region Methods
		/// <summary>
		/// Clean up the results directory before the process.
		/// </summary>
		/// <returns>Returns exit code. For successful execution will return 0.</returns>
		public int CleanUpDirectory()
		{
			var processName = Convert.ToString(MsSqlAction.CleanUpDirectory);
			this.OnProcessStarted(new ProcessStartedEventArgs(processName));

			int exitCode;
			using (var process = new Process())
			{
				var psi = new ProcessStartInfo("del")
					{
						UseShellExecute = false,
						WorkingDirectory = this._settings.MsSqlScriptStoragePath,
						RedirectStandardInput = true,
						RedirectStandardOutput = true,
						Arguments = String.Format(@"{0}\*.* /q", this._settings.MsSqlScriptStoragePath)
					};
				process.StartInfo = psi;
				process.Start();

				this.OnOutputDataReceived(new OutputDataReceivedEventArgs(process.StandardOutput));

				process.WaitForExit();
				exitCode = process.ExitCode;
			}

			this.OnProcessCompleted(new ProcessCompletedEventArgs(processName, exitCode));
			return exitCode;
		}

		/// <summary>
		/// Gets the list of tables for synchronization.
		/// </summary>
		/// <returns>Returns exit code. For successful execution will return 0.</returns>
		public int GetTables()
		{
			var processName = Convert.ToString(MsSqlAction.GetTables);
			this.OnProcessStarted(new ProcessStartedEventArgs(processName));

			int exitCode;
			using (var process = new Process())
			{
				var query =
					String.Format(
						"SELECT t.[name] FROM sys.tables AS t JOIN sys.schemas AS s ON t.schema_id = s.schema_id WHERE s.[name] = '{0}' AND t.[name] <> 'sysdiagrams' ORDER BY t.[name] ASC",
						this._settings.MsSqlSourceDatabaseSchema);
				var psi = new ProcessStartInfo(this._settings.MsSqlCommandExecutablePath)
					{
						UseShellExecute = false,
						WorkingDirectory = this._settings.MsSqlScriptStoragePath,
						RedirectStandardInput = true,
						RedirectStandardOutput = true,
						Arguments =
							String.Format("-S localhost -U username -P password -d AlienSync_Source -Q \"{0}\" -o \"{1}\\tables.txt\"",
							              query,
							              this._settings.MsSqlScriptStoragePath)
					};
				process.StartInfo = psi;
				process.Start();

				this.OnOutputDataReceived(new OutputDataReceivedEventArgs(process.StandardOutput));

				process.WaitForExit();
				exitCode = process.ExitCode;
			}

			this.OnProcessCompleted(new ProcessCompletedEventArgs(processName, exitCode));
			return exitCode;
		}

		/// <summary>
		/// Generates the list of SQL scripts for synchronization.
		/// </summary>
		/// <returns>Returns exit code. For successful execution will return 0.</returns>
		public int GenerateScripts()
		{
			var processName = Convert.ToString(MsSqlAction.GenerateScripts);
			this.OnProcessStarted(new ProcessStartedEventArgs(processName));

			int exitCode;
			using (var process = new Process())
			{
				var psi = new ProcessStartInfo(this._settings.MsSqlCommandExecutablePath)
				{
					UseShellExecute = false,
					WorkingDirectory = this._settings.MsSqlScriptStoragePath,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					Arguments = String.Format(
						"--git-dir={0} --work-tree={1} pull -v --progress \"origin\" {2}",
						"",
						"",
						this._settings.GitBranchName)
				};
				process.StartInfo = psi;
				process.Start();

				this.OnOutputDataReceived(new OutputDataReceivedEventArgs(process.StandardOutput));

				process.WaitForExit();
				exitCode = process.ExitCode;
			}

			this.OnProcessCompleted(new ProcessCompletedEventArgs(processName, exitCode));
			return exitCode;
		}

		/// <summary>
		/// Cleanses the list of SQL scripts for synchronization.
		/// </summary>
		/// <returns>Returns exit code. For successful execution will return 0.</returns>
		/// <remarks>As TableDiff.exe has a bug to handle NULL value, this cleansing process needs to be done.</remarks>
		public int CleanseScripts()
		{
			var processName = Convert.ToString(MsSqlAction.CleanseScripts);
			this.OnProcessStarted(new ProcessStartedEventArgs(processName));

			int exitCode;
			using (var process = new Process())
			{
				var psi = new ProcessStartInfo(this._settings.MsSqlCommandExecutablePath)
				{
					UseShellExecute = false,
					WorkingDirectory = this._settings.MsSqlScriptStoragePath,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					Arguments = String.Format(
						"--git-dir={0} --work-tree={1} pull -v --progress \"origin\" {2}",
						"",
						"",
						this._settings.GitBranchName)
				};
				process.StartInfo = psi;
				process.Start();

				this.OnOutputDataReceived(new OutputDataReceivedEventArgs(process.StandardOutput));

				process.WaitForExit();
				exitCode = process.ExitCode;
			}

			this.OnProcessCompleted(new ProcessCompletedEventArgs(processName, exitCode));
			return exitCode;
		}

		/// <summary>
		/// Applies the list of SQL scripts for synchronization.
		/// </summary>
		/// <returns>Returns exit code. For successful execution will return 0.</returns>
		public int ApplyDifferences()
		{
			var processName = Convert.ToString(MsSqlAction.ApplyDifferences);
			this.OnProcessStarted(new ProcessStartedEventArgs(processName));

			int exitCode;
			using (var process = new Process())
			{
				var psi = new ProcessStartInfo(this._settings.MsSqlCommandExecutablePath)
				{
					UseShellExecute = false,
					WorkingDirectory = this._settings.MsSqlScriptStoragePath,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					Arguments = String.Format(
						"--git-dir={0} --work-tree={1} pull -v --progress \"origin\" {2}",
						"",
						"",
						this._settings.GitBranchName)
				};
				process.StartInfo = psi;
				process.Start();

				this.OnOutputDataReceived(new OutputDataReceivedEventArgs(process.StandardOutput));

				process.WaitForExit();
				exitCode = process.ExitCode;
			}

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
