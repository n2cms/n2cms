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
	/// <summary>
	/// Provides quick acccess to often used APIs.
	/// </summary>
	public class ViewContentHelper : ContentHelperBase
	{
		HtmlHelper html;

		public ViewContentHelper(HtmlHelper html)
			: base(html.ContentEngine(), () => html.CurrentPath())
		{
			this.html = html;
		}

		public HtmlHelper Html
		{
			get { return html; }
		}

		public virtual RegisterHelper Register
		{
			get { return new RegisterHelper(Html); }
		}

		public virtual RenderHelper Render
		{
			get { return new RenderHelper { Html = Html, Content = Path.CurrentItem }; }
		}

		// markup

		public string UniqueID(string prefix = null)
		{
			if (string.IsNullOrEmpty(prefix))
				return "_" + Path.CurrentItem.ID;

			return prefix + Path.CurrentItem.ID;
		}

		public Tree TreeFrom(int skipLevels = 0, int takeLevels = 3, bool rootless = false, Func<ContentItem, string> cssGetter = null, ItemFilter filter = null)
		{
			return TreeFrom(Traverse.AncestorAtLevel(skipLevels), takeLevels, rootless, cssGetter, filter);
		}

		public Tree TreeFrom(ContentItem item, int takeLevels = 3, bool rootless = false, Func<ContentItem, string> cssGetter = null, ItemFilter filter = null)
		{
			if (item == null)
				return CreateTree(new NoHierarchyBuilder());

			if (cssGetter == null)
				cssGetter = GetNavigationClass;

			return CreateTree(new TreeHierarchyBuilder(item, takeLevels))
				.ExcludeRoot(rootless)
				.LinkProvider((i) => LinkTo(i).Class(cssGetter(i)))
				.Filters(filter ?? N2.Content.Is.Navigatable());
		}

		public string GetNavigationClass(ContentItem item)
		{
			return Path.CurrentItem == item ? "current" : Traverse.Ancestors().Contains(item) ? "trail" : "";
		}

		public ILinkBuilder LinkTo(ContentItem item)
		{
			if (item == null) return CreateLink(item);

			var lb = CreateLink(item);
			lb.Class(GetNavigationClass(item));
			return lb;
		}

		public bool HasValue(string detailName)
		{
			return Path.CurrentItem[detailName] != null && !("".Equals(Path.CurrentItem[detailName]));
		}

		protected virtual ILinkBuilder CreateLink(ContentItem item)
		{
			return Link.To(item);
		}

		protected virtual Tree CreateTree(HierarchyBuilder hierarchy)
		{
			return Tree.Using(hierarchy);
		}
	}


}