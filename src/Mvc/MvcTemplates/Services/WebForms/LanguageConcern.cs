using N2.Engine.Globalization;
using N2.Web.UI;
using N2.Engine;

namespace N2.Templates.Mvc.Services.WebForms
{
    [Service(typeof(ContentPageConcern))]
    public class LanguageConcern : ContentPageConcern
    {
        ILanguageGateway gateway;

        public LanguageConcern(ILanguageGateway gateway)
        {
            this.gateway = gateway;
        }

        public override void OnPreInit(System.Web.UI.Page page, ContentItem item)
        {
            if (item == null) return;

            ILanguage language = gateway.GetLanguage(item);
            if (language != null && !string.IsNullOrEmpty(language.LanguageCode))
            {
                page.Culture = language.LanguageCode;
                page.UICulture = language.LanguageCode;
            }
        }
    }
}
