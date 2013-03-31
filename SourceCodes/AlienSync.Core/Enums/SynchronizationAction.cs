namespace AlienSync.Core.Enums
{
	/// <summary>
	/// This specifies a synchronization related action.
	/// </summary>
	public enum SynchronizationAction
	{
		/// <summary>
		/// Indicates that no action is identified.
		/// </summary>
		None = 0,

		/// <summary>
		/// Indicates that only WinSCP will be used for the synchronization.
		/// </summary>
		ScpOnly = 1,

		/// <summary>
		/// Indicates that WinSCP followed by Git will be used for the synchronization.
		/// </summary>
		ScpThenGit = 2,

		/// <summary>
		/// Indicates that WinSCP followed by Hg will be used for the synchronization.
		/// </summary>
		ScpThenHg = 3,

		/// <summary>
		/// Indicates that only MS-SQL database will be used for the synchronization.
		/// </summary>
		MsSqlOnly = 4
	}
}
