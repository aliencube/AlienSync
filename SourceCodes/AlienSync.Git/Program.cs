using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using WinSCP;
using AlienSync.Core;
using AlienSync.Core.Enums;
using AlienSync.Core.Events;
using AlienSync.Core.Exceptions;

namespace AlienSync.Git
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
			sb.AppendLine("  AlienSync.Git.exe");
			sb.AppendLine();
			sb.AppendLine("Configuration:");
			sb.AppendLine("  Enter correct information into AlienSync.Git.exe.config before executing this application.");
			sb.AppendLine();

			Console.WriteLine(sb.ToString());
		}

		/// <summary>
		/// Processes the requests.
		/// </summary>
		/// <param name="args">List of parameters manually set.</param>
		private static void ProcessRequests(string[] args)
		{
			var sync = new Synchronizer(args);
			sync.SynchronizationStarted += Sync_SynchronizationStarted;
			sync.SynchronizationCompleted += Sync_SynchronizationCompleted;

			sync.ScpSynchronizationStarted += Sync_ScpSynchronizationStarted;
			sync.ScpSynchronizationCompleted += Sync_ScpSynchronizationCompleted;

			sync.DirectorySynchronizationStarted += Sync_DirectorySynchronizationStarted;
			sync.DirectorySynchronizationCompleted += Sync_DirectorySynchronizationCompleted;
			sync.FileTransferred += Sync_FileTransferred;

			sync.GitSynchronizationStarted += Sync_GitSynchronizationStarted;
			sync.GitSynchronizationCompleted += Sync_GitSynchronizationCompleted;

			sync.ProcessStarted += Sync_ProcessStarted;
			sync.ProcessCompleted += Sync_ProcessCompleted;
			sync.OutputDataReceived += Sync_OutputDataReceived;

			sync.ProcessRequests(SynchronizationAction.ScpThenGit);
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
			Synchronizer.SaveLogs(((Synchronizer)sender).Settings.GetSeparator());

			Console.WriteLine(message);
		}

		/// <summary>
		/// Occurs when WinSCP synchronization process is started.
		/// </summary>
		/// <param name="sender">Object that triggers the WinSCP synchronization started event.</param>
		/// <param name="e">Provides data for event.</param>
		private static void Sync_ScpSynchronizationStarted(object sender, EventArgs e)
		{
			var message = "WinSCP Synchronization started ...\n";
			Synchronizer.SaveLogs(message);

			Console.WriteLine(message);
		}

		/// <summary>
		/// Occurs when WinSCP synchronization process is completed.
		/// </summary>
		/// <param name="sender">Object that triggers the WinSCP synchronization started event.</param>
		/// <param name="e">Provides data for event.</param>
		private static void Sync_ScpSynchronizationCompleted(object sender, EventArgs e)
		{
			var message = "WinSCP Synchronization completed!\n";
			Synchronizer.SaveLogs(message);

			Console.WriteLine(message);
		}

		/// <summary>
		/// Occurs when directory synchronization process is started.
		/// </summary>
		/// <param name="sender">Object that triggers the directory synchronization started event.</param>
		/// <param name="e">Provides data for directory synchronization started event.</param>
		private static void Sync_DirectorySynchronizationStarted(object sender, DirectorySynchronizationStartedEventArgs e)
		{
			var message = String.Format("Synchronizing {0}\n         with {1} ...\n", e.LocalDirectory, e.RemoteDirectory);
			Synchronizer.SaveLogs(message);

			Console.WriteLine(message);
		}

		/// <summary>
		/// Occurs when directory synchronization process is completed.
		/// </summary>
		/// <param name="sender">Object that triggers the directory synchronization completed event.</param>
		/// <param name="e">Provides data for event.</param>
		private static void Sync_DirectorySynchronizationCompleted(object sender, EventArgs e)
		{
			var message = "... Done!\n";
			Synchronizer.SaveLogs(message);

			Console.WriteLine(message);
		}

		/// <summary>
		/// Occurs when file is transfered.
		/// </summary>
		/// <param name="sender">Object that triggers the file transfer event.</param>
		/// <param name="e">Provides data for file transfer event.</param>
		private static void Sync_FileTransferred(object sender, TransferEventArgs e)
		{
			var sb = new StringBuilder();
			string message;
			if (e.Error == null)
			{
				message = String.Format("+File synchronized: {0}", e.FileName);
				sb.AppendLine(message);

				Console.WriteLine(message);
			}
			else
			{
				message = String.Format("-File not synchronized: {0} - {1}", e.FileName, e.Error);
				sb.AppendLine(message);

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(message);
				Console.ResetColor();
			}

			if (e.Chmod != null)
			{
				if (e.Chmod.Error == null)
				{
					message = String.Format("+Permisions set: {0} to {1}", e.Chmod.FileName, e.Chmod.FilePermissions);
					sb.AppendLine(message);

					Console.WriteLine(message);
				}
				else
				{
					message = String.Format("-Permissions not set  : {0} - {1}", e.Chmod.FileName, e.Chmod.Error);
					sb.AppendLine(message);

					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(message);
					Console.ResetColor();
				}
			}
			else
			{
				message = String.Format("+Permissions kept : {0}", e.Destination);
				sb.AppendLine(message);

				Console.WriteLine(message);
			}

			if (e.Touch != null)
			{
				if (e.Touch.Error == null)
				{
					message = String.Format("+Timestamp set    : {0} to {1}", e.Touch.FileName, e.Touch.LastWriteTime);
					sb.AppendLine(message);

					Console.WriteLine(message);
				}
				else
				{
					message = String.Format("-Timestamp not set    : {0} - {1}", e.Touch.FileName, e.Touch.Error);
					sb.AppendLine(message);

					Console.WriteLine(message);
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(message);
					Console.ResetColor();
				}
			}
			else
			{
				// This should never happen with Session.SynchronizeDirectories
				message = String.Format("+Timestamp kept   : {0}", e.Destination);
				sb.AppendLine(message);

				Console.WriteLine(message);
			}
			Console.WriteLine();
			Synchronizer.SaveLogs(sb.ToString());
		}

		/// <summary>
		/// Occurs when Git synchronization process is started.
		/// </summary>
		/// <param name="sender">Object that triggers the Git synchronization started event.</param>
		/// <param name="e">Provides data for event.</param>
		private static void Sync_GitSynchronizationStarted(object sender, EventArgs e)
		{
			var message = "Git Synchronization started ...\n";
			Synchronizer.SaveLogs(message);

			Console.WriteLine(message);
		}

		/// <summary>
		/// Occurs when Git synchronization process is started.
		/// </summary>
		/// <param name="sender">Object that triggers the Git synchronization completed event.</param>
		/// <param name="e">Provides data for event.</param>
		private static void Sync_GitSynchronizationCompleted(object sender, EventArgs e)
		{
			var message = "Git Synchronization completed!\n";
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

		/// <summary>
		/// Occurs when the instance receives output stream.
		/// </summary>
		/// <param name="sender">Object that triggers the output data received event.</param>
		/// <param name="e">Provides data for output data received event.</param>
		private static void Sync_OutputDataReceived(object sender, AlienSync.Core.Events.OutputDataReceivedEventArgs e)
		{
			var message = e.Output.ReadToEnd();
			Synchronizer.SaveLogs(message);

			Console.WriteLine(message);
		}
		#endregion
	}
}
