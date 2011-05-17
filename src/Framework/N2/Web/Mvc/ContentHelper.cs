using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Collections;
using N2.Linq;
using N2.Web.Mvc.Html;
using N2.Persistence.Finder;
using N2.Engine;

namespace N2.Web.Mvc
{	
	public class ContentHelper
	{
		HtmlHelper html;

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
			get { return Html.CurrentItem(); }
		}

		public ContentItem CurrentPage
		{
			get { return Html.CurrentPage(); }
		}

		public virtual TraverseHelper Traverse
		{
			get {  return new TraverseHelper(Html); }
		}

		public virtual IQueryable<T> Query<T>()
		{
			return Html.ContentEngine().Query<T>();
		}

		public IItemFinder Find
		{
			get { return Html.ResolveService<IItemFinder>(); }
		}

		public virtual RegisterHelper Register
		{
			get { return new RegisterHelper(Html); }
		}

		public virtual RenderHelper Render
		{
			get { return new RenderHelper { Html = Html, Content = CurrentItem }; }
		}

		public virtual FilterHelper Is
		{
			get { return new FilterHelper(); }
		}

		public virtual IEngine Engine
		{
			get { return Html.ContentEngine(); }
		}

		public virtual IServiceContainer Services
		{
			get { return Html.ResolveService<IServiceContainer>(); }
		}

		// markup

		public string UniqueID(string prefix = null)
		{
			if (string.IsNullOrEmpty(prefix))
				return "_" + CurrentItem.ID;

			return prefix + CurrentItem.ID;
		}

		public Tree TreeFrom(int skipLevels = 0, int takeLevels = 3, bool rootless = false, Func<ContentItem, string> cssGetter = null, ItemFilter filter = null)
		{
			return TreeFrom(Traverse.AncestorAtLevel(skipLevels), takeLevels, rootless, cssGetter, filter);
		}

		public Tree TreeFrom(ContentItem item, int takeLevels = 3, bool rootless = false, Func<ContentItem, string> cssGetter = null, ItemFilter filter = null)
		{
			if (cssGetter == null)
				cssGetter = GetNavigationClass;

			return CreateTree(new TreeHierarchyBuilder(item, takeLevels))
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
			if (item == null) return CreateLink(item);

			var lb = CreateLink(item);
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
			return new ContentScope(newCurrentItem, Html.ViewContext.ViewData);
		}

		public IDisposable BeginScope(string newCurrentItemUrlOrId)
		{
			if (newCurrentItemUrlOrId != null)
			{
				int id;
				ContentItem item = null;
				if (int.TryParse(newCurrentItemUrlOrId, out id))
					item = html.ContentEngine().Persister.Get(id);

				if(item == null)
					item = Html.ResolveService<IUrlParser>().Parse(newCurrentItemUrlOrId);

				if (item != null)
					return new ContentScope(item, html.ViewContext.ViewData);
			}
			return new EmptyDisposable();
		}

		public void EndScope()
		{
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

		protected virtual Link CreateLink(ContentItem item)
		{
			return new Link(item);
		}

		protected virtual Tree CreateTree(HierarchyBuilder hierarchy)
		{
			return new Tree(hierarchy);
		}
	}


}