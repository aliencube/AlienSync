using System.Configuration;

namespace AlienSync.Core.Configuration
{
	/// <summary>
	/// This represents the directory settings configuraiton entity.
	/// </summary>
	public class DirectoryConfiguration : ConfigurationSection
	{
		/// <summary>
		/// Gets or sets the collection of the local directories.
		/// </summary>
		[ConfigurationProperty("localDirectories")]
		[ConfigurationCollection(typeof(DirectoryElementCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
		public DirectoryElementCollection LocalDirectories
		{
			get { return (DirectoryElementCollection)this["localDirectories"]; }
			set { this["localDirectories"] = value; }
		}

		/// <summary>
		/// Gets or sets the collection of the remote directories.
		/// </summary>
		[ConfigurationProperty("remoteDirectories")]
		[ConfigurationCollection(typeof(DirectoryElementCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
		public DirectoryElementCollection RemoteDirectories
		{
			get { return (DirectoryElementCollection)this["remoteDirectories"]; }
			set { this["remoteDirectories"] = value; }
		}
	}
}
