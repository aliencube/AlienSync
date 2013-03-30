using System;
using System.IO;
using System.Linq;
using AlienSync.Core.Enums;
using WinSCP;
using AlienSync.Core.Events;
using AlienSync.Core.Wrappers;

namespace AlienSync.Core
{
	/// <summary>
	/// This represents the Synchronizer entity that is the main entiry point for sync.
	/// </summary>
	public class Synchronizer
	{
		#region Constructors
		/// <summary>
		/// Initialises a new instance of the Synchronizer object.
		/// </summary>
		/// <param name="args">List of parameters manually set.</param>
		public Synchronizer(string[] args = null)
		{
			this._parameters = new Parameters(args);
		}
		#endregion

		#region Properties
		private readonly Parameters _parameters;

		/// <summary>
		/// Gets the session options.
		/// </summary>
		public SessionOptions Options
		{
			get
			{
				if (this._options == null)
					this._options = this._parameters.SessionOptions;
				return this._options;
			}
		}
		private SessionOptions _options;

		/// <summary>
		/// Gets the configuration settings.
		/// </summary>
		public Settings Settings
		{
			get
			{
				if (this._settings == null)
					this._settings = Settings.Instance;
				return this._settings;
			}
		}
		private Settings _settings;

		/// <summary>
		/// Gets the value that specifies whether WinSCP is installed or not.
		/// </summary>
		public bool WinScpInstalled
		{
			get { return File.Exists(this.Settings.ScpExecutablePath); }
		}

		/// <summary>
		/// Gets the value that specifies whether Git is installed or not.
		/// </summary>
		public bool GitInstalled
		{
			get { return File.Exists(this.Settings.GitExecutablePath); }
		}

		/// <summary>
		/// Gets the value that specifies whether SSIS package exists or not.
		/// </summary>
		public bool MsSqlCommandInstalled
		{
			get { return File.Exists(this.Settings.MsSqlCommandExecutablePath); }
		}

