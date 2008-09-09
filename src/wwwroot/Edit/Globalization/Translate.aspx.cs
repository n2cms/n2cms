using System;
using N2.Engine.Globalization;
using N2.Definitions;
using N2.Web;

namespace N2.Edit.Globalization
{
    /// <summary>
    /// Redirects to editing a translation.
    /// </summary>
    [NavigationTranslationsPlugin("translate", 70)]
    public partial class Translate : Web.EditPage
    {
        protected override void OnInit(EventArgs e)
        {
            string languageCode = Request["language"];

            ILanguageGateway gateway = Engine.Resolve<ILanguageGateway>();
            ILanguage language = gateway.GetLanguage(languageCode);

            if (language != null)
            {
                ContentItem translation = gateway.GetTranslation(SelectedItem, language);
                if (translation != null)
                {
                    string url = Engine.EditManager.GetEditExistingItemUrl(translation);
                    Response.Redirect(url);
                }
                else if (SelectedItem.Parent != null)
                {
                    ContentItem parent = SelectedItem.Parent;
                    ContentItem parentTranslation = gateway.GetTranslation(parent, language);
                    ItemDefinition definition = Engine.Definitions.GetDefinition(SelectedItem.GetType());
                    Url url = Engine.EditManager.GetEditNewPageUrl(parentTranslation, definition, null, CreationPosition.Below);
                    url = url.AppendQuery(LanguageGateway.LanguageKey, SelectedItem[LanguageGateway.LanguageKey] ?? SelectedItem.ID);
                    Response.Redirect(url);
                }
                else
                {
                    cvCannotTranslate.IsValid = false;
                }
            }
            else
            {
                throw new ArgumentException("Couldn't find any language for the with the language code: " + languageCode, "language");
            }

            base.OnInit(e);
        }
    }
}
