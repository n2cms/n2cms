using N2.Engine;
using N2.Plugin;
using N2.Templates.Mvc._Class.Configuration;
using N2.Templates.Mvc.Web;
using N2.Templates.Configuration;
using N2.Web.Mail;
using System.Configuration;

namespace N2.Templates.Mvc.Services
{
	/// <summary>
	/// Registers components and services needed by the templates project.
	/// </summary>
	[AutoInitialize]
	public class TemplatesInitializer : IPluginInitializer
	{
		public void Initialize(IEngine engine)
		{
			engine.AddComponent("n2.templates.pagemodifier", typeof(IPageModifierContainer), typeof (TemplatePageModifier));
			TemplatesSection config = ConfigurationManager.GetSection("n2/templates") as TemplatesSection;
			if (config == null || config.MailConfiguration == MailConfigSource.ContentRootOrConfiguration)
			{
				engine.AddComponent("n2.templates.contentMailSender", typeof(IMailSender), typeof(DynamicMailSender));
			}
			else
			{
				engine.AddComponent("n2.templates.fakeMailSender", typeof(IMailSender), typeof(FakeMailSender));
			}
			engine.AddComponent("n2.templates.permissionDeniedHandler", typeof(PermissionDeniedHandler));

			engine.AddComponent("n2.templates.syndication.rssWriter", typeof(RssWriter));
			engine.AddComponent("n2.templates.rss.definitionAppender", typeof(SyndicatableDefinitionAppender));

			engine.AddComponent("n2.templates.seo.definitions", typeof(SEODefinitionAppender));
			engine.AddComponent("n2.templates.seo.modifier", typeof(SEOPageModifier));
		}
	}
}