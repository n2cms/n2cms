using System.Web.Hosting;
using System.IO;
using N2.Templates.Mvc.Models.Pages;
using N2.Web.UI;
using N2.Engine;

namespace N2.Templates.Mvc.Services.WebForms
{
    /// <summary>
    /// Sets the theme of the page template.
    /// </summary>
	[Service(typeof(ContentPageConcern))]
	public class ThemeConcern : ContentPageConcern
	{
		public override void OnPreInit(System.Web.UI.Page page, ContentItem item)
		{
			if (item == null)
				return;

			var startPage = N2.Find.Closest<StartPage>(item) ?? Find.ClosestStartPage;
			if (startPage == null)
				return;

			string theme = startPage.Theme;
			
			var exists = page.Cache["ThemeModifier." + theme];
			if (exists == null)
			{
				exists = Directory.Exists(HostingEnvironment.MapPath("~/App_Themes/" + theme));
				page.Cache["ThemeModifier." + theme] = exists;
			}

			if ((bool)exists)
			{
				page.Theme = theme;
			}
		}
	}
}
