using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using N2.Collections;
using N2.Definitions;
using N2.Engine.Globalization;
using N2.Persistence.Finder;
using N2.Web.Mvc.Html;
using N2.Persistence.NH;
using System;
using N2.Engine;
using N2.Web;

namespace N2.Collections
{
	/// <summary>
	/// Simplifies traversing items in the content hierarchy.
	/// </summary>
	public class TraverseHelper
	{
		IEngine engine;
		FilterHelper filter;
		Func<PathData> pathGetter;
		ItemFilter defaultFilter;

		public TraverseHelper(IEngine engine, FilterHelper filter, Func<PathData> pathGetter)
		{
			this.engine = engine;
			this.filter = filter;
			this.pathGetter = pathGetter;
		}

		protected ContentItem CurrentItem
		{
			get { return pathGetter().CurrentItem; }
		}

		protected ContentItem CurrentPage
		{
			get { return pathGetter().CurrentPage; }
		}

		public ContentItem StartPage
		{
			get 
			{ 
				var page = N2.Find.ClosestOf<IStartPage>(CurrentItem) ?? engine.UrlParser.StartPage;
				var redirect = page as IRedirect;
				if (redirect != null && redirect.RedirectTo != null)
					return redirect.RedirectTo;
				return page;
			}
		}

		public ContentItem RootPage
		{
			get { return N2.Find.ClosestOf<IRootPage>(CurrentItem) ?? engine.Persister.Repository.Get(engine.Resolve<IHost>().CurrentSite.RootItemID); }
		}

		/// <summary>The default filter to apply to all results from this object.</summary>
		public ItemFilter DefaultFilter
		{
			get { return defaultFilter ?? (defaultFilter = filter.Accessible()); }
			set { defaultFilter = value; }
		}

		/// <summary>Translations of the current item.</summary>
		/// <returns></returns>
		public IEnumerable<ILanguage> Translations()
		{
			return Translations(CurrentPage);
		}

		/// <summary>Translations of the current item.</summary>
		/// <returns></returns>
		public IEnumerable<ILanguage> Translations(ContentItem item)
		{
			UseMasterVersion(ref item);
			var lg = engine.Resolve<ILanguageGateway>();
			return lg.FindTranslations(item).Select(i => lg.GetLanguage(i));
		}

		/// <summary>Ancestors of a given item.</summary>
		/// <param name="item"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public IEnumerable<ContentItem> Ancestors(ContentItem item = null, ItemFilter filter = null)
		{
			return N2.Find.EnumerateParents(item ?? CurrentItem, StartPage, true).Where(filter ?? DefaultFilter);
		}

		/// <summary>Ancestors between a start level and a descendant level.</summary>
		/// <param name="startLevel"></param>
		/// <param name="stopLevel"></param>
		/// <returns></returns>
		public IEnumerable<ContentItem> AncestorsBetween(int startLevel = 0, int stopLevel = 5)
		{
			var ancestors = N2.Find.EnumerateParents(CurrentItem, StartPage, true).ToList();
			ancestors.Reverse();
			if (stopLevel < 0)
				stopLevel = ancestors.Count + stopLevel;

			if (startLevel < stopLevel)
				for (int i = startLevel; i < stopLevel && i < ancestors.Count; i++)
					yield return ancestors[i];
			else
				for (int i = Math.Min(stopLevel, ancestors.Count - 1); i >= startLevel; i--)
					yield return ancestors[i];
		}

		/// <summary>Children of the current item.</summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		public IEnumerable<ContentItem> Children()
		{
			return Children(null);
		}

		/// <summary>Children of the current item.</summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		public IEnumerable<ContentItem> Children(ItemFilter filter)
		{
			return Children(CurrentItem, filter ?? DefaultFilter);
		}

		/// <summary>Children of a given item.</summary>
		/// <param name="parent"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public IEnumerable<ContentItem> Children(ContentItem parent, ItemFilter filter = null)
		{
			if (parent == null) return Enumerable.Empty<ContentItem>();
			UseMasterVersion(ref parent);
			
			return parent.GetChildren(filter ?? DefaultFilter);
		}

		/// <summary>Pages below the current item.</summary>
		/// <returns></returns>
		public IEnumerable<ContentItem> ChildPages()
		{
			return Children(CurrentPage, Content.Is.AccessiblePage());
		}

		/// <summary>Pages below a given item.</summary>
		/// <returns></returns>
		public IEnumerable<ContentItem> ChildPages(ContentItem item)
		{
			return Children(item, Content.Is.AccessiblePage());
		}

