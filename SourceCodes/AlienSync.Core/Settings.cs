using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using WinSCP;
using AlienSync.Core.Configuration;
using AlienSync.Core.Exceptions;

namespace AlienSync.Core
{
	/// <summary>
	/// This represents the application configuration settings entity.
	/// </summary>
	public class Settings
	{
		#region Constructors
		/// <summary>
		///	Initialises a new instance of the Settings object as private.
		/// </summary>
		private Settings()
		{
		}
		#endregion

		#region Properties

		#region For Common
		/// <summary>
		/// Gets the instance of the settings object.
		/// </summary>
		public static Settings Instance
		{
			get
			{
				if (_instance == null)
					_instance = new Settings { _directories = (DirectoryConfiguration)ConfigurationManager.GetSection("scpDirectorySettings") };
				return _instance;
			}
		}
		private static Settings _instance;

		/// <summary>
		/// Gets the list of connection strings.
		/// </summary>
		public Dictionary<string, string> ConnectionStrings
		{
			get
			{
				if (this._connectionStrings == null || !this._connectionStrings.Any())
				{
					this._connectionStrings = new Dictionary<string, string>();
					foreach (ConnectionStringSettings setting in ConfigurationManager.ConnectionStrings)
						this._connectionStrings.Add(setting.Name, setting.ConnectionString);
				}
				return this._connectionStrings;
			}
		}
		private Dictionary<string, string> _connectionStrings;

		/// <summary>
		/// Gets the directory path to store log files.
		/// </summary>
		public string LogPath
		{
			get
			{
				var path = ConfigurationManager.AppSettings["LogPath"];
				if (String.IsNullOrEmpty(path))
					path = @"\logs";
				var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				if (!String.IsNullOrEmpty(directory))
					path = String.Format("{0}{1}", directory.TrimEnd('/', '\\'), path.TrimEnd('/', '\\'));
				return path;
			}
		}
		#endregion



		#region For SCP
		private DirectoryConfiguration _directories;

		/// <summary>
		/// Gets the list of local directories for SCP synchronization.
		/// </summary>
		public Dictionary<string, string> ScpLocalDirectories
		{
			get
			{
				if (this._localDirectories == null || !this._localDirectories.Any())
					this._localDirectories = this._directories
												  .LocalDirectories
												  .Cast<DirectoryElement>()
												  .ToDictionary(p => p.Key, p => p.Value);
				return this._localDirectories;
			}
		}
		private Dictionary<string, string> _localDirectories;

		/// <summary>
		/// Gets the list of remote directories for SCP synchronization.
		/// </summary>
		public Dictionary<string, string> ScpRemoteDirectories
		{
			get
			{
				if (this._remoteDirectories == null || !this._remoteDirectories.Any())
					this._remoteDirectories = this._directories
												  .RemoteDirectories
												  .Cast<DirectoryElement>()
												  .ToDictionary(p => p.Key, p => p.Value);
				return this._remoteDirectories;
			}
		}
		private Dictionary<string, string> _remoteDirectories;

		/// <summary>
		/// Gets the executable path of WinSCP.
		/// </summary>
		public string ScpExecutablePath
		{
			get
			{
				var path = ConfigurationManager.AppSettings["Scp.ExecutablePath"];
				if (String.IsNullOrEmpty(path))
				{
					path = @"C:\Program Files (x86)\WinSCP";
					if (!Directory.Exists(path))
						path = @"C:\Program Files\WinSCP";
				}
				if (!path.ToLower().EndsWith("winscp.exe"))
					path = String.Format(@"{0}\WinSCP.exe", path.TrimEnd('/', '\\'));
				return path;
			}
		}

		/// <summary>
		/// Gets the SCP synchronization mode.
		/// </summary>
		public SynchronizationMode ScpSynchronizationMode
		{
			get
			{
				SynchronizationMode result;
				var value = Enum.TryParse(ConfigurationManager.AppSettings["Scp.SynchronizationMode"], true, out result)
								? result
								: SynchronizationMode.Both;
				return value;
			}
		}

		/// <summary>
		/// Gets the value that specifies whether to remove files in the target for SCP synchronization.
		/// </summary>
		public bool ScpRemoveFiles
		{
			get
			{
				bool result;
				var value = Boolean.TryParse(ConfigurationManager.AppSettings["Scp.RemoveFiles"], out result) && result;
				return value;
			}
		}

		/// <summary>
		/// Gets the value that specifies whether to synchronize older files or not for SCP synchronization.
		/// </summary>
		public bool ScpMirrorMode
		{
			get
			{
				bool result;
				var value = Boolean.TryParse(ConfigurationManager.AppSettings["Scp.MirrorMode"], out result) && result;
				return value;
			}
		}

		/// <summary>
		/// Gets the synchronization criteria for SCP synchronization.
		/// </summary>
		public SynchronizationCriteria ScpSynchronizationCriteria
		{
			get
			{
				SynchronizationCriteria result;
				var value = Enum.TryParse(ConfigurationManager.AppSettings["Scp.SynchronizationCriteria"], true, out result)
					            ? result
					            : SynchronizationCriteria.Time;
				return value;
			}
		}

		/// <summary>
		/// Gets the transfer options for SCP synchronization.
		/// </summary>
		public TransferOptions ScpTransferOptions
		{
			get
			{
				//	NOTE: It will always returns NULL.
				//	Furder implementation will be done in the later stage.
				//var value = ConfigurationManager.AppSettings["Scp.TransferOptions"];
				return null;
			}
		}
		#endregion



