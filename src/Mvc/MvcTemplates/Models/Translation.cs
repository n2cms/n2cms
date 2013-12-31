using N2.Engine.Globalization;

namespace N2.Templates.Mvc
{
    public class Translation
    {
        private ContentItem page;
        private ILanguage language;

        public Translation(ContentItem page, ILanguage language)
        {
            this.page = page;
            this.language = language;
        }

        public ContentItem Page
        {
            get { return page; }
            set { page = value; }
        }

        public ILanguage Language
        {
            get { return language; }
            set { language = value; }
        }
    }
}
