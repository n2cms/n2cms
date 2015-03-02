using System;
using System.Collections.Generic;
using System.Linq;
using N2.Definitions;
using N2.Engine;
using N2.Engine.Globalization;
using N2.Web;

namespace N2.Collections
{
    /// <summary>
    /// Simplifies traversing items in the content hierarchy.
    /// </summary>
    public class TraverseHelper
    {
        Func<IEngine> engine;
        FilterHelper filterIs;
        Func<PathData> pathGetter;
        ItemFilter defaultFilter;

        public TraverseHelper(Func<IEngine> engine, FilterHelper filter, Func<PathData> pathGetter)
        {
            this.engine = engine;
            this.filterIs = filter;
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
            get { return ClosestStartPage(CurrentItem); }
        }

        public ContentItem RootPage
        {
            get { return N2.Find.EnumerateParents(CurrentItem, null, true).LastOrDefault() ?? engine().Persister.Get(engine().Resolve<IHost>().CurrentSite.RootItemID); }
        }

        /// <summary>The default filter to apply to all results from this object.</summary>
        public ItemFilter DefaultFilter
        {
            get { return defaultFilter ?? (defaultFilter = filterIs.Accessible()); }
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
            TryMasterVersion(ref item);
            var lg = engine().Resolve<LanguageGatewaySelector>().GetLanguageGateway(item);
            return lg.FindTranslations(item).Select(i => lg.GetLanguage(i));
        }

        /// <summary>Ancestors of a given item.</summary>
        /// <param name="item"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable<ContentItem> Ancestors(ContentItem item = null, ItemFilter filter = null, ContentItem lastAncestor = null)
        {
            TryMasterVersion(ref item);
            return N2.Find.EnumerateParents(item ?? CurrentItem, lastAncestor ?? StartPage, true).Where(filter ?? DefaultFilter);
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
            TryMasterVersion(ref parent);
            
            return parent.Children.Where(filter ?? DefaultFilter);
        }

        /// <summary>Pages below a given item.</summary>
        /// <returns></returns>
        public IEnumerable<ContentItem> ChildPages(ContentItem parent = null, ItemFilter filter = null)
        {
            if (parent == null) parent = CurrentPage;
            if (filter == null) filter = this.filterIs.AccessiblePage();

            TryMasterVersion(ref parent);
            return (parent).Children.FindPages().Where(filter);
        }

        /// <summary>Pages below the current item suitable for display in a navigation (visible, published, accessible).</summary>
        /// <returns></returns>
        public IEnumerable<T> NavigatableChildPages<T>()
        {
            return NavigatableChildPages().OfType<T>();
        }

        /// <summary>Pages below the current item suitable for display in a navigation (visible, published, accessible).</summary>
        /// <returns></returns>
        public IEnumerable<ContentItem> NavigatableChildPages()
        {
            return NavigatableChildPages(CurrentPage);
        }

        /// <summary>Pages below the given item suitable for display in a navigation (visible, published, accessible).</summary>
        /// <returns></returns>
        public IEnumerable<ContentItem> NavigatableChildPages(ContentItem parent)
        {
            TryMasterVersion(ref parent);
            return (parent ?? CurrentPage).Children.FindNavigatablePages().Where(filterIs.Accessible());
        }

        /// <summary>Pages below the given item suitable for display in a navigation (visible, published, accessible).</summary>
        /// <returns></returns>
        public IEnumerable<T> NavigatableChildPages<T>(ContentItem parent)
        {
            TryMasterVersion(ref parent);
            return (parent ?? CurrentPage).Children.FindNavigatablePages().Where(filterIs.Accessible()).OfType<T>();
        }

        /// <summary>Parts below the current item.</summary>
        /// <returns></returns>
        public IEnumerable<ContentItem> ChildParts()
        {
            return Children(filterIs.Accessible() & filterIs.Part());
        }

        /// <summary>Parts below the current item.</summary>
        /// <returns></returns>
        public IEnumerable<T> ChildParts<T>()
        {
            return Children(filterIs.Accessible() & filterIs.Part()).OfType<T>();
        }

        /// <summary>Parts in a given zone below the current item.</summary>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        public IEnumerable<T> ChildParts<T>(string zoneName)
        {
            return ChildParts(zoneName).OfType<T>();
        }

        /// <summary>Parts in a given zone below the current item.</summary>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        public IEnumerable<ContentItem> ChildParts(string zoneName)
        {
            return ChildParts(CurrentItem, zoneName);
        }

