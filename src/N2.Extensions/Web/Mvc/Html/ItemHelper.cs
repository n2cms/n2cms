using System;
using N2.Engine;
using N2.Web.Parts;
using N2.Web.UI;

namespace N2.Web.Mvc.Html
{
	public abstract class ItemHelper
	{
		private readonly IItemContainer _itemContainer;
		private PartsAdapter _partsAdapter;

		protected ItemHelper(IItemContainer itemContainer)
		{
			_itemContainer = itemContainer;
			CurrentItem = itemContainer.CurrentItem;
		}

		protected ItemHelper(IItemContainer itemContainer, ContentItem item)
		{
			_itemContainer = itemContainer;
			CurrentItem = item;
		}

		protected virtual IEngine Engine
		{
			get { return Context.Current; }
		}

		protected IItemContainer Container
		{
			get { return _itemContainer; }
		}

		protected ContentItem CurrentItem { get; private set; }

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