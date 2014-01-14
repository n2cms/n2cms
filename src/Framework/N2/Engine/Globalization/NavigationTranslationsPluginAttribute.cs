using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using N2.Edit;
using N2.Web;
using System.Linq;

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
            var selector = context.Engine.Resolve<LanguageGatewaySelector>();
            if (!selector.Enabled || selector.LanguagesPerSite /*avoid showing options that might not be relevant */)
                return null;

            ILanguageGateway gateway = selector.GetAllLanguages();

            HtmlGenericControl div = new HtmlGenericControl("div");
            div.Attributes["class"] = "languages";
            container.Controls.Add(div);

            base.AddTo(div, context);

            foreach (ILanguage language in gateway.GetAvailableLanguages())
            {
                Url url = Engine.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Content/Globalization/Translate.aspx");
                url = url.AppendQuery("language", language.LanguageCode);
                url = url.AppendQuery(SelectionUtility.SelectedQueryKey + "={selected}");

                HyperLink h = new HyperLink();
                h.ID = language.LanguageCode.Replace('-', '_').Replace(' ', '_');
                h.Target = Targets.Preview;
                h.NavigateUrl = context.Rebase(context.Format(url, true));
                h.CssClass = "templatedurl language";
                h.ToolTip = language.LanguageTitle;
                
                // [bherila] use CSS sprite instead of flag image here
                //h.Text = string.Format("<img src='{0}' alt=''/>", Url.ToAbsolute(language.FlagUrl));
                string[] parts = language.LanguageCode.Split('-');
                h.Text = string.Format(@"<span class=""{0} sprite""></span>", parts.LastOrDefault().ToLower());

                h.Attributes["data-url-template"] = context.Rebase(url);
                div.Controls.Add(h);
            }

            return div;
        }
    }
}
