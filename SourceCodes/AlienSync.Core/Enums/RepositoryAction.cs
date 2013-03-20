namespace AlienSync.Core.Enums
{
	/// <summary>
	/// This specifies a repository related action.
	/// </summary>
	public enum RepositoryAction
	{
		/// <summary>
		/// Indicates that no action is identified.
		/// </summary>
		None = 0,

		/// <summary>
		/// Indicates that "PULL" action is identified.
		/// </summary>
		Pull = 1,

		/// <summary>
		/// Indicates that "ADD" action is identified.
		/// </summary>
		Add = 2,

		/// <summary>
		/// Indicates that "COMMIT" action is identified.
		/// </summary>
		Commit = 3,

		/// <summary>
		/// Indicates that "PUSH" action is identified.
		/// </summary>
		Push = 4
	}
}
