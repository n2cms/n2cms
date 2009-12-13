using System;
using N2.Engine;
using N2.Web.Parts;
using N2.Web.UI;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
	public abstract class ItemHelper
	{
		PartsAdapter _partsAdapter;
        ContentItem currentItem;

        protected ItemHelper(ViewContext viewContext)
		{
            ViewContext = viewContext;
		}
        protected ItemHelper(ViewContext viewContext, ContentItem currentItem)
            : this(viewContext)
		{
            CurrentItem = currentItem;
		}

		protected ViewContext ViewContext { get; set; }

        protected virtual IEngine Engine
		{
			get { return Context.Current; }
		}

        protected ContentItem CurrentItem
        {
            get { return currentItem ?? (currentItem = ViewContext.CurrentItem()); }
            set { this.currentItem = value; }
        }

		/// <summary>The content adapter related to the current page item.</summary>
		protected virtual PartsAdapter PartsAdapter
		{
			get
			{
				return _partsAdapter ?? (_partsAdapter = Engine.Resolve<IContentAdapterProvider>()
				                                         	.ResolveAdapter<PartsAdapter>(CurrentItem.FindPath(PathData.DefaultAction)));
			}
		}
	}
}