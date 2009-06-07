using System;
using System.Collections.Generic;
using N2.Collections;
using N2.Details;
using N2.Web.UI;

namespace N2.Templates.Mvc.Items
{
	/// <summary>
	/// A base class for page items in the template project. Provides automatic
	/// wiring to the default location of the aspx template and access to parts
	/// added to recursive zones of a parent.
	/// </summary>
	[WithEditableTitle("Title", 10, Focus = true, ContainerName = Tabs.Content)]
	[TabContainer(Tabs.Content, "Content", 0)]
	[TabContainer(Tabs.Advanced, "Advanced", 100)]
	public abstract class AbstractPage : ContentItem, IItemContainer
	{
		[EditableCheckBox("Show Title", 60, ContainerName = Tabs.Advanced)]
		public virtual bool ShowTitle
		{
			get { return (bool) (GetDetail("ShowTitle") ?? true); }
			set { SetDetail("ShowTitle", value, true); }
		}

		public virtual IList<T> GetChildren<T>() where T : ContentItem
		{
			return new ItemList<T>(Children,
			                       new AccessFilter(),
			                       new TypeFilter(typeof (T)));
		}

		public virtual IList<T> GetChildren<T>(string zoneName) where T : ContentItem
		{
			return new ItemList<T>(Children,
			                       new AccessFilter(),
			                       new TypeFilter(typeof (T)),
			                       new ZoneFilter(zoneName));
		}

		public override string TemplateUrl
		{
			get { return "~/Views/" + GetType().Name + "/Index.aspx"; }
		}

		/// <summary>Gets the item associated with the item container.</summary>
		public ContentItem CurrentItem
		{
			get { return this; }
		}
	}
}