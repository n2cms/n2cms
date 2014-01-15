using System.Collections.Generic;
using N2.Collections;
using N2.Details;
using N2.Web;
using N2.Web.UI;
using System;
using N2.Definitions;

namespace N2.Templates.Items
{
    /// <summary>
    /// A base class for page items in the template project. Provides automatic
    /// wiring to the default location of the aspx template and access to parts
    /// added to recursive zones of a parent.
    /// </summary>
    [WithEditableTitle("Title", 10, Focus = true, ContainerName = Tabs.Content)]
    [TabContainer(Tabs.Content, "Content", Tabs.ContentIndex)]
    [TabContainer(Tabs.Advanced, "Advanced", Tabs.AdvancedIndex)]
    [Template("create", "{ManagementUrl}/Content/New.aspx")]
    [Template("update", "{ManagementUrl}/Content/Edit.aspx")]
    [Template("delete", "{ManagementUrl}/Content/delete.aspx")]
    public abstract class AbstractPage : ContentItem, IPage
    {
        /// <summary>The name without extension .png of an icon file located in /Templates/UI/Img/. Defaults to "page".</summary>
        [Obsolete("No longer useful, sorry.")]
        protected virtual string IconName
        {
            get { return "page"; }
        }

        [EditableCheckBox("Show Title", 60, ContainerName = Tabs.Advanced)]
        public virtual bool ShowTitle
        {
            get { return (bool)(GetDetail("ShowTitle") ?? true); }
            set { SetDetail("ShowTitle", value, true); }
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
