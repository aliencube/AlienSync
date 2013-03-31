using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using AlienSync.Core;
using AlienSync.Core.Enums;
using AlienSync.Core.Events;
using AlienSync.Core.Exceptions;

namespace AlienSync.MsSql
{
	/// <summary>
	/// This represents the main program entity.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Executes the console app.
		/// </summary>
		/// <param name="args">List of parameters manually set.</param>
		public static void Main(string[] args)
		{
			Splash();
			try
			{
				ProcessRequests(args);
			}
			catch (FileNotFoundException ex)
			{
				ShowMessage(ex);
				ShowUsage();
			}
			catch (InvalidConfigurationException ex)
			{
				ShowMessage(ex);
				ShowUsage();
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}

		#region Methods

		/// <summary>
		///	Shows the splash message.
		/// </summary>
		private static void Splash()
		{
			var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
			var sb = new StringBuilder();
			sb.AppendLine(String.Format("{0} v{1}", fvi.ProductName, fvi.FileVersion));
			sb.AppendLine("------------------------------");

			Console.WriteLine(sb.ToString());
		}

		/// <summary>
		/// Shows the message.
		/// </summary>
		/// <param name="ex">Exception.</param>
		private static void ShowMessage(Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Ooops!");
			Console.ResetColor();
			Console.WriteLine(ex.Message);

			Synchronizer.SaveLogs(ex.Message);
			Synchronizer.SaveLogs(ex.StackTrace);
		}

		/// <summary>
		/// Shows the usage message.
		/// </summary>
		private static void ShowUsage()
		{
			var sb = new StringBuilder();
			sb.AppendLine("Usage:");
			sb.AppendLine("  AlienSync.MsSql.exe");
			sb.AppendLine();
			sb.AppendLine("Configuration:");
			sb.AppendLine("  Enter correct information into AlienSync.MsSql.exe.config before executing this application.");
			sb.AppendLine();

			Console.WriteLine(sb.ToString());
		}

		/// <summary>
		/// Processes the requests.
		/// </summary>
		/// <param name="args">List of parameters manually set.</param>
		private static void ProcessRequests(string[] args)
		{
			var sync = new Synchronizer(args, SynchronizationAction.MsSqlOnly);
			sync.SynchronizationStarted += Sync_SynchronizationStarted;
			sync.SynchronizationCompleted += Sync_SynchronizationCompleted;

			sync.MsSqlSynchronizationStarted += Sync_MsSqlSynchronizationStarted;
			sync.MsSqlSynchronizationCompleted += Sync_MsSqlSynchronizationCompleted;

			sync.ProcessStarted += Sync_ProcessStarted;
			sync.ProcessCompleted += Sync_ProcessCompleted;

			sync.ProcessRequests();
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Occurs when synchronization process is started.
		/// </summary>
		/// <param name="sender">Object that triggers the synchronization started event.</param>
		/// <param name="e">Provides data for synchronization started event.</param>
		private static void Sync_SynchronizationStarted(object sender, SynchronizationStartedEventArgs e)
		{
			var message = String.Format("Synchronization started at {0:yyyy-MM-dd HH:mm:ss}\n", e.DateStarted);
			Synchronizer.SaveLogs(message);

			Console.WriteLine(message);
		}

		/// <summary>
		/// Occurs when synchronization process is completed.
		/// </summary>
		/// <param name="sender">Object that triggers the synchronization completed event.</param>
		/// <param name="e">Provides data for synchronization completed event.</param>
		private static void Sync_SynchronizationCompleted(object sender, SynchronizationCompletedEventArgs e)
		{
			var message = String.Format("Synchronization completed at {0:yyyy-MM-dd HH:mm:ss}", e.DateCompleted);
			Synchronizer.SaveLogs(message);
			Synchronizer.SaveLogs(((Synchronizer) sender).Settings.GetSeparator());

			Console.WriteLine(message);
		}

		/// <summary>
		/// Occurs when MS-SQL synchronization process is started.
		/// </summary>
		/// <param name="sender">Object that triggers the MS-SQL synchronization started event.</param>
		/// <param name="e">Provides data for event.</param>
		private static void Sync_MsSqlSynchronizationStarted(object sender, EventArgs e)
		{
			var message = "MS-SQL Synchronization started ...\n";
			Synchronizer.SaveLogs(message);

			Console.WriteLine(message);
		}

		/// <summary>
		/// Occurs when MS-SQL synchronization process is started.
		/// </summary>
		/// <param name="sender">Object that triggers the MS-SQL synchronization completed event.</param>
		/// <param name="e">Provides data for event.</param>
		private static void Sync_MsSqlSynchronizationCompleted(object sender, EventArgs e)
		{
			var message = "MS-SQL Synchronization completed!\n";
			Synchronizer.SaveLogs(message);

			Console.WriteLine(message);
		}

		/// <summary>
		/// Occurs when the process is started.
		/// </summary>
		/// <param name="sender">Object that triggers the process started event.</param>
		/// <param name="e">Provides data for event.</param>
		private static void Sync_ProcessStarted(object sender, ProcessStartedEventArgs e)
		{
			var message = String.Format("{0} started ...\n", e.ProcessName);
			Synchronizer.SaveLogs(message);

			Console.WriteLine(message);
		}

		/// <summary>
		/// Occurs when the process is completed.
		/// </summary>
		/// <param name="sender">Object that triggers the process started event.</param>
		/// <param name="e">Provides data for process completed event.</param>
		private static void Sync_ProcessCompleted(object sender, ProcessCompletedEventArgs e)
		{
			var message = new StringBuilder();
			if (e.ExitCode > 0)
				message.AppendLine(String.Format("Exit Code: {0}", e.ExitCode));
			message.AppendLine(String.Format("{0} completed!\n", e.ProcessName));

			Synchronizer.SaveLogs(message.ToString());

			Console.WriteLine(message);
		}
		#endregion
	}
}
