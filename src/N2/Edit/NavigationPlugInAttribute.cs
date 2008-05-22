using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace N2.Edit
{
	/// <summary>
	/// An attribute defining a right-click item in the navigation pane.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
	public class NavigationPluginAttribute : LinkPluginAttribute
	{
		#region Constructors
		public NavigationPluginAttribute()
		{
		}

		public NavigationPluginAttribute(string title, string name, string urlFormat)
		{
			this.Title = title;
			this.Name = name;
			this.UrlFormat = urlFormat;
		}
		public NavigationPluginAttribute(string title, string name, string urlFormat, string target, string iconUrl, int sortOrder)
			: this(title, name, urlFormat)
		{
			this.Target = target;
			this.IconUrl = iconUrl;
			this.SortOrder = sortOrder;
		} 
		#endregion 

		protected override string ArrayVariableName
		{
			get { return "navigationPlugIns"; }
		}
	}
}
