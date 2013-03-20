using System;
using System.Diagnostics;
using System.IO;
using AlienSync.Core.Enums;
using AlienSync.Core.Events;

namespace AlienSync.Core.Wrappers
{
	/// <summary>
	/// This represents the Git wrapper entity.
	/// </summary>
	public class GitWrapper
	{
		#region Constructors
		/// <summary>
		/// Initialises a new instance of the GitWrapper object.
		/// </summary>
		/// <param name="settings">Configuration settings.</param>
		public GitWrapper(Settings settings)
		{
			this._settings = settings;
		}
		#endregion

		#region Properties
		private readonly Settings _settings;

		/// <summary>
		/// Gets the value that specifies whether the given local repository path is valid or not.
		/// </summary>
		public bool IsValidLocalRepository
		{
			get
			{
				var path = this._settings.GitLocalRepositoryPath;
				if (!path.EndsWith(".git"))
					path += @"\.git";
				return Directory.Exists(path);
			}
		}

		/// <summary>
		/// Gets the Git directory path.
		/// </summary>
		public string GitDirectoryPath
		{
			get
			{
				if (String.IsNullOrEmpty(this._gitDirectoryPath))
					this._gitDirectoryPath = this.IsValidLocalRepository
												 ? String.Format(@"{0}\.git", this._settings.GitLocalRepositoryPath)
						                         : String.Empty;
				return this._gitDirectoryPath;
			}
		}
		private string _gitDirectoryPath;

		/// <summary>
		/// Gets the git work tree.
		/// </summary>
		public string GitWorkTree
		{
			get
			{
				if (String.IsNullOrEmpty(this._gitWorkTree))
					this._gitWorkTree = this.IsValidLocalRepository
											? this._settings.GitLocalRepositoryPath
						                    : String.Empty;
				return this._gitWorkTree;
			}
		}
		private string _gitWorkTree;

		#endregion

		#region Methods
		/// <summary>
		/// Pulls the change from the remote repository to local repository.
		/// </summary>
		/// <returns>Returns exit code. For successful execution will return 0.</returns>
		public int Pull()
		{
			var processName = Convert.ToString(RepositoryAction.Pull);
			this.OnProcessStarted(new ProcessStartedEventArgs(processName));

			int exitCode;
			using (var process = new Process())
			{
				var psi = new ProcessStartInfo(this._settings.GitExecutablePath)
					          {
						          UseShellExecute = false,
								  WorkingDirectory = this._settings.GitLocalRepositoryPath,
						          RedirectStandardInput = true,
						          RedirectStandardOutput = true,
						          Arguments = String.Format(
									  "--git-dir={0} --work-tree={1} pull -v --progress \"origin\" {2}",
							          this.GitDirectoryPath,
							          this.GitWorkTree,
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
		/// Adds the all unversioned change from the working directory to local repository.
		/// </summary>
		/// <returns>Returns exit code. For successful execution will return 0.</returns>
		public int Add()
		{
			var processName = Convert.ToString(RepositoryAction.Add);
			this.OnProcessStarted(new ProcessStartedEventArgs(processName));

			int exitCode;
			using (var process = new Process())
			{
				var psi = new ProcessStartInfo(this._settings.GitExecutablePath)
					          {
						          UseShellExecute = false,
								  WorkingDirectory = this._settings.GitLocalRepositoryPath,
						          RedirectStandardInput = true,
						          RedirectStandardOutput = true,
						          Arguments = String.Format(
							          "--git-dir={0} --work-tree={1} add -v {2}",
							          this.GitDirectoryPath,
							          this.GitWorkTree,
							          this._settings.GitPatternForAddition)
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
		/// Commits the change from the working directory to local repository.
		/// </summary>
		/// <returns>Returns exit code. For successful execution will return 0.</returns>
		public int Commit()
		{
			var processName = Convert.ToString(RepositoryAction.Commit);
			this.OnProcessStarted(new ProcessStartedEventArgs(processName));

			int exitCode;
			using (var process = new Process())
			{
				var psi = new ProcessStartInfo(this._settings.GitExecutablePath)
					          {
						          UseShellExecute = false,
								  WorkingDirectory = this._settings.GitLocalRepositoryPath,
						          RedirectStandardInput = true,
						          RedirectStandardOutput = true,
						          Arguments = String.Format(
							          "--git-dir={0} --work-tree={1} commit -a -m \"{2}\"",
							          this.GitDirectoryPath,
							          this.GitWorkTree,
							          this._settings.GitCommitMessage)
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
		/// Pushes the change from the local repository to remote repository.
		/// </summary>
		/// <returns>Returns exit code. For successful execution will return 0.</returns>
		public int Push()
		{
			var processName = Convert.ToString(RepositoryAction.Push);
			this.OnProcessStarted(new ProcessStartedEventArgs(processName));

			int exitCode;
			using (var process = new Process())
			{
				var psi = new ProcessStartInfo(this._settings.GitExecutablePath)
					          {
						          UseShellExecute = false,
								  WorkingDirectory = this._settings.GitLocalRepositoryPath,
						          RedirectStandardInput = true,
						          RedirectStandardOutput = true,
						          Arguments = String.Format(
							          "--git-dir={0} --work-tree={1} push --progress \"origin\" {2}:{2}",
							          this.GitDirectoryPath,
							          this.GitWorkTree,
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