		/// <summary>
		/// Gets the value that specifies whether TableDiff is installed or not.
		/// </summary>
		public bool MsSqlTableDiffInstalled
		{
			get { return File.Exists(this.Settings.MsSqlTableDiffExecutablePath); }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Processes the synchronization requests.
		/// </summary>
		/// <param name="action">Action for synchronization. Default value is <c>SynchronizationAction.ScpOnly</c>.</param>
		public void ProcessRequests(SynchronizationAction action = SynchronizationAction.ScpOnly)
		{
			//	Subscribes the SynchronizationStarted event.
			this.OnSynchronizationStarted(new SynchronizationStartedEventArgs(DateTime.Now));

			switch (action)
			{
				case SynchronizationAction.ScpOnly:
					//	Processes the sync through WinSCP.
					this.ProcessRequestsWithScp();
					break;
				case SynchronizationAction.ScpThenGit:
					//	Processes the sync through WinSCP.
					this.ProcessRequestsWithScp();

					//	Processes the sync through Git.
					this.ProcessRequestsWithGit();
					break;
				case SynchronizationAction.ScpThenHg:
					//	Processes the sync through WinSCP.
					this.ProcessRequestsWithScp();

					//	Processes the sync through Hg.
					this.ProcessRequestsWithHg();
					break;
				case SynchronizationAction.MsSqlOnly:
					//	Processes the sync through MS-SQL.
					this.ProcessRequestsWithMsSql();
					break;
			}

			//	Subscribes the SynchronizationCompleted event.
			this.OnSynchronizationCompleted(new SynchronizationCompletedEventArgs(DateTime.Now));
		}

		/// <summary>
		/// Processes the synchronization requests through SCP.
		/// </summary>
		private void ProcessRequestsWithScp()
		{
			this.OnScpSynchronizationStarted();

			//	Checks whether WinSCP has been installed or not.
			if (!this.WinScpInstalled)
				throw new FileNotFoundException("WinSCP cannot be found at the designated location.");

			using (var session = new Session())
			{
				session.FileTransferred += Session_FileTransferred;
				session.ExecutablePath = this.Settings.ScpExecutablePath;

				session.Open(this.Options);
				//	Synchronizes local directories with remote directories.
				//	It only considers directories declared in both local and remote.
				foreach (var remote in this.Settings
				                           .ScpRemoteDirectories
				                           .Where(p => this.Settings.ScpLocalDirectories.ContainsKey(p.Key)))
				{
					if (!session.Opened)
						session.Open(this.Options);

					this.OnDirectorySynchronizationStarted(new DirectorySynchronizationStartedEventArgs(
						                                       this.Settings.ScpLocalDirectories[remote.Key],
						                                       remote.Value));

					var result = session.SynchronizeDirectories(
						this.Settings.ScpSynchronizationMode,
						this.Settings.ScpLocalDirectories[remote.Key],
						remote.Value,
						this.Settings.ScpRemoveFiles,
						this.Settings.ScpMirrorMode,
						this.Settings.ScpSynchronizationCriteria,
						this.Settings.ScpTransferOptions);

					result.Check();

					this.OnDirectorySynchronizationCompleted();
				}
			}

			this.OnScpSynchronizationCompleted();
		}

		/// <summary>
		/// Processes the synchronization requests through Git.
		/// </summary>
		private void ProcessRequestsWithGit()
		{
			this.OnGitSynchronizationStarted();

			//	Checks whether Git has been installed or not.
			if (!this.GitInstalled)
				throw new FileNotFoundException("Git cannot be found at the designated location.");

			var git = new GitWrapper(this.Settings);
			git.ProcessStarted += Wrapper_ProcessStarted;
			git.ProcessCompleted += Wrapper_ProcessCompleted;
			git.OutputDataReceived += Wrapper_OutputDataReceived;

			var pulled = git.Pull();
			if (pulled > 0)
			{
				this.OnGitSynchronizationCompleted();
				return;
			}

			var added = git.Add();
			if (added > 0)
			{
				this.OnGitSynchronizationCompleted();
				return;
			}

			var commited = git.Commit();
			if (commited > 0)
			{
				this.OnGitSynchronizationCompleted();
				return;
			}

			var pushed = git.Push();
			this.OnGitSynchronizationCompleted();
		}

		/// <summary>
		/// Processes the synchronization requests through Hg.
		/// </summary>
		private void ProcessRequestsWithHg()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Processes the synchronization requests through MS-SQL.
		/// </summary>
		private void ProcessRequestsWithMsSql()
		{
			this.OnMsSqlSynchronizationStarted();

			//	Checks whether MS-SQL command has been installed or not.
			if (!this.MsSqlCommandInstalled)
				throw new FileNotFoundException("SQLCMD.exe cannot be found at the designated location.");

			//	Checks whether TableDiff has been installed or not.
			if (!this.MsSqlTableDiffInstalled)
				throw new FileNotFoundException("TableDiff.exe cannot be found at the designated location.");

			var wrapper = new MsSqlWrapper(this.Settings);
			wrapper.ProcessStarted += Wrapper_ProcessStarted;
			wrapper.ProcessCompleted += Wrapper_ProcessCompleted;
			wrapper.OutputDataReceived += Wrapper_OutputDataReceived;

			var directoryCleanedUp = wrapper.CleanUpDirectory();
			if (directoryCleanedUp > 0)
			{
				this.OnMsSqlSynchronizationCompleted();
				return;
			}

			var tablesGot = wrapper.GetTables();
			if (tablesGot > 0)
			{
				this.OnMsSqlSynchronizationCompleted();
				return;
			}

			var scriptsGenerated = wrapper.GenerateScripts();
			if (scriptsGenerated > 0)
			{
				this.OnMsSqlSynchronizationCompleted();
				return;
			}

			var differencesApplied = wrapper.ApplyDifferences();
			this.OnMsSqlSynchronizationCompleted();
		}

		/// <summary>
		/// Saves the message into the log file.
		/// </summary>
		/// <param name="message">Message to save log.</param>
		public static void SaveLogs(string message)
		{
			if (!Directory.Exists(Settings.Instance.LogPath))
				Directory.CreateDirectory(Settings.Instance.LogPath);

			using (var log = File.AppendText(String.Format(@"{0}\log-{1:yyyy-MM-dd}.txt",
			                                               Settings.Instance.LogPath, DateTime.Now)))
			{
				log.WriteLine(message);
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Occurs when synchronization process is started.
		/// </summary>
		public event EventHandler<SynchronizationStartedEventArgs> SynchronizationStarted;

		/// <summary>
		/// Occurs when synchronization process is completed.
		/// </summary>
		public event EventHandler<SynchronizationCompletedEventArgs> SynchronizationCompleted;

		/// <summary>
		/// Occurs when WinSCP synchronization process is started.
		/// </summary>
		public event EventHandler<EventArgs> ScpSynchronizationStarted;

		/// <summary>
		/// Occurs when WinSCP synchronization process is completed.
		/// </summary>
		public event EventHandler<EventArgs> ScpSynchronizationCompleted;

		/// <summary>
		/// Occurs when directory synchronization process is started.
		/// </summary>
		public event EventHandler<DirectorySynchronizationStartedEventArgs> DirectorySynchronizationStarted;

		/// <summary>
		/// Occurs when directory synchronization process is completed.
		/// </summary>
		public event EventHandler<EventArgs> DirectorySynchronizationCompleted;

		/// <summary>
		/// Occurs when file is transfered.
		/// </summary>
		/// <remarks>This event is used to handle the bubbled up FileTransferred event.</remarks>
		public event EventHandler<TransferEventArgs> FileTransferred;

		/// <summary>
		/// Occurs when Git synchronization process is started.
		/// </summary>
		public event EventHandler<EventArgs> GitSynchronizationStarted;

		/// <summary>
		/// Occurs when Git synchronization process is completed.
		/// </summary>
		public event EventHandler<EventArgs> GitSynchronizationCompleted;

		/// <summary>
		/// Occurs when Hg synchronization process is started.
		/// </summary>
		public event EventHandler<EventArgs> HgSynchronizationStarted;

		/// <summary>
		/// Occurs when Hg synchronization process is completed.
		/// </summary>
		public event EventHandler<EventArgs> HgSynchronizationCompleted;

		/// <summary>
		/// Occurs when the process is started.
		/// </summary>
		/// <remarks>This event is used to handle the bubbled up ProcessStarted event.</remarks>
		public event EventHandler<ProcessStartedEventArgs> ProcessStarted;

		/// <summary>
		/// Occurs when the process is completed.
		/// </summary>
		/// <remarks>This event is used to handle the bubbled up ProcessCompleted event.</remarks>
		public event EventHandler<ProcessCompletedEventArgs> ProcessCompleted;

		/// <summary>
		/// Occurs when the instance writes to its redirected System.Diagnostics.Process.StandardOutput stream.
		/// </summary>
		/// <remarks>This event is used to handle the bubbled up OutputDataReceived event.</remarks>
		public event EventHandler<AlienSync.Core.Events.OutputDataReceivedEventArgs> OutputDataReceived;

		/// <summary>
		/// Occurs when MS-SQL synchronization process is started.
		/// </summary>
		public event EventHandler<EventArgs> MsSqlSynchronizationStarted;

		/// <summary>
		/// Occurs when MS-SQL synchronization process is completed.
		/// </summary>
		public event EventHandler<EventArgs> MsSqlSynchronizationCompleted;
		#endregion

		#region Event Handlers
		/// <summary>
		/// Occurs when synchronization process is started.
		/// </summary>
		/// <param name="e">Provides data for synchronization started event.</param>
		protected virtual void OnSynchronizationStarted(SynchronizationStartedEventArgs e)
		{
			if (this.SynchronizationStarted != null)
				this.SynchronizationStarted(this, e);
		}

		/// <summary>
		/// Occurs when synchronization process is completed.
		/// </summary>
		/// <param name="e">Provides data for synchronization completed event.</param>
		protected virtual void OnSynchronizationCompleted(SynchronizationCompletedEventArgs e)
		{
			if (this.SynchronizationCompleted != null)
				this.SynchronizationCompleted(this, e);
		}

		/// <summary>
		/// Occurs when directory synchronization process is started.
		/// </summary>
		/// <param name="e">Provides data for directory synchronization started event.</param>
		protected virtual void OnDirectorySynchronizationStarted(DirectorySynchronizationStartedEventArgs e)
		{
			if (this.DirectorySynchronizationStarted != null)
				this.DirectorySynchronizationStarted(this, e);
		}

		/// <summary>
		/// Occurs when directory synchronization process is completed.
		/// </summary>
		protected virtual void OnDirectorySynchronizationCompleted()
		{
			if (this.DirectorySynchronizationCompleted != null)
				this.DirectorySynchronizationCompleted(this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when file is transfered.
		/// </summary>
		/// <param name="sender">Object that triggers the file transfer event.</param>
		/// <param name="e">Provides data for file transfer event.</param>
		protected void Session_FileTransferred(object sender, TransferEventArgs e)
		{
			//	Bubbles up the event to the parent.
			if (this.FileTransferred != null)
				this.FileTransferred(sender, e);
		}

		/// <summary>
		/// Occurs when WinSCP synchronization process is started.
		/// </summary>
		protected virtual void OnScpSynchronizationStarted()
		{
			if (this.ScpSynchronizationStarted != null)
				this.ScpSynchronizationStarted(this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when WinSCP synchronization process is completed.
		/// </summary>
		protected virtual void OnScpSynchronizationCompleted()
		{
			if (this.ScpSynchronizationCompleted != null)
				this.ScpSynchronizationCompleted(this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when Git synchronization process is started.
		/// </summary>
		protected virtual void OnGitSynchronizationStarted()
		{
			if (this.GitSynchronizationStarted != null)
				this.GitSynchronizationStarted(this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when Git synchronization process is completed.
		/// </summary>
		protected virtual void OnGitSynchronizationCompleted()
		{
			if (this.GitSynchronizationCompleted != null)
				this.GitSynchronizationCompleted(this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when Hg synchronization process is started.
		/// </summary>
		protected virtual void OnHgSynchronizationStarted()
		{
			if (this.HgSynchronizationStarted != null)
				this.HgSynchronizationStarted(this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when Hg synchronization process is completed.
		/// </summary>
		protected virtual void OnHgSynchronizationCompleted()
		{
			if (this.HgSynchronizationCompleted != null)
				this.HgSynchronizationCompleted(this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when MS-SQL synchronization process is started.
		/// </summary>
		protected virtual void OnMsSqlSynchronizationStarted()
		{
			if (this.MsSqlSynchronizationStarted != null)
				this.MsSqlSynchronizationStarted(this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when MS-SQL synchronization process is completed.
		/// </summary>
		protected virtual void OnMsSqlSynchronizationCompleted()
		{
			if (this.MsSqlSynchronizationCompleted != null)
				this.MsSqlSynchronizationCompleted(this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when the process is started.
		/// </summary>
		/// <param name="sender">Object that triggers the process started event.</param>
		/// <param name="e">Provides data for process started event.</param>
		protected void Wrapper_ProcessStarted(object sender, ProcessStartedEventArgs e)
		{
			if (this.ProcessStarted != null)
				this.ProcessStarted(this, e);
		}

		/// <summary>
		/// Occurs when the process is completed.
		/// </summary>
		/// <param name="sender">Object that triggers the process started event.</param>
		/// <param name="e">Provides data for process completed event.</param>
		protected void Wrapper_ProcessCompleted(object sender, ProcessCompletedEventArgs e)
		{
			if (this.ProcessCompleted != null)
				this.ProcessCompleted(this, e);
		}

		/// <summary>
		/// Occurs when the instance receives output stream.
		/// </summary>
		/// <param name="sender">Object that triggers the output data received event.</param>
		/// <param name="e">Provides data for output data received event.</param>
		protected void Wrapper_OutputDataReceived(object sender, AlienSync.Core.Events.OutputDataReceivedEventArgs e)
		{
			if (this.OutputDataReceived != null)
				this.OutputDataReceived(sender, e);
		}
		#endregion
	}
}
