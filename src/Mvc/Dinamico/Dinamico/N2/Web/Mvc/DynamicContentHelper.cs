using System.Web.Mvc;
using System.Web;
using N2.Details;
using N2.Definitions;
using N2.Web.Rendering;
using N2.Definitions.Runtime;
using N2.Collections;
using System;

namespace N2.Web.Mvc
{
	/// <remarks>This code is here since it has dependencies on ASP.NET 3.0 which isn't a requirement for N2 in general.</remarks>
	public class DynamicContentHelper : ViewContentHelper
	{
		public DynamicContentHelper(HtmlHelper html)
			: base(html)
		{
		}

		public dynamic Display
		{
			get { return new DisplayHelper { Html = Html, Current = Current.Item }; }
		}

		public dynamic Has
		{
			get { return new HasValueHelper(HasValue); }
		}

		public dynamic Data
		{
			get
			{
				if (Current.Item == null)
					return new DataHelper(() => Current.Item);

				string key = "DataHelper" + Current.Item.ID;
				var data = Html.ViewContext.ViewData[key] as DataHelper;
				if (data == null)
					Html.ViewContext.ViewData[key] = data = new DataHelper(() => Current.Item);
				return data;
			}
		}

		public TranslateHelper Translate
		{
			get { return new TranslateHelper(); }
		}

		// Room for future improvement.
		public class TranslateHelper
		{
			public IHtmlString this[string key]
			{
				get { return Html(key); }
			}

			public IHtmlString Html(string key)
			{
				return new HtmlString(key);
			}

			public string Text(string key)
			{
				return key;
			}
		}

		//// obsolete stuff from 2.2 remains here for a while (uncomment to use)

		//[Obsolete("Use Html.UniqueID")]
		//public string UniqueID(string prefix = null)
		//{
		//    if (string.IsNullOrEmpty(prefix))
		//        return "_" + Current.Page.ID;

		//    return prefix + Current.Page.ID;
		//}

		//[Obsolete("Use Html.Tree")]
		//public Tree TreeFrom(int skipLevels = 0, int takeLevels = 3, bool rootless = false, Func<ContentItem, string> cssGetter = null, ItemFilter filter = null)
		//{
		//    return TreeFrom(Traverse.AncestorAtLevel(skipLevels), takeLevels, rootless, cssGetter, filter);
		//}

		//[Obsolete("Use Html.Tree")]
		//public Tree TreeFrom(ContentItem item, int takeLevels = 3, bool rootless = false, Func<ContentItem, string> cssGetter = null, ItemFilter filter = null)
		//{
		//    if (item == null)
		//        return Tree.Using(new NoHierarchyBuilder());

		//    if (cssGetter == null)
		//        cssGetter = GetNavigationClass;

		//    return Tree.Using(new TreeHierarchyBuilder(item, takeLevels))
		//        .ExcludeRoot(rootless)
		//        .LinkWriter((n, w) => LinkTo(n.Current).Class(cssGetter(n.Current)).WriteTo(w))
		//        .Filters(filter ?? N2.Content.Is.Navigatable());
		//}

		//[Obsolete]
		//public string GetNavigationClass(ContentItem item)
		//{
		//    return Current.Page == item ? "current" : Traverse.Ancestors().Contains(item) ? "trail" : "";
		//}

		//[Obsolete("Use Html.Link")]
		//public ILinkBuilder LinkTo(ContentItem item)
		//{
		//    if (item == null) return Link.To(item);

		//    var lb = Link.To(item);
		//    lb.Class(GetNavigationClass(item));
		//    return lb;
		//}
	}
}