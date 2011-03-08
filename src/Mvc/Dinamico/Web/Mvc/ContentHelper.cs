using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Collections;
using N2.Persistence.Finder;
using N2.Web.Mvc.Html;
using N2.Definitions;
using N2.Engine.Globalization;
using N2.Web.Rendering;
using N2.Details;
using System.IO;

namespace N2.Web.Mvc
{	
	public class ContentHelper
	{
		HtmlHelper html;
		TraverseHelper traverse;
		ContentItem currentItem;
		ContentItem currentPage;

		public ContentHelper(HtmlHelper html)
		{
			this.html = html;
		}

		public HtmlHelper Html
		{
			get { return html; }
		}

		public ContentItem CurrentItem
		{
			get { return currentItem ?? (currentItem = Html.CurrentItem()); }
			set { currentItem = value; }
		}

		public ContentItem CurrentPage
		{
			get { return currentPage ?? (currentPage = Html.CurrentPage()); }
			set { currentPage = value; }
		}

		public TraverseHelper Traverse
		{
			get { return traverse ?? (traverse = new TraverseHelper(Html)); }
		}

		public Tree TreeFrom(int skipLevels = 0, int takeLevels = 3, bool rootless = false, Func<ContentItem, string> cssGetter = null, ItemFilter filter = null)
		{
			return TreeFrom(Traverse.AncestorAtLevel(skipLevels), takeLevels, rootless, cssGetter, filter);
		}

		public Tree TreeFrom(ContentItem item, int takeLevels = 3, bool rootless = false, Func<ContentItem, string> cssGetter = null, ItemFilter filter = null)
		{
			if (cssGetter == null)
				cssGetter = GetNavigationClass;

			return new TreeBuilder(new TreeHierarchyBuilder(item, takeLevels))
				.ExcludeRoot(rootless)
				.LinkProvider((i) => LinkTo(i).Class(cssGetter(i)))
				.Filters(filter ?? Traverse.DefaultFilter());
		}

		public string GetNavigationClass(ContentItem item)
		{
			return CurrentItem == item ? "current" : Traverse.Ancestors().Contains(item) ? "trail" : "";
		}

		public ILinkBuilder LinkTo(ContentItem item)
		{
			var lb = new LinkBuilder(item);
			lb.ClassName = GetNavigationClass(item);
			return lb;
		}

		public IItemFinder Find()
		{
			return Html.ResolveService<IItemFinder>();
		}

		public IQueryAction FindDescendant(ContentItem root = null)
		{
			if (root == null)
				root = CurrentItem;
			return Html.ResolveService<IItemFinder>().Where.AncestralTrail.Like(Utility.GetTrail(root) + "%");
		}

		public RegisterHelper Register
		{
			get { return new RegisterHelper(Html); }
		}

		public dynamic Display
		{
			get { return new DisplayHelper { Html = Html, Current = CurrentItem }; }
		}

		public dynamic Data
		{
			get 
			{
				if(CurrentItem == null)
					return new DataHelper(null);

				var data = Html.ViewContext.ViewData["DataHelper" + CurrentItem.Path] as DataHelper;
				if(data == null)
					Html.ViewContext.ViewData["DataHelper" + CurrentItem.Path] = data = new DataHelper(CurrentItem);
				return data;
			}
		}

		public RenderHelper Render
		{
			get { return new RenderHelper { Html = Html, Content = CurrentItem }; }
		}

		public IDisposable BeginContentScope(ContentItem newCurrentItem)
		{
			currentItem = null;
			return new ContentScope(newCurrentItem, Html.ViewContext.ViewData);
		}

		public void EndContentScope()
		{
			currentItem = null;
			ContentScope.End(Html.ViewData);
		}

		#region class ContentScope
		class ContentScope : IDisposable
		{
			ViewDataDictionary viewData;

			public ContentScope(ContentItem newCurrentItem, ViewDataDictionary viewData)
			{
				this.viewData = viewData;
				viewData["PreviousItem"] = viewData[ContentRoute.ContentItemKey];
				viewData[ContentRoute.ContentItemKey] = newCurrentItem;
			}

			#region IDisposable Members

			public void Dispose()
			{
				End(viewData);
			}

			public static void End(ViewDataDictionary viewData)
			{
				if (viewData["PreviousItem"] == null)
					viewData.Remove(ContentRoute.ContentItemKey);
				else
					viewData[ContentRoute.ContentItemKey] = viewData["PreviousItem"];
				viewData.Remove("PreviousItem");
			}

			#endregion
		}
		#endregion
	}

	public class TreeBuilder : Tree, IHtmlString
	{
		#region Constructor

		public TreeBuilder(HierarchyBuilder builder)
			: base(builder)
		{
		}

		public TreeBuilder(HierarchyNode<ContentItem> root)
			: base(root)
		{
		}

		#endregion

		#region IHtmlString Members

		public string ToHtmlString()
		{
			return ToString();
		}

		#endregion
	}

	public class LinkBuilder : Link, IHtmlString
	{
		#region Constructor

		public LinkBuilder()
			: base(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
		{
		}

		public LinkBuilder(string text, string href)
			: base(text, string.Empty, string.Empty, href, string.Empty)
		{
		}

		public LinkBuilder(string text, string title, string target, string href)
			: base(text, title, target, href, string.Empty)
		{
		}

		public LinkBuilder(string text, string title, string target, string href, string className)
			: base(text, title, target, href, className)
		{
		}

		public LinkBuilder(ILink link)
		{
			UpdateFrom(link);
		}

		public LinkBuilder(ContentItem item)
			: base(item, string.Empty)
		{
		}

		public LinkBuilder(ContentItem item, string className)
			: base(item.Title, string.Empty, string.Empty, item.Url, className)
		{
			if (item is ILink)
				UpdateFrom(item as ILink);
		}

		#endregion

		#region IHtmlString Members

		public string ToHtmlString()
		{
			return ToString();
		}

		#endregion
	}
}