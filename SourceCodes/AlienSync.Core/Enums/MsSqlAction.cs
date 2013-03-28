namespace AlienSync.Core.Enums
{
	/// <summary>
	/// This specifies a MS-SQL synchronization related action.
	/// </summary>
	public enum MsSqlAction
	{
		/// <summary>
		/// Indicates that no action is identified.
		/// </summary>
		None = 0,

		/// <summary>
		/// Indicates that "Clean up directory" action is identified.
		/// </summary>
		CleanUpDirectory = 1,

		/// <summary>
		/// Indicates that "Get tables" action is identified.
		/// </summary>
		GetTables = 2,

		/// <summary>
		/// Indicates that "Generate script" action is identified.
		/// </summary>
		GenerateScripts = 3,

		/// <summary>
		/// Indicates that "Cleanse script" action is identified.
		/// </summary>
		CleanseScripts = 4,

		/// <summary>
		/// Indicates that "Apply differences" action is identified.
		/// </summary>
		ApplyDifferences = 5
	}
}
