using System;

namespace N2.Web.UI
{
    public class TemplateUrlEventArgs : EventArgs
    {
        public TemplateUrlEventArgs()
        {
        }
        public TemplateUrlEventArgs(ContentItem item)
        {
            this.item = item;
            this.templateUrl = item.TemplateUrl;
        }
        
        private string templateUrl = null;
        ContentItem item;

        public ContentItem Item
        {
            get { return item; }
        }

        public string TemplateUrl
        {
            get { return templateUrl; }
            set { templateUrl = value; }
        }
    }
}