		#region For Git
		/// <summary>
		/// Gets the executable path of Git.
		/// </summary>
		public string GitExecutablePath
		{
			get
			{
				var path = ConfigurationManager.AppSettings["Git.ExecutablePath"];
				if (String.IsNullOrEmpty(path))
				{
					path = @"C:\Program Files (x86)\Git\bin";
					if (!Directory.Exists(path))
						path = @"C:\Program Files\Git\bin";
				}
				if (!path.ToLower().EndsWith("git.exe"))
					path = String.Format(@"{0}\git.exe", path.TrimEnd('/', '\\'));
				return path;
			}
		}

		/// <summary>
		/// Gets the Git local repository path.
		/// </summary>
		public string GitLocalRepositoryPath
		{
			get
			{
				var path = ConfigurationManager.AppSettings["Git.LocalRepositoryPath"];
				if (!Directory.Exists(path))
					path = String.Empty;
				return path.TrimEnd('/', '\\');
			}
		}

		/// <summary>
		/// Gets the branch name for Git. Default value is "HEAD".
		/// </summary>
		public string GitBranchName
		{
			get
			{
				var value = ConfigurationManager.AppSettings["Git.BranchName"];
				if (String.IsNullOrEmpty(value))
					value = "HEAD";
				return value;
			}
		}

		/// <summary>
		/// Sets the regular expression pattern to add files. Default value is ".", ie. all files.
		/// </summary>
		public string GitPatternForAddition
		{
			get
			{
				var value = ConfigurationManager.AppSettings["Git.PatternForAddition"];
				if (String.IsNullOrEmpty(value))
					value = ".";
				return value;
			}
		}

		/// <summary>
		/// Gets the automated commit message. Default value is "Committed by AlienSync for Git".
		/// </summary>
		public string GitCommitMessage
		{
			get
			{
				var value = ConfigurationManager.AppSettings["Git.CommitMessage"];
				if (String.IsNullOrEmpty(value))
					value = "Committed by AlienSync for Git";
				return value;
			}
		}

		#endregion



		#region For Hg
		
		#endregion

		#region For MS-SQL
		/// <summary>
		/// Gets the connection details for MS-SQL source database.
		/// </summary>
		public Dictionary<string, string> MsSqlSourceConnection
		{
			get { return this.GetMsSqlConnection("MsSqlSourceConnection"); }
		}

		/// <summary>
		/// Gets the connection details for MS-SQL destination database.
		/// </summary>
		public Dictionary<string, string> MsSqlDestinationConnection
		{
			get { return this.GetMsSqlConnection("MsSqlDestinationConnection"); }
		}

		/// <summary>
		/// Gets the executable path of MS-SQL SQLCMD.exe.
		/// </summary>
		public string MsSqlCommandExecutablePath
		{
			get
			{
				var path = ConfigurationManager.AppSettings["MsSql.CommandExecutablePath"];
				if (String.IsNullOrEmpty(path))
				{
					path = @"C:\Program Files\Microsoft SQL Server\110\Tools\Binn";
					if (!Directory.Exists(path))
						path = @"C:\Program Files\Microsoft SQL Server\100\Tools\Binn";
				}
				if (!path.ToLower().EndsWith("sqlcmd.exe"))
					path = String.Format(@"{0}\sqlcmd.exe", path.TrimEnd('/', '\\'));
				return path;
			}
		}

		/// <summary>
		/// Gets the executable path of TableDiff.exe.
		/// </summary>
		public string MsSqlTableDiffExecutablePath
		{
			get
			{
				var path = ConfigurationManager.AppSettings["MsSql.TableDiffExecutablePath"];
				if (String.IsNullOrEmpty(path))
				{
					//	Sets the version of MS-SQL Server 2012.
					path = @"C:\Program Files\Microsoft SQL Server\110\COM";
					//	Sets the version of MS-SQL Server 2008.
					if (!Directory.Exists(path))
						path = @"C:\Program Files\Microsoft SQL Server\100\COM";
				}
				if (!path.ToLower().EndsWith("tablediff.exe"))
					path = String.Format(@"{0}\tablediff.exe", path.TrimEnd('/', '\\'));
				return path;
			}
		}

		/// <summary>
		/// Gets the storage path to store TableDiff results.
		/// </summary>
		public string MsSqlScriptStoragePath
		{
			get
			{
				var path = ConfigurationManager.AppSettings["MsSql.ScriptStoragePath"];
				if (String.IsNullOrEmpty(path))
					path = @"\results";
				var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				if (!String.IsNullOrEmpty(directory))
					path = String.Format("{0}{1}", directory.TrimEnd('/', '\\'), path.TrimEnd('/', '\\'));
				return path;
			}
		}

		#endregion

		#endregion

		#region Methods

		/// <summary>
		/// Gets the separator using the list of separators with the given length.
		/// </summary>
		/// <param name="separators">List of separators. Default separator is "-".</param>
		/// <param name="length">Length. Default value is 80.</param>
		/// <returns></returns>
		public string GetSeparator(List<string> separators = null, int length = 80)
		{
			if (separators == null || !separators.Any())
				separators = new List<string>() {"-"};

			if (length <= 0)
				length = 80;

			var value = String.Empty;
			for (var i = 0; i < length; i++)
				foreach (var s in separators)
					value += s;
			return value;
		}

		/// <summary>
		/// Gets the collection of the connection details for MS-SQL database.
		/// </summary>
		/// <param name="name">Connectionstring key.</param>
		/// <returns>Returns the collection of the connection etails for MS-SQL database.</returns>
		private Dictionary<string, string> GetMsSqlConnection(string name)
		{
			var connection = ConfigurationManager.AppSettings[name];
			if (String.IsNullOrEmpty(connection))
				throw new InvalidConfigurationException("A valid connection string doesn't exist.");

			var collection = connection.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
									   .ToDictionary(p => p.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(" ", ""),
													 p => p.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[1]);
			return collection;
		}
		#endregion
	}
}
