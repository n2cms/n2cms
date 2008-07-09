using N2.Engine;
using N2.Plugin;
using N2.Templates.Web;
using System;
using N2.Templates.Configuration;
using N2.Web.Mail;

namespace N2.Templates
{
	[AutoInitialize]
	public class TemplatesInitializer : IPluginInitializer
	{
		public void Initialize(IEngine engine)
		{
			engine.AddComponent("n2.templates.pagemodifier", typeof(IPageModifierContainer), typeof (TemplatePageModifier));
            TemplatesSection config = engine.Resolve<TemplatesSection>();
            if (config == null || config.MailConfiguration == MailConfigSource.ContentRootOrConfiguration)
            {
                engine.AddComponent("n2.templtaes.contentMailSender", typeof(IMailSender), typeof(Services.DynamicMailSender));
            }
            else
            {
                engine.AddComponent("n2.templtaes.fakeMailSender", typeof(IMailSender), typeof(Services.FakeMailSender));
            }
		}
	}
}
