using Castle.Core;
using N2.Definitions;
using N2.Details;
using N2.Web.UI;
using N2.Plugin;

namespace N2.Templates.Services
{
    public class SeoDefinitionAppender : IAutoStart
    {
        private readonly IDefinitionManager definitions;
        private string titleTitle = "Page title";
        private string metaKeywordsTitle = "Meta keywords";
        private string metaDescriptionTitle = "Meta description";
        private string seoTabTitle = "SEO";

        public SeoDefinitionAppender(IDefinitionManager definitions)
        {
            this.definitions = definitions;
        }

        public string TitleTitle
        {
            get { return titleTitle; }
            set { titleTitle = value; }
        }

        public string MetaKeywordsTitle
        {
            get { return metaKeywordsTitle; }
            set { metaKeywordsTitle = value; }
        }

        public string MetaDescriptionTitle
        {
            get { return metaDescriptionTitle; }
            set { metaDescriptionTitle = value; }
        }

        public string SeoTabTitle
        {
            get { return seoTabTitle; }
            set { seoTabTitle = value; }
        }

        #region IStartable Members

        public void Start()
        {
            foreach (ItemDefinition definition in definitions.GetDefinitions())
            {
                if(IsPage(definition))
                {
                    TabContainerAttribute seoTab = new TabContainerAttribute("seo", SeoTabTitle, 30);
                    definition.Add(seoTab);

                    AddEditableText(definition, TitleTitle, SeoConcern.HeadTitle, 151, 200);
                    AddEditableText(definition, MetaKeywordsTitle, SeoConcern.MetaKeywords, 152, 400);
                    AddEditableText(definition, MetaDescriptionTitle, SeoConcern.MetaDescription, 153, 1000);
                }
            }
        }

        private void AddEditableText(ItemDefinition definition, string title, string name, int sortOrder, int maxLength)
        {
            EditableTextAttribute titleEditor = new EditableTextAttribute(title, sortOrder, maxLength);
            titleEditor.Name = name;
            titleEditor.ContainerName = "seo";
            definition.Add(titleEditor);
        }

        private bool IsPage(ItemDefinition definition)
        {
            return typeof (Items.AbstractPage).IsAssignableFrom(definition.ItemType);
        }

        public void Stop()
        {
        }

        #endregion
    }
}