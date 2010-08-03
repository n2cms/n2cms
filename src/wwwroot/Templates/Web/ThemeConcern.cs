using System.Web.Hosting;
using System.IO;
using N2.Web.UI;
using N2.Templates.Services;
using N2.Templates.Items;
using N2.Templates.Web.UI;
using N2.Engine;

namespace N2.Templates.Web
{
    /// <summary>
    /// Sets the theme of the page template.
    /// </summary>
	[Service(typeof(TemplateConcern))]
	public class ThemeConcern : TemplateConcern
	{
		public override void OnPreInit(ITemplatePage template)
		{
			var item = template.CurrentItem;
			if (item == null)
				return;

			var startPage = Find.Closest<StartPage>(item) ?? Find.ClosestStartPage;
			if (startPage == null)
				return;

			string theme = startPage.Theme;
			
			var exists = template.Page.Cache["ThemeModifier." + theme];
			if (exists == null)
			{
				exists = Directory.Exists(HostingEnvironment.MapPath("~/App_Themes/" + theme));
				template.Page.Cache["ThemeModifier." + theme] = exists;
			}

			if ((bool)exists)
			{
				template.Page.Theme = theme;
			}
		}
	}
}
