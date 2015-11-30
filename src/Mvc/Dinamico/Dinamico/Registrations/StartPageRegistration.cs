using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Definitions.Runtime;
using Dinamico.Models;
using N2.Definitions;
using N2.Security;
using N2.Details;
using N2.Web.Mvc;

namespace Dinamico.Dinamico.Registrations
{
    [Registration]
    public class StartPageRegistration : FluentRegisterer<StartPage>
    {
        public override void RegisterDefinition(IContentRegistration<StartPage> register)
        {
            register.ControlledBy<Controllers.StartPageController>();

            register.Page(title: "Start Page", description: "The topmost node of a site. This can be placed below a language intersection to also represent a language");
            register.RestrictParents(typeof(IRootPage), typeof(LanguageIntersection));
			register.AvailableZone("Scripts");

            using (register.RecursiveContainer("SiteContainer", headingFormat: null).Allow(Permission.Administer).Begin())
            {
                using (register.TabContainer(Defaults.Containers.Site, "Site").Begin())
                {
                    register.On(sp => sp.Theme).ThemeSelection().Configure(ets => ets.EnablePreview = true);
                    register.On(sp => sp.LanguageCode).Languages();
                    register.On(sp => sp.FooterText).FreeText("Footer text")
                        .WithTokens();
                    register.On(sp => sp.Logotype).ImageUpload();
                    register.On(sp => sp.HostName).Text("Site host name (DNS)")
                        .Help("Sets a host name for this site/language. The web server must be configured to accept this host name for this to work.");

					register.On(sp => sp.LoginPage).Url("Login page")
						.Help("Page to display when authorization to a page fails.");
                }
            }

            using (register.WithinContainer(Defaults.Containers.Metadata))
            {
                register.RegisterEditable(new WithEditableTemplateSelectionAttribute());
				register.On(sp => sp.Author).Meta();
				register.On(sp => sp.Keywords).Meta();
				register.On(sp => sp.Description).Meta();
            }
        }
    }
}