		/// <summary>Parts below the current item.</summary>
		/// <returns></returns>
		public IEnumerable<ContentItem> ChildParts()
		{
			return Children(Content.Is.Accessible() & Content.Is.Part());
		}

		/// <summary>Parts in a given zone below the current item.</summary>
		/// <param name="zoneName"></param>
		/// <returns></returns>
		public IEnumerable<ContentItem> ChildParts(string zoneName)
		{
			return Children(Content.Is.Accessible() & Content.Is.Part() & Content.Is.InZone(zoneName));
		}

		/// <summary>Descendants of the current item.</summary>
		/// <returns></returns>
		public IEnumerable<ContentItem> Descendants()
		{
			return N2.Find.EnumerateChildren(CurrentItem).Where(DefaultFilter);
		}

		/// <summary>Descendants of a given item.</summary>
		/// <param name="ancestor"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public IEnumerable<ContentItem> Descendants(ContentItem ancestor, ItemFilter filter = null)
		{
			return N2.Find.EnumerateChildren(ancestor).Where(filter ?? DefaultFilter);
		}

		/// <summary>Descendant pages of a given item.</summary>
		/// <param name="ancestor"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public IEnumerable<ContentItem> DescendantPages(ContentItem ancestor, ItemFilter filter = null)
		{
			return N2.Find.EnumerateChildren(ancestor).Where(Content.Is.Page()).Where(filter ?? DefaultFilter);
		}

		/// <summary>Siblings of the current item.</summary>
		/// <returns></returns>
		public IEnumerable<ContentItem> Siblings()
		{
			return Siblings(null, null);
		}

		/// <summary>Siblings of a given item.</summary>
		/// <param name="item"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public IEnumerable<ContentItem> Siblings(ContentItem sibling)
		{
			return Siblings(sibling, null);
		}

		/// <summary>Siblings of the current item.</summary>
		/// <param name="item"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public IEnumerable<ContentItem> Siblings(ItemFilter filter)
		{
			return Siblings(null, filter);
		}

		/// <summary>Siblings of a given item.</summary>
		/// <param name="item"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public IEnumerable<ContentItem> Siblings(ContentItem item, ItemFilter filter)
		{
			if (item == null) item = CurrentItem;
			if (item.Parent == null) return Enumerable.Empty<ContentItem>();
			UseMasterVersion(ref item);

			return item.Parent.GetChildren(filter ?? DefaultFilter);
		}

		/// <summary>The previous sibling among a given item's parent's children.</summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public ContentItem PreviousSibling(ContentItem item = null)
		{
			if (item == null) item = CurrentItem;
			UseMasterVersion(ref item);

			ContentItem previous = null;
			foreach (var sibling in Siblings(item))
			{
				if (sibling == item)
					return previous;
				
				previous = sibling;
			}
			return null;
		}

		/// <summary>The next sibling among a given item's parent's children.</summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public ContentItem NextSibling(ContentItem item = null)
		{
			if (item == null) item = CurrentItem;
			UseMasterVersion(ref item);

			bool next = false;
			foreach (var sibling in Siblings(item))
			{
				if (next)
					return sibling;
				if (sibling == item)
					next = true;
			}
			return null;
		}

		/// <summary>The level index of a given item.</summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int LevelOf(ContentItem item = null)
		{
			return Ancestors(item).Count();
		}

		/// <summary>The item at a given level from the start page.</summary>
		/// <param name="levelIndex"></param>
		/// <returns></returns>
		public ContentItem AncestorAtLevel(int levelIndex)
		{
			return Ancestors().Reverse().Skip(levelIndex).FirstOrDefault();
		}
		
		/// <summary>The parent of the current item (page or part).</summary>
		/// <returns></returns>
		public ContentItem Parent()
		{
			return Parent(CurrentItem);
		}

		/// <summary>The parent of a given item.</summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public ContentItem Parent(ContentItem item)
		{
			if (item == null) item = CurrentItem;
			if (item == StartPage) return null;
			UseMasterVersion(ref item);

			return item.Parent;
		}

		/// <summary>The parent of the current page.</summary>
		/// <returns></returns>
		public ContentItem PageParent()
		{
			return Parent(CurrentPage);
		}

		public ContentItem Path(string path, ContentItem startItem = null)
		{
			return (startItem ?? StartPage).FindPath(path).CurrentItem;
		}

		private void UseMasterVersion(ref ContentItem item)
		{
			if (item != null && item.VersionOf != null)
				item = item.VersionOf;
		}
	}
}