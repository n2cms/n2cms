using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.IO;
using N2.Web.UI;

namespace N2.Templates.Web
{
    /// <summary>
    /// Sets the theme of the page template.
    /// </summary>
	public class ThemeModifier : IPageModifier
	{
		bool themeVerified = false;

		public void Modify<T>(ContentPage<T> page) where T : ContentItem
		{
			string theme = Find.StartPage.Theme;
			if (!themeVerified && theme != null)
			{
				if (Directory.Exists(page.Server.MapPath("~/App_Themes/" + theme)))
				{
					themeVerified = true;
				}
			}

			if (themeVerified)
			{
				page.Theme = theme;
			}
		}
	}
}
