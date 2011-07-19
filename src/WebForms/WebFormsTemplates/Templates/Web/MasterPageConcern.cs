using System.IO;
using System.Linq;
using System.Configuration;
using N2.Templates.Configuration;
using N2.Web.UI;
using N2.Templates.Services;
using N2.Templates.Web.UI;
using N2.Engine;
using N2.Configuration;

namespace N2.Templates.Web
{
	/// <summary>
	/// Applies the template defined in the n2/templates configuration section 
	/// to the page.
	/// </summary>
	[Service(typeof(ContentPageConcern))]
	public class MasterPageConcern : ContentPageConcern
	{
		string masterPageFile;

		public MasterPageConcern(ConfigurationManagerWrapper configuration)
		{
			var section = configuration.GetContentSection<TemplatesSection>("templates");
			if (section != null)
				masterPageFile = section.MasterPageFile;
		}

		public override void OnPreInit(System.Web.UI.Page page, ContentItem item)
		{
			if (!string.IsNullOrEmpty(masterPageFile) && !string.IsNullOrEmpty(page.MasterPageFile))
				page.MasterPageFile = masterPageFile;
		}
	}
}