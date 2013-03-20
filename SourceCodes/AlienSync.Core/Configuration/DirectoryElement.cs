using System.Configuration;

namespace AlienSync.Core.Configuration
{
	/// <summary>
	/// This represents the directory element entity.
	/// </summary>
	public class DirectoryElement : ConfigurationElement
	{
		/// <summary>
		/// Gets or sets the element key.
		/// </summary>
		[ConfigurationProperty("key", DefaultValue = "", IsRequired = true)]
		[StringValidator(InvalidCharacters = "`~!@#$%^&*()-=_+[]{}\\|;:'\",.<>/?", MinLength = 0, MaxLength = 64)]
		public string Key
		{
			get { return (string) this["key"]; }
			set { this["key"] = value; }
		}

		/// <summary>
		/// Gets or sets the element value.
		/// </summary>
		[ConfigurationProperty("value", DefaultValue = "", IsRequired = true)]
		[StringValidator(InvalidCharacters = "<>\"|?*", MinLength = 0, MaxLength = 256)]
		public string Value
		{
			get { return (string)this["value"]; }
			set { this["value"] = value; }
		}
	}
}