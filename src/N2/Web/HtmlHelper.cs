using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace N2.Web
{
    public class HtmlHelper
    {
        ContentItem currentItem;
        Page page;

        public HtmlHelper(Page page, ContentItem currentItem)
        {
            this.page = page;
            this.currentItem = currentItem;
        }

        public ContentItem CurrentItem
        {
            get { return currentItem; }
            set { currentItem = value; }
        }

        public Page Page
        {
            get { return page; }
            set { page = value; }
        }

        public Url Url(string url)
        {
            return new Url(url);
        }

        public TagBuilder A(string url, string text)
        {
            return new TagBuilder("a", text).Attr("href", url);
        }

        public TagBuilder Img(string url, string alt)
        {
            return new TagBuilder("img", string.Empty).Attr("src", url).Attr("alt", alt);
        }

        public TagBuilder Radio(string name, string value)
        {
            return new TagBuilder("input", string.Empty).Attr("type", "radio").Attr("name", name).Attr("value", value);
        }
    }
}
