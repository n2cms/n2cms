using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Collections;
using N2.Web.Mvc.Html;

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
				if (CurrentItem == null)
					return new DataHelper(() => CurrentItem);

				string key = "DataHelper" + CurrentItem.ID;
				var data = Html.ViewContext.ViewData[key] as DataHelper;
				if (data == null)
					Html.ViewContext.ViewData[key] = data = new DataHelper(() => CurrentItem);
				return data;
			}
		}

		public RenderHelper Render
		{
			get { return new RenderHelper { Html = Html, Content = CurrentItem }; }
		}

		public FilterHelper Is
		{
			get { return new FilterHelper(); }
		}

		public string UniqueID(string prefix = null)
		{
			if (string.IsNullOrEmpty(prefix))
				return "_" + CurrentItem.ID;

			return prefix + CurrentItem.ID;
		}

		// markup

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
				.Filters(filter ?? N2.Filter.Is.Navigatable());
		}

		public string GetNavigationClass(ContentItem item)
		{
			return CurrentItem == item ? "current" : Traverse.Ancestors().Contains(item) ? "trail" : "";
		}

		public ILinkBuilder LinkTo(ContentItem item)
		{
			if (item == null) return new LinkBuilder();

			var lb = new LinkBuilder(item);
			lb.ClassName = GetNavigationClass(item);
			return lb;
		}

		public bool HasValue(string detailName)
		{
			return CurrentItem[detailName] != null && !("".Equals(CurrentItem[detailName]));
		}

		// content scope

		public IDisposable BeginScope(ContentItem newCurrentItem)
		{
			currentItem = null;
			return new ContentScope(newCurrentItem, Html.ViewContext.ViewData);
		}

		public IDisposable BeginScope(string newCurrentItemUrl)
		{
			if (newCurrentItemUrl != null)
			{
				var item = Html.ResolveService<IUrlParser>().Parse(newCurrentItemUrl);
				if(item != null)
					return new ContentScope(item, html.ViewContext.ViewData);
			}
			return new EmptyDisposable();
		}

		public void EndScope()
		{
			currentItem = null;
			ContentScope.End(Html.ViewContext.ViewData);
		}

		class EmptyDisposable : IDisposable
		{
			#region IDisposable Members

			public void Dispose()
			{
			}

			#endregion
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