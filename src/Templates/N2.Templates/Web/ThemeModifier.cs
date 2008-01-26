using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace N2.Templates.Web
{
	public class ThemeModifier : IPageModifier
	{
		public void Modify<T>(UI.TemplatePage<T> page) 
			where T : Items.AbstractPage
		{
			page.Theme = Find.StartPage.Theme; //TODO: Create component
		}
	}
}
