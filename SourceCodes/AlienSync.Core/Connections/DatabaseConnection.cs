namespace AlienSync.Core.Connections
{
	/// <summary>
	/// This represents the database connection entity derived from connection string.
	/// </summary>
	public class DatabaseConnection
	{
		/// <summary>
		/// Gets or sets the database server.
		/// </summary>
		public string DataSource { get; set; }

		/// <summary>
		/// Gets or sets the database name.
		/// </summary>
		public string InitialCatalog { get; set; }

		/// <summary>
		/// Gets or sets the user Id of the database.
		/// </summary>
		public string UserId { get; set; }

		/// <summary>
		/// Gets or sets the password of the database.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the value that specifies whether to integrated Windows account or not.
		/// </summary>
		public bool IntegratedSecurity { get; set; }
	}
}
