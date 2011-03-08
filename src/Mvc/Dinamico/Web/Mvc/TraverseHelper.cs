using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Collections;
using N2.Engine.Globalization;
using N2.Web.Mvc.Html;
using System.Web.Mvc;
using N2.Definitions;

namespace N2.Web.Mvc
{
	public class TraverseHelper
	{
		HtmlHelper html;

		public TraverseHelper(HtmlHelper html)
		{
			this.html = html;
		}

		public ContentItem CurrentItem
		{
			get { return html.CurrentItem(); }
		}

		public ContentItem CurrentPage
		{
			get { return html.CurrentPage(); }
		}

		public ILanguage CurrentLanguage
		{
			get { return html.ResolveService<ILanguageGateway>().GetLanguage(CurrentPage); }
		}

		public IEnumerable<ILanguage> Translations()
		{
			var lg = html.ResolveService<ILanguageGateway>();
			return lg.FindTranslations(CurrentPage).Select(i => lg.GetLanguage(i));
		}

		public ContentItem StartPage
		{
			get { return N2.Find.ClosestOf<IStartPage>(CurrentPage) ?? N2.Find.StartPage; }
		}

		public ContentItem RootPage
		{
			get { return N2.Find.ClosestOf<IRootPage>(CurrentPage) ?? N2.Find.RootItem; }
		}

		public virtual ItemFilter DefaultFilter()
		{
			return new NavigationFilter();
		}

		public IEnumerable<ContentItem> Ancestors(ContentItem item = null, ItemFilter filter = null)
		{
			return (filter ?? DefaultFilter()).Pipe(N2.Find.EnumerateParents(item ?? CurrentItem, StartPage, true));
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
			return N2.Find.EnumerateChildren(item).Where((filter ?? DefaultFilter()).Match);
		}

		public IEnumerable<ContentItem> DescendantPages(ContentItem item, ItemFilter filter = null)
		{
			return N2.Find.EnumerateChildren(item).Where(p => p.IsPage).Where((filter ?? DefaultFilter()).Match);
		}

		public IEnumerable<ContentItem> Siblings(ContentItem item = null)
		{
			if (item == null)
				item = CurrentItem;
			if (item.Parent == null)
				return Enumerable.Empty<ContentItem>();

			return item.Parent.GetChildren(new NavigationFilter());
		}

		public int LevelOf(ContentItem item = null)
		{
			return Ancestors(item).Count();
		}

		public ContentItem AncestorAtLevel(int level)
		{
			return Ancestors().Reverse().Skip(level).FirstOrDefault();
		}

	}
}