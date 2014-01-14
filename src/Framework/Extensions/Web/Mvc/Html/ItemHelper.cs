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
            set { this.currentItem = value; }
        }

        /// <summary>The content adapter related to the current page item.</summary>
        protected virtual PartsAdapter PartsAdapter
        {
            get
            {
                if (partsAdapter == null)
                    partsAdapter = Adapters.ResolveAdapter<PartsAdapter>(CurrentItem);
                return partsAdapter;
            }
        }

        /// <summary>The content adapter related to the current page item.</summary>
        [Obsolete("Use PartsAdapter")]
        protected virtual MvcAdapter MvcAdapter
        {
            get
            {
                return Adapters.ResolveAdapter<MvcAdapter>(CurrentItem);
            }
        }

        [Obsolete]
        protected virtual MvcAdapter GetMvcAdapterFor(Type contentType)
        {
            return Adapters.ResolveAdapter<MvcAdapter>(contentType);
        }

        protected IContentAdapterProvider Adapters
        {
            get { return Html.ResolveService<IContentAdapterProvider>(); }
        }
    }
}
