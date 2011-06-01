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
	public class IndexableAttribute : Attribute, IIndexableProperty, IEditable
	{
		#region IIndexableProperty Members

		public bool Index
		{
			get { return true; }
		}

		public string GetIndexableText(ContentItem item, string name)
		{
			object value = item[name];
			if (value == null)
				return null;
			
			return value.ToString();
		}

		#endregion

		#region IEditable Members

		string IEditable.Title { get; set; }

		bool IEditable.UpdateItem(ContentItem item, System.Web.UI.Control editor)
		{
			return false;
		}

		void IEditable.UpdateEditor(ContentItem item, System.Web.UI.Control editor)
		{
		}

		#endregion

		#region IContainable Members

		string IContainable.ContainerName { get; set; }

		int IContainable.SortOrder { get; set; }

		System.Web.UI.Control IContainable.AddTo(System.Web.UI.Control container)
		{
			var ph = new PlaceHolder();
			container.Controls.Add(ph);
			return ph;
		}

		#endregion

		#region IUniquelyNamed Members

		string IUniquelyNamed.Name { get; set; }

		#endregion

		#region IComparable<IContainable> Members

		int IComparable<IContainable>.CompareTo(IContainable other)
		{
			return 0;
		}

		#endregion

		#region IComparable<IEditable> Members

		int IComparable<IEditable>.CompareTo(IEditable other)
		{
			return 0;
		}

		#endregion
	}
}
