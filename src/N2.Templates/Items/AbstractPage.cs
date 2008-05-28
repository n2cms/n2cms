using System.Collections.Generic;
using System.Web.UI;
using N2.Collections;
using N2.Details;
using N2.Web.UI;

namespace N2.Templates.Items
{
	/// <summary>
	/// A base class for page items in the template project. Provides automatic
	/// wiring to the default location of the aspx template and access to parts
	/// added to recursive zones of a parent.
	/// </summary>
	[WithEditableTitle("Title", 10, Focus = true, ContainerName = Tabs.Content)]
	[TabPanel(Tabs.Content, "Content", 0)]
	public abstract class AbstractPage : ContentItem
	{
		private static readonly int startIndex = "N2.Templates.".Length;
		
		public override string TemplateUrl
		{
			get
			{
				string url = GetType().FullName.Substring(startIndex);
				url = url.Replace("Items.", "UI/").Replace('.', '/');
				return "~/" + url + ".aspx";
			}
		}

		[EditableCheckBox("Show Title", 60, ContainerName = Tabs.Advanced)]
		public virtual bool ShowTitle
		{
			get { return (bool)(GetDetail("ShowTitle") ?? true); }
			set { SetDetail("ShowTitle", value, true); }
		}

		public override ItemList GetChildren(string childZoneName)
		{
			if (VersionOf != null)
				return VersionOf.GetChildren(childZoneName);

			ItemList items = base.GetChildren(childZoneName);
			if (childZoneName.StartsWith("Recursive") && Parent is AbstractContentPage)
			{
				items.AddRange(Parent.GetChildren(childZoneName));
			}
			return items;
		}

		public virtual IList<T> GetChildren<T>() where T : ContentItem
		{
			return new ItemList<T>(Children,
			                       new AccessFilter(),
			                       new TypeFilter(typeof(T)));
		}

		public virtual IList<T> GetChildren<T>(string zoneName) where T : ContentItem
		{
			return new ItemList<T>(Children,
			                       new AccessFilter(),
			                       new TypeFilter(typeof(T)),
			                       new ZoneFilter(zoneName));
		}
	}
}