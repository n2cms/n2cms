using System.IO;
using N2.Templates.Configuration;
using System.Configuration;
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
	[Service(typeof(TemplateConcern))]
	public class MasterPageConcern : TemplateConcern
	{
		string masterPageFile;

		public MasterPageConcern(ConfigurationManagerWrapper configuration)
		{
			var section = configuration.GetContentSection<TemplatesSection>("templates");
			if (section != null)
				masterPageFile = section.MasterPageFile;
		}

		public override void OnPreInit(ITemplatePage template)
		{
			if (!string.IsNullOrEmpty(masterPageFile))
			{
				template.Page.MasterPageFile = masterPageFile;
			}
		}
	}
}