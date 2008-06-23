using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Edit
{
    public class NavigationLinkPluginAttribute : NavigationPluginAttribute
    {
		#region Constructors
		public NavigationLinkPluginAttribute()
		{
		}

		public NavigationLinkPluginAttribute(string title, string name, string urlFormat)
		{
			this.Title = title;
			this.Name = name;
			this.UrlFormat = urlFormat;
		}
        public NavigationLinkPluginAttribute(string title, string name, string urlFormat, string target, string iconUrl, int sortOrder)
			: this(title, name, urlFormat)
		{
			this.Target = target;
			this.IconUrl = iconUrl;
			this.SortOrder = sortOrder;
		} 
		#endregion 
    }
}
