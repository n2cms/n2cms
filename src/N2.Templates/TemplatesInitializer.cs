using N2.Engine;
using N2.Plugin;
using N2.Templates.Web;
using System;
using N2.Templates.Configuration;
using N2.Web.Mail;
using System.Configuration;
using N2.Templates.Services;

namespace N2.Templates
{
	[AutoInitialize]
	public class TemplatesInitializer : IPluginInitializer
	{
		public void Initialize(IEngine engine)
		{
			engine.AddComponent("n2.templates.pagemodifier", typeof(IPageModifierContainer), typeof (TemplatePageModifier));
            TemplatesSection config = ConfigurationManager.GetSection("n2/templates") as TemplatesSection;
            if (config == null || config.MailConfiguration == MailConfigSource.ContentRootOrConfiguration)
            {
                engine.AddComponent("n2.templates.contentMailSender", typeof(IMailSender), typeof(Services.DynamicMailSender));
            }
            else
            {
                engine.AddComponent("n2.templates.fakeMailSender", typeof(IMailSender), typeof(Services.FakeMailSender));
            }
            //engine.AddComponent("n2.templates.notFoundHandler", typeof(NotFoundHandler));
            engine.AddComponent("n2.templates.permissionDeniedHandler", typeof(PermissionDeniedHandler));
        }
	}

    [N2.Plugin.Scheduling.ScheduleExecution(10)]
    public class Test : N2.Plugin.Scheduling.ScheduledAction
    {
        public override void Execute()
        {
            var count = N2.Context.Current.Resolve<N2.Web.IWebContext>().RequestItems.Count;
            System.Diagnostics.Debug.WriteLine(count);
        }
    }
}
