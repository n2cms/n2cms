using N2.Engine;
using N2.Plugin;
using N2.Templates.Web;
using System;

namespace N2.Templates
{
	[AutoInitialize]
	public class TemplatesInitializer : IPluginInitializer
	{
		public void Initialize(IEngine engine)
		{
			engine.AddComponent("n2.templates.pagemodifier", typeof(IPageModifierContainer), typeof (TemplatePageModifier));
			engine.AddComponent("ne.templtaes.mailSender", typeof(Services.IMailSender), typeof(Services.DynamicMailSender));
		}
	}
}
