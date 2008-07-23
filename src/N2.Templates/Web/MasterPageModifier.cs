using System.IO;
using N2.Templates.Configuration;
using System.Configuration;
namespace N2.Templates.Web
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

		public void Modify<T>(UI.TemplatePage<T> page) 
			where T : Items.AbstractPage
		{
            if (config != null && !string.IsNullOrEmpty(config.MasterPageFile))
            {
            	page.MasterPageFile = config.MasterPageFile;
			}
		}
	}
}