using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Engine.Globalization;

namespace N2.Edit.Globalization
{
    //[EditToolbarPlugin("{ManagementUrl}/Content/Globalization/LanguageMenu.ascx")]
    public partial class LanguageMenu : EditPageUserControl
    {
        protected Repeater rptLanguages;

        protected bool CreatingNew
        {
            get { return Request["discriminator"] != null; }
        }
        protected ILanguageGateway Gateway
        {
            get { return Engine.Resolve<LanguageGatewaySelector>().GetLanguageGateway(Selection.SelectedItem); }
        }
        protected ILanguage CurrentLanguage
        {
            get { return Gateway.GetLanguage(Selection.SelectedItem); }
        }

        protected override void OnPreRender(EventArgs e)
        {
            List<TranslateSpecification> translations = new List<TranslateSpecification>(Gateway.GetEditTranslations(Selection.SelectedItem, false, false));
            if (Gateway.Enabled && translations.Count > 0)
            {
                if (!CreatingNew)
                {
                    rptLanguages.DataSource = translations;
                }
                DataBind();
            }
            else
            {
                Visible = false;
            }
            base.OnPreRender(e);
        }
    }
}
