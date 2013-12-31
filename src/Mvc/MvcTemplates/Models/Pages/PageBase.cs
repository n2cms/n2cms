using System;
using System.Collections.Generic;
using N2.Collections;
using N2.Details;
using N2.Web.UI;

namespace N2.Templates.Mvc.Models.Pages
{
    /// <summary>
    /// A base class for page items in the template project. Provides automatic
    /// wiring to the default location of the aspx template and access to parts
    /// added to recursive zones of a parent.
    /// </summary>
    [WithEditableTitle("Title", 4, Focus = true, ContainerName = Tabs.Content)]
    [TabContainer(Tabs.Content, "Content", 0)]
    [TabContainer(Tabs.Advanced, "Advanced", 100)]
    [SidebarContainer(Tabs.Details, 6, HeadingText = "Details")]
    public abstract class PageBase : ContentItem
    {
        // editables

        [EditableCheckBox("Show Title", 10, ContainerName = Tabs.Details, DefaultValue = true)]
        public virtual bool ShowTitle { get; set; }

        // helpers

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
    }

    [Obsolete("Use PageBase and [PageDefinition]")]
    public abstract class AbstractPage : PageBase, IItemContainer
    {
        /// <summary>Gets the item associated with the item container.</summary>
        public ContentItem CurrentItem
        {
            get { return this; }
        }
    }
}
