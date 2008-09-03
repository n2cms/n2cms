using System;
using System.Collections.Generic;
using N2.Engine.Globalization;
using N2.Templates.Layouts;

namespace N2.Templates.UI.Layouts
{
    public partial class TopMenu : Web.UI.TemplateUserControl<ContentItem>
    {
        public int MaxLevels
        {
            get { return tm.MaxLevels; }
            set { tm.MaxLevels = value; }
        }

        protected ILanguageGateway languages;
        protected override void OnInit(EventArgs e)
        {
            languages = Engine.Resolve<ILanguageGateway>();

            if (languages.Enabled)
            {
                rptLanguages.DataSource = GetTranslations();
                DataBind();

                rptLanguages.Visible = rptLanguages.Items.Count > 0;
            }
            else
            {
                rptLanguages.Visible = false;
            }
            base.OnInit(e);
        }

        private IEnumerable<Translation> GetTranslations()
        {
            foreach (ContentItem translation in languages.FindTranslations(CurrentPage))
            {
                yield return new Translation(translation, languages.GetLanguage(translation));
            }
        }
    }
}