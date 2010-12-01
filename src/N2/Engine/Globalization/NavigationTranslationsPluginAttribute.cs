using System;
using System.Collections.Generic;
using System.Text;
using N2.Edit;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web;
using System.Web.UI.HtmlControls;

namespace N2.Engine.Globalization
{
    /// <summary>
    /// Adds language icons to the right-click menu in the navigation pane.
    /// </summary>
	public class NavigationTranslationsPluginAttribute : NavigationSeparatorPluginAttribute
    {
        public NavigationTranslationsPluginAttribute(string name, int sortOrder)
            : base(name, sortOrder)
        {
        }

        public override Control AddTo(Control container, PluginContext context)
        {
            ILanguageGateway gateway = N2.Context.Current.Resolve<ILanguageGateway>();
            if (!gateway.Enabled)
                return null;

            HtmlGenericControl div = new HtmlGenericControl("div");
            div.Attributes["class"] = "languages";
            container.Controls.Add(div);

            base.AddTo(div, context);

            foreach (ILanguage language in gateway.GetAvailableLanguages())
            {
				Url url = Engine.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Content/Globalization/Translate.aspx");
                url = url.AppendQuery("language", language.LanguageCode);
                url = url.AppendQuery("selected={selected}");

                HyperLink h = new HyperLink();
                h.ID = language.LanguageCode.Replace('-', '_').Replace(' ', '_');
                h.Target = Targets.Preview;
				h.NavigateUrl = context.Rebase(context.Format(url, true));
                h.CssClass = "language";
                h.ToolTip = language.LanguageTitle;
                h.Text = string.Format("<img src='{0}' alt=''/>", Url.ToAbsolute(language.FlagUrl));
                div.Controls.Add(h);

                RegisterToolbarUrl(container, h.ClientID, url);
            }

            return div;
        }
    }
}
