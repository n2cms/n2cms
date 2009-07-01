using System;
using System.Configuration;
using System.Web.UI;
using N2.Templates.Mvc.Configuration;

namespace N2.Templates.Mvc.Web
{
	/// <summary>
	/// Applies the template defined in the n2/templates configuration section 
	/// to the page.
	/// </summary>
	public class MasterPageModifier : IPageModifier
	{
		private readonly TemplatesSection config;

		public MasterPageModifier()
		{
			config = ConfigurationManager.GetSection("n2/templates") as TemplatesSection;
		}

		#region IPageModifier Members

		public void Modify(Page page)
		{
			return;

			// Not implemented as still not found a way to hook into the MVC pipeline
			if (config != null && !string.IsNullOrEmpty(config.MasterPageFile))
			{
				page.MasterPageFile = config.MasterPageFile;
			}
		}

		#endregion
	}
}