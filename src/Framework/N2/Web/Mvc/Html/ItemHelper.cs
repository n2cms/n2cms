using System;
using System.Web.Mvc;
using N2.Engine;
using N2.Web.Parts;

namespace N2.Web.Mvc.Html
{
    public abstract class ItemHelper
    {
        PartsAdapter partsAdapter;
        ContentItem currentItem;

        protected ItemHelper(HtmlHelper helper, ContentItem currentItem)
        {
            Html = helper;
            CurrentItem = currentItem;
        }

        protected HtmlHelper Html { get; set; }

        protected ContentItem CurrentItem
        {
            get { return currentItem ?? (currentItem = Html.CurrentItem()); }
            set { currentItem = value; }
        }

        /// <summary>The content adapter related to the current page item.</summary>
        protected virtual PartsAdapter PartsAdapter
        {
            get { return partsAdapter ?? (partsAdapter = Adapters.ResolveAdapter<PartsAdapter>(CurrentItem)); }
        }

        protected IContentAdapterProvider Adapters
        {
            get { return Html.ResolveService<IContentAdapterProvider>(); }
        }
    }
}
