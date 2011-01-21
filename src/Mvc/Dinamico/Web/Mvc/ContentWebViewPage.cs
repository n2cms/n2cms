using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2;
using N2.Web.Mvc.Html;
using N2.Web;
using N2.Collections;

namespace N2.Web.Mvc
{
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
			if(item is ILink)
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

	public class ContentContext<TModel> where TModel:class
	{
		ContentWebViewPage<TModel> page;

		public ContentContext(ContentWebViewPage<TModel> page)
		{
			this.page = page;
		}

		public TModel Current
		{
			get { return CurrentItem as TModel; }
		}

		private ContentItem CurrentItem
		{
			get { return page.Html.ViewContext.CurrentItem(); }
		}

		public ContentItem StartPage
		{
			get { return N2.Find.StartPage; }
		}

		public IEnumerable<ContentItem> Ancestors
		{
			get { return N2.Find.EnumerateParents(CurrentItem, StartPage, true); }
		}

		public IEnumerable<ContentItem> AncestorsBetween(int startLevel = 0, int depth = 1)
		{
			return N2.Find.EnumerateParents(CurrentItem, StartPage, true).Reverse().Skip(startLevel).Take(depth);
		}

		public IList<ContentItem> Children(ContentItem item)
		{
			return item.GetChildren(new NavigationFilter());
		}

		public int Level
		{
			get { return Ancestors.Count(); }
		}

		public ContentItem AncestorAtLevel(int level)
		{
			return Ancestors.Reverse().Skip(level).FirstOrDefault();
		}

		public Tree TreeFrom(int level = 0, int depth = 3, bool excludeRoot = false, Func<ContentItem, string> classProvider = null)
		{
			return TreeFrom(AncestorAtLevel(level), depth, excludeRoot, classProvider);
		}

		public Tree TreeFrom(ContentItem item, int depth = 3, bool excludeRoot = false, Func<ContentItem, string> classProvider = null)
		{
			if (classProvider == null)
				classProvider = GetNavigationClass;

			return new TreeBuilder(new TreeHierarchyBuilder(item, depth))
				.ExcludeRoot(excludeRoot)
				.LinkProvider((i) => LinkTo(i).Class(classProvider(i)))
				.Filters(new NavigationFilter());
		}

		public string GetNavigationClass(ContentItem item)
		{
			return Current == item ? "current" : Ancestors.Contains(item) ? "trail" : "";
		}

		public ILinkBuilder LinkTo(ContentItem item)
		{
			return new LinkBuilder(item);
		}
	}

	public abstract class ContentWebViewPage : ContentWebViewPage<ContentItem>
	{
	}

	public abstract class ContentWebViewPage<TModel> : WebViewPage<TModel> where TModel:class
	{
		private ContentContext<TModel> content;

		public ContentContext<TModel> Content
		{
			get { return content ?? (content = new ContentContext<TModel>(this)); }
			set { content = value; }
		}
	}
}