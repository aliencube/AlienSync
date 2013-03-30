using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
		private List<string> _tableNames;
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
				var query = String.Format(
					"SELECT t.[name] FROM sys.tables AS t JOIN sys.schemas AS s ON t.schema_id = s.schema_id WHERE s.[name] = '{0}' AND t.[name] <> 'sysdiagrams' ORDER BY t.[name] ASC",
					this._settings.MsSqlSourceDatabaseSchema);
				var psi = new ProcessStartInfo(this._settings.MsSqlCommandExecutablePath)
					{
						UseShellExecute = false,
						WorkingDirectory = this._settings.MsSqlScriptStoragePath,
						RedirectStandardInput = true,
						RedirectStandardOutput = true,
						Arguments = String.Format(
							"-S {0} -U {1} -P {2} -d {3} -Q \"{4}\" -o \"{5}\\tables.txt\"",
							this._settings.MsSqlSourceConnection.DataSource,
							this._settings.MsSqlSourceConnection.UserId,
							this._settings.MsSqlSourceConnection.Password,
							this._settings.MsSqlSourceConnection.InitialCatalog,
							query,
							this._settings.MsSqlScriptStoragePath)
					};
				process.StartInfo = psi;
				process.Start();

				this.OnOutputDataReceived(new OutputDataReceivedEventArgs(process.StandardOutput));

				process.WaitForExit();
				exitCode = process.ExitCode;
			}

			using (var file = File.OpenText(String.Format(@"{0}\tables.txt", this._settings.MsSqlScriptStoragePath)))
			{
				var lines = file.ReadToEnd()
				                .Split(new string[] {"\n", "\r"}, StringSplitOptions.RemoveEmptyEntries)
				                .Skip(2)
				                .Where(p => !p.StartsWith("("))
				                .ToList();
				this._tableNames = lines;
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

			//	Sets the exitcode 404, if no table name exists.
			var exitCode = 404;
			if (this._tableNames != null && this._tableNames.Any())
			{
				foreach (var tableName in this._tableNames)
				{
					this.OnProcessStarted(new ProcessStartedEventArgs(String.Format("{0} - {1}", processName, tableName)));

					var filename = String.Format(@"{0}\{1}.sql", this._settings.MsSqlScriptStoragePath, tableName);
					using (var process = new Process())
					{
						var psi = new ProcessStartInfo(this._settings.MsSqlTableDiffExecutablePath)
							{
								UseShellExecute = false,
								WorkingDirectory = this._settings.MsSqlScriptStoragePath,
								RedirectStandardInput = true,
								RedirectStandardOutput = true,
								Arguments = String.Format(
									"-sourceserver [{0}] -sourcedatabase [{1}] -sourceschema [{2}] -sourcetable [{3}] -sourceuser [{4}] -sourcepassword [{5}] -destinationserver [{6}] -destinationdatabase [{7}] -destinationschema [{8}] -destinationtable [{9}] -destinationuser [{10}] -destinationpassword [{11}] -dt -et {12} -f \"{13}\"",
									this._settings.MsSqlSourceConnection.DataSource,
									this._settings.MsSqlSourceConnection.InitialCatalog,
									this._settings.MsSqlSourceDatabaseSchema,
									tableName,
									this._settings.MsSqlSourceConnection.UserId,
									this._settings.MsSqlSourceConnection.Password,
									this._settings.MsSqlDestinationConnection.DataSource,
									this._settings.MsSqlDestinationConnection.InitialCatalog,
									this._settings.MsSqlDestinationDatabaseSchema,
									tableName,
									this._settings.MsSqlDestinationConnection.UserId,
									this._settings.MsSqlDestinationConnection.Password,
									"TableDiffs",
									filename)
							};
						process.StartInfo = psi;
						process.Start();

						this.OnOutputDataReceived(new OutputDataReceivedEventArgs(process.StandardOutput));

						process.WaitForExit();
						exitCode = process.ExitCode;
					}

					if (File.Exists(filename))
						this.CleanseScript(filename);

					this.OnProcessCompleted(new ProcessCompletedEventArgs(String.Format("{0} - {1}", processName, tableName), exitCode));
					if (exitCode > 0)
						break;
				}
			}

			this.OnProcessCompleted(new ProcessCompletedEventArgs(processName, exitCode));
			return exitCode;
		}

		/// <summary>
		/// Cleanses the SQL script file for synchronization.
		/// </summary>
		/// <param name="filepath">Full path of the filename to be cleansed.</param>
		/// <remarks>As TableDiff.exe has a bug to handle NULL value, this cleansing process needs to be done.</remarks>
		private void CleanseScript(string filepath)
		{
			string data;
			using (var file = File.OpenText(filepath))
			{
				data = file.ReadToEnd().Replace("N'Null'", "Null");
			}
			File.WriteAllText(filepath, data);
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
