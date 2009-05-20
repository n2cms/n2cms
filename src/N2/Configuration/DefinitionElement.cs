using System.Configuration;

namespace N2.Configuration
{
	/// <summary>
	/// A configured definition of a content item.
	/// </summary>
	public class DefinitionElement : NamedElement
	{
		/// <summary>The type of item.</summary>
		[ConfigurationProperty("type", IsRequired = true)]
		public string Type
		{
			get { return (string)base["type"]; }
			set { base["type"] = value; }
		}

		/// <summary>The title used to describe the type.</summary>
		[ConfigurationProperty("title")]
		public string Title
		{
			get { return (string)base["title"]; }
			set { base["title"] = value; }
		}

		/// <summary>The sort of order of the item.</summary>
		[ConfigurationProperty("sortOrder", DefaultValue = 1000)]
		public int SortOrder
		{
			get { return (int)base["sortOrder"]; }
			set { base["sortOrder"] = value; }
		}

		/// <summary>A tool tip available when adding items.</summary>
		[ConfigurationProperty("toolTip")]
		public string ToolTip
		{
			get { return (string)base["toolTip"]; }
			set { base["toolTip"] = value; }
		}

		/// <summary>A description showed when adding items.</summary>
		[ConfigurationProperty("description")]
		public string Description
		{
			get { return (string)base["description"]; }
			set { base["description"] = value; }
		}
	}
}