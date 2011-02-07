using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Collections;
using N2.Persistence.Finder;
using N2.Web.Mvc.Html;

namespace N2.Web.Mvc
{	
	public class ContentHelper<TModel> where TModel : class
	{
		ContentWebViewPage<TModel> page;

		public ContentHelper(ContentWebViewPage<TModel> page)
		{
			this.page = page;
		}

		public HtmlHelper<TModel> Html
		{
			get { return page.Html; }
		}

		public TModel Model
		{
			get { return Html.ViewData.Model as TModel; }
		}

		public ContentItem CurrentItem
		{
			get { return page.Html.CurrentItem(); }
		}

		public ContentItem CurrentPage
		{
			get { return page.Html.CurrentPage(); }
		}

		public ContentItem StartPage
		{
			get { return N2.Find.StartPage; }
		}

		public ContentItem RootPage
		{
			get { return N2.Find.RootItem; }
		}

		protected virtual ItemFilter DefaultFilter
		{
			get { return new NavigationFilter(); }
		}

		public IEnumerable<ContentItem> Ancestors(ContentItem item = null, ItemFilter filter = null)
		{
			return (filter ?? DefaultFilter).Pipe(N2.Find.EnumerateParents(item ?? CurrentItem, StartPage, true));
		}

		public IEnumerable<ContentItem> AncestorsBetween(int skipLevel = 0, int takeLevels = 1)
		{
			return N2.Find.EnumerateParents(CurrentItem, StartPage, true).Reverse().Skip(skipLevel).Take(takeLevels);
		}

		public IEnumerable<ContentItem> Children(ContentItem item, ItemFilter filter = null)
		{
			return item.GetChildren(filter ?? new NavigationFilter());
		}

		public IEnumerable<ContentItem> Descendants(ContentItem item, ItemFilter filter = null)
		{
			return N2.Find.EnumerateChildren(item).Where((filter ?? DefaultFilter).Match);
		}

		public IEnumerable<ContentItem> Siblings(ContentItem item = null)
		{
			if (item == null)
				item = CurrentItem;
			if (item.Parent == null)
				return Enumerable.Empty<ContentItem>();

			return item.Parent.GetChildren(new NavigationFilter());
		}

		public int Level(ContentItem item = null)
		{
			return Ancestors(item).Count();
		}

		public ContentItem AncestorAtLevel(int level)
		{
			return Ancestors().Reverse().Skip(level).FirstOrDefault();
		}

		public Tree TreeFrom(int skipLevels = 0, int takeLevels = 3, bool rootless = false, Func<ContentItem, string> cssGetter = null, ItemFilter filter = null)
		{
			return TreeFrom(AncestorAtLevel(skipLevels), takeLevels, rootless, cssGetter, filter);
		}

		public Tree TreeFrom(ContentItem item, int takeLevels = 3, bool rootless = false, Func<ContentItem, string> cssGetter = null, ItemFilter filter = null)
		{
			if (cssGetter == null)
				cssGetter = GetNavigationClass;

			return new TreeBuilder(new TreeHierarchyBuilder(item, takeLevels))
				.ExcludeRoot(rootless)
				.LinkProvider((i) => LinkTo(i).Class(cssGetter(i)))
				.Filters(filter ?? DefaultFilter);
		}

		public string GetNavigationClass(ContentItem item)
		{
			return Model == item ? "current" : Ancestors().Contains(item) ? "trail" : "";
		}

		public ILinkBuilder LinkTo(ContentItem item)
		{
			return new LinkBuilder(item);
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

		public dynamic Display
		{
			get { return new DisplayHelper<TModel>(Html, CurrentItem); }
		}

		public dynamic Data
		{
			get 
			{
				var data = Html.ViewContext.ViewData["DataHelper" + CurrentItem.Path] as DataHelper;
				if(data == null)
					Html.ViewContext.ViewData["DataHelper" + CurrentItem.Path] = data = new DataHelper(CurrentItem);
				return data;
			}
		}
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