using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Edit;

namespace N2.Edit
{
	/// <summary>
	/// An attribute defining a right-click item in the navigation pane.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
	[Obsolete("Namechange to NavigationPluginAttribute")]
	public class NavigationPlugInAttribute : NavigationPluginAttribute
	{
		[Obsolete]
		public NavigationPlugInAttribute()
			: base()
		{
		}

		[Obsolete]
		public NavigationPlugInAttribute(string title, string name, string urlFormat)
			: base(title, name, urlFormat)
		{
		}

		[Obsolete]
		public NavigationPlugInAttribute(string title, string name, string urlFormat, string target, string iconUrl, int sortOrder)
			: base(title, name, urlFormat, target, iconUrl, sortOrder)
		{
		} 
	}
}
