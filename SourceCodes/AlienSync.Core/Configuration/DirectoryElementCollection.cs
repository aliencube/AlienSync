using System.Configuration;

namespace AlienSync.Core.Configuration
{
	/// <summary>
	/// This represents the directory collection entity.
	/// </summary>
	public class DirectoryElementCollection : ConfigurationElementCollection
	{
		#region Properties
		/// <summary>
		/// Gets the type of the ConfigurationElementCollection.
		/// </summary>
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.AddRemoveClearMap;
			}
		}

		/// <summary>
		/// Gets or sets the directory element at the specified index location.
		/// </summary>
		/// <param name="index">The index location of the directory element to remove.</param>
		/// <returns>Returns the directory element at the specified index location.</returns>
		public DirectoryElement this[int index]
		{
			get
			{
				return (DirectoryElement)BaseGet(index);
			}
			set
			{
				if (BaseGet(index) != null)
					BaseRemoveAt(index);
				BaseAdd(index, value);
			}
		}

		/// <summary>
		/// Gets or sets the directory element having the specified key.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <returns>Returns the directory element having the specified key.</returns>
		public DirectoryElement this[string key]
		{
			get
			{
				return (DirectoryElement)BaseGet(key);
			}
			set
			{
				var item = (DirectoryElement)BaseGet(key);
				if (item != null)
				{
					var index = BaseIndexOf(item);
					BaseRemoveAt(index);
					BaseAdd(index, value);
				}
				BaseAdd(value);
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates a new ConfigurationElement.
		/// </summary>
		/// <returns>Returns a new ConfigurationElement.</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new DirectoryElement();
		}

		/// <summary>
		/// Gets the element key for a specified configuration element.
		/// </summary>
		/// <param name="element">ConfigurationElement to return for.</param>
		/// <returns>Returns an Object that acts as the key for the specified ConfigurationElement.</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((DirectoryElement)element).Key;
		}

		/// <summary>
		/// Adds a directory element to the ConfigurationElementCollection.
		/// </summary>
		/// <param name="element">Directory element.</param>
		public void Add(DirectoryElement element)
		{
			BaseAdd(element);
		}

		/// <summary>
		/// Removes all directory element objects from the collection.
		/// </summary>
		public void Clear()
		{
			BaseClear();
		}

		/// <summary>
		/// Removes a directory element from the collection.
		/// </summary>
		/// <param name="key">Directory element key.</param>
		public void Remove(string key)
		{
			BaseRemove(key);
		}

		/// <summary>
		/// Removes the directory element at the specified index location.
		/// </summary>
		/// <param name="index">The index location of the directory element to remove.</param>
		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}
		#endregion
	}
}