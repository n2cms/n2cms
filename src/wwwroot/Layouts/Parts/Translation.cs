using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Engine.Globalization;

namespace N2.Templates.UI.Layouts.Parts
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
