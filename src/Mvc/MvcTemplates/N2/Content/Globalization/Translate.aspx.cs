using System;
using N2.Definitions;
using N2.Engine.Globalization;
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

            ILanguageGateway gateway = Engine.Resolve<LanguageGatewaySelector>().GetLanguageGateway(Selection.SelectedItem);
            ILanguage language = gateway.GetLanguage(languageCode);

            if (language != null)
            {
                ContentItem translation = gateway.GetTranslation(Selection.SelectedItem, language);
                if (translation != null)
                {
                    // item has a translation
                    string url = Engine.ManagementPaths.GetEditExistingItemUrl(translation);
                    Response.Redirect(url);
                }
                else if (Selection.SelectedItem.Parent != null)
                {
                    // item not translated, try to create translation
                    ContentItem parent = Selection.SelectedItem.Parent;
                    ContentItem parentTranslation = gateway.GetTranslation(parent, language);
                    if(parentTranslation != null)
                    {
                        // create new translation below translated parent
                        ItemDefinition definition = Engine.Definitions.GetDefinition(Selection.SelectedItem);
                        Url url = Engine.ManagementPaths.GetEditNewPageUrl(parentTranslation, definition, null, CreationPosition.Below);
                        url = url.AppendQuery(LanguageGateway.TranslationKey, Selection.SelectedItem.TranslationKey ?? Selection.SelectedItem.ID);
                        Response.Redirect(url);
                    }
                    else
                    {
                        // parent is not translated, cannot continue
                        cvCannotTranslate.IsValid = false;
                    }
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
