using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using System.Web.UI.WebControls;

namespace N2.Persistence.Search
{
	/// <summary>
	/// A property that should be indexed by the search service.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class IndexableAttribute : Attribute, IIndexableProperty, IUniquelyNamed
	{
		public string Name { get; set; }

		#region IIndexableProperty Members

		public bool Index
		{
			get { return true; }
		}

		public string GetIndexableText(ContentItem item)
		{
			object value = item[Name];
			if (value == null)
				return null;
			
			return value.ToString();
		}

		#endregion
	}
}
