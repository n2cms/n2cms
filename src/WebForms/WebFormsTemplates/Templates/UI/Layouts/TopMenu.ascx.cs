using System;
using System.Collections.Generic;
using N2.Engine.Globalization;
using N2.Templates.Layouts;
using N2.Collections;
using N2.Resources;

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
            Page.StyleSheet(Register.DefaultFlagsCssPath);

            languages = Engine.Resolve<LanguageGatewaySelector>().GetLanguageGateway(CurrentPage);

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
            ItemFilter languageFilter = new AllFilter(new AccessFilter(), new PublishedFilter());
            IEnumerable<ContentItem> translations = languages.FindTranslations(CurrentPage);
            foreach (ContentItem translation in languageFilter.Pipe(translations))
            {
                ILanguage language = languages.GetLanguage(translation);
                
                // Hide translations when filtered access to their language
                ContentItem languageItem = language as ContentItem;
                if(languageItem == null || languageFilter.Match(languageItem))
                    yield return new Translation(translation, language);
            }
        }
    }
}
