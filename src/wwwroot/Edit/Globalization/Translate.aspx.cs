using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Globalization;
using N2.Definitions;

namespace N2.Edit.Globalization
{
    /// <summary>
    /// Redirects to editing a translation.
    /// </summary>
    [NavigationTranslationsPlugin("translate", 70)]
    public partial class Translate : N2.Edit.Web.EditPage
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
                    string url = Engine.EditManager.GetEditNewPageUrl(parent, definition, null, CreationPosition.Below);
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
