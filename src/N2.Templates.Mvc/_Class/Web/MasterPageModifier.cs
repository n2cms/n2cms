using System.IO;
using N2.Templates.Configuration;
using System.Configuration;
using N2.Web.UI;

namespace N2.Templates.Mvc.Web
{
	/// <summary>
	/// Applies the template defined in the n2/templates configuration section 
	/// to the page.
	/// </summary>
	public class MasterPageModifier : IPageModifier
	{
		TemplatesSection config;

		public MasterPageModifier()
		{
			config = ConfigurationManager.GetSection("n2/templates") as TemplatesSection;
		}

		public void Modify<T>(ContentPage<T> page) where T : ContentItem
		{
			if (config != null && !string.IsNullOrEmpty(config.MasterPageFile))
			{
				page.MasterPageFile = config.MasterPageFile;
			}
		}
	}
}