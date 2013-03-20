using System;
using System.Collections.Generic;
using System.Linq;
using WinSCP;
using AlienSync.Core.Exceptions;

namespace AlienSync.Core
{
	/// <summary>
	/// This represents the parameters entity for the WinSCP session.
	/// </summary>
	public class Parameters
	{
		#region Constructors
		/// <summary>
		/// Initialises a new instance of the Parameters object.
		/// </summary>
		/// <param name="args">List of parameters manually set.</param>
		public Parameters(string[] args = null)
		{
			this.SessionOptions = GetSessionOptions(args);
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the session options.
		/// </summary>
		public SessionOptions SessionOptions { get; private set; }
		#endregion

		#region Methods
		/// <summary>
		/// Gets the session options.
		/// </summary>
		/// <param name="args">List of parameters manually set.</param>
		/// <returns>Returns the session options.</returns>
		private static SessionOptions GetSessionOptions(string[] args = null)
		{
			SessionOptions options;
			if (args != null && args.Any())
				options = GetSessionOptionsFromArgs(args);
			else
				options = GetSessionOptionsFromConfig();
			return options;
		}

		/// <summary>
		/// Gets the session options from the list of parameters manually set.
		/// </summary>
		/// <param name="args">List of parameters manually set.</param>
		/// <exception cref="NotImplementedException">Thrown when this method is not yet implemented.</exception>
		/// <returns>Returns the session options.</returns>
		/// <remarks>NOTE: Setting options with arguments will be implemented in the later stage.</remarks>
		private static SessionOptions GetSessionOptionsFromArgs(string[] args)
		{
			//	NOTE: Setting options with arguments will be implemented in the later stage.
			throw new NotImplementedException("Arguments are not considered to setup options.");
		}

		/// <summary>
		/// Gets the session options from app.config.
		/// </summary>
		/// <returns>Returns the session options.</returns>
		private static SessionOptions GetSessionOptionsFromConfig()
		{
			var connection = ConvertConnectionString();
			var options = new SessionOptions() {HostName = connection["hostname"]};

			Protocol protocol;
			options.Protocol = connection.ContainsKey("protocol") && Enum.TryParse(connection["protocol"], true, out protocol) ? protocol : Protocol.Sftp;

			FtpMode ftpMode;
			options.FtpMode = connection.ContainsKey("ftpmode") && Enum.TryParse(connection["ftpmode"], true, out ftpMode) ? ftpMode : FtpMode.Passive;

			FtpSecure ftpSecure;
			options.FtpSecure = connection.ContainsKey("ftpsecure") && Enum.TryParse(connection["ftpsecure"], true, out ftpSecure) ? ftpSecure : FtpSecure.None;

			int portNumber;
			options.PortNumber = connection.ContainsKey("portnumber") && Int32.TryParse(connection["portnumber"], out portNumber) ? portNumber : 0;

			options.UserName = connection.ContainsKey("username") ? connection["username"] : null;
			options.Password = connection.ContainsKey("password") ? connection["password"] : null;
			options.SshHostKeyFingerprint = connection.ContainsKey("sshhostkeyfingerprint") ? connection["sshhostkeyfingerprint"] : null;
			options.SshPrivateKeyPath = connection.ContainsKey("sshprivatekeypath") ? connection["sshprivatekeypath"] : null;
			options.SslHostCertificateFingerprint = connection.ContainsKey("sslhostcertificatefingerprint") ? connection["sslhostcertificatefingerprint"] : null;

			double timeout;
			options.Timeout = TimeSpan.FromSeconds(connection.ContainsKey("timeout") && Double.TryParse(connection["timeout"], out timeout) ? timeout : 15);

			return options;
		}

		/// <summary>
		/// Converts the connection string to the dictionary collection.
		/// </summary>
		/// <exception cref="InvalidConfigurationException">Thrown when no connection string is defined.</exception>
		/// <returns>Returns the connection string converted to the dictionary collecton.</returns>
		private static Dictionary<string, string> ConvertConnectionString()
		{
			var connection = String.Empty;
			if (!String.IsNullOrEmpty(Settings.Instance.ConnectionStrings["SftpConnection"]))
				connection = Settings.Instance.ConnectionStrings["SftpConnection"];
			else if (!String.IsNullOrEmpty(Settings.Instance.ConnectionStrings["ScpConnection"]))
				connection = Settings.Instance.ConnectionStrings["ScpConnection"];
			else if (!String.IsNullOrEmpty(Settings.Instance.ConnectionStrings["FtpConnection"]))
				connection = Settings.Instance.ConnectionStrings["FtpConnection"];

			if (String.IsNullOrEmpty(connection))
				throw new InvalidConfigurationException("A valid connection string doesn't exist.");

			var collection = connection.Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries)
			                           .ToDictionary(p => p.Split(new string[] {"="}, StringSplitOptions.RemoveEmptyEntries)[0].ToLower(),
			                                         p => p.Split(new string[] {"="}, StringSplitOptions.RemoveEmptyEntries)[1]);
			return collection;
		}

		#endregion
	}
}