        /// <summary>Parts in a given zone below the current item.</summary>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        public IEnumerable<ContentItem> ChildParts(ContentItem parent, string zoneName)
        {
            TryMasterVersion(ref parent);
            return (parent ?? CurrentItem).Children.FindParts(zoneName).Where(filterIs.Accessible());
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
            TryMasterVersion(ref ancestor);
            return N2.Find.EnumerateChildren(ancestor).Where(filter ?? DefaultFilter);
        }

        /// <summary>Descendant pages of a given item.</summary>
        /// <param name="ancestor"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable<ContentItem> DescendantPages(ContentItem ancestor = null, ItemFilter filter = null)
        {
            if (!TryCurrentPage(ref ancestor))
                return Enumerable.Empty<ContentItem>();
            TryMasterVersion(ref ancestor);
            return N2.Find.EnumerateChildren(ancestor).Where(this.filterIs.Page()).Where(filter ?? DefaultFilter);
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
            if (!TryCurrentItem(ref item))
                return Enumerable.Empty<ContentItem>();
            if (item.Parent == null) return Enumerable.Empty<ContentItem>();
            TryMasterVersion(ref item);

            return item.Parent.Children.Where(filter ?? DefaultFilter);
        }

        /// <summary>The previous sibling among a given item's parent's children.</summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ContentItem PreviousSibling(ContentItem item = null)
        {
            if (item == null) item = CurrentItem;
            TryMasterVersion(ref item);

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
            TryMasterVersion(ref item);

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
        public ContentItem AncestorAtLevel(int levelIndex, bool fallbackToClosest = false)
        {
            var ancestor = Ancestors().Reverse().Skip(levelIndex).FirstOrDefault();
            if (ancestor != null)
                return ancestor;

            if (!fallbackToClosest)
                return null;

            return Ancestors().Reverse().Take(levelIndex).Reverse().FirstOrDefault();
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
            TryMasterVersion(ref item);

            return item.Parent;
        }

        /// <summary>The parent of the current page.</summary>
        /// <returns></returns>
        public ContentItem PageParent()
        {
            return Parent(CurrentPage);
        }

        public PathData Path(string path, ContentItem startItem = null)
        {
            if (path == null)
                return null;
            var g = (startItem ?? engine().UrlParser.StartPage);
            return g == null ? null : g.FindPath(path);
        }

        /// <summary>Gets the item at of the specified type.</summary>
        /// <returns>An ancestor at the specified level.</returns>
        public static ContentItem ClosestOf<T>(ContentItem item) where T : class
        {
            return Closest<T>(item) as ContentItem;
        }

        /// <summary>Gets the item at of the specified type.</summary>
        /// <returns>An ancestor at the specified level.</returns>
        public static T Closest<T>(ContentItem item) where T : class
        {
            if (item == null)
                return null;

            TryMasterVersion(ref item);

            var typed = item as T;
            if (typed != null)
                return typed;

            if (item.VersionOf.HasValue)
                return Closest<T>(item.VersionOf);

            return Closest<T>(item.Parent);
        }

        /// <summary>Gets the closest start page ancestor of the given item.</summary>
        /// <param name="item">The item whose start page to get.</param>
        /// <returns>The closest start page node.</returns>
        public ContentItem ClosestStartPage(ContentItem item = null)
        {
            TryMasterVersion(ref item);
            var startPage = ClosestOf<IStartPage>(item ?? CurrentItem) ?? engine().UrlParser.StartPage;
            TryRedirect(ref startPage);
            return startPage;
        }

        public ContentItem ClosestPage(ContentItem item)
        {
            TryMasterVersion(ref item);
            if (item == null || item.IsPage)
                return item;
            return ClosestPage(item.Parent);
        }

        private bool TryRedirect(ref ContentItem page)
        {
            var redirect = page as IRedirect;
            if (redirect != null && redirect.RedirectTo != null)
            {
                page = redirect.RedirectTo;
                return true;
            }
            return false;
        }


        private bool TryCurrentItem(ref ContentItem item)
        {
            if (item != null)
                return true;

            item = CurrentItem;
            if (item != null)
                return true;

            return false;
        }

        private bool TryCurrentPage(ref ContentItem page)
        {
            if (page != null)
                return true;

            page = CurrentPage;
            if (page != null)
                return true;

            return false;
        }

        private static bool TryMasterVersion(ref ContentItem item)
        {
            if (item != null && item.VersionOf.HasValue)
            {
                item = item.VersionOf;
                return true;
            }
            return false;
        }
    }
}
