using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.IO;

namespace N2.Templates.Web
{
	public class ThemeModifier : IPageModifier
	{
		bool themeVerified = false;

		public void Modify<T>(UI.TemplatePage<T> page) 
			where T : Items.AbstractPage
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
