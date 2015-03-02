using N2.Definitions.Static;
using N2.Details;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Definitions
{
    public delegate ContentItem GroupFactoryDelegate(ContentItem parent, string title, string name, Func<IEnumerable<ContentItem>> childFactory);

    public class GroupChildrenAttribute : Attribute, IInjectable<DefinitionMap>
    {
        public GroupChildrenAttribute()
            : this(GroupChildrenMode.Pages)
        {
        }

        public GroupChildrenAttribute(GroupChildrenMode groupBy)
        {
            GroupBy = groupBy;
            AllowDirectQuery = true;
            PageSize = 25;
            StartPagingTreshold = 100;
            DaysBeforeArchived = 365;
	        MinGroupSize = 1;
        }

        private DefinitionMap map;
        protected DefinitionMap Map
        {
            get { return map ?? (map = Context.Current.Resolve<DefinitionMap>()); }
            set { map = value; }
        }

        public GroupChildrenMode GroupBy { get; set; }
        public int StartPagingTreshold { get; set; }
        public int PageSize { get; set; }
        public int DaysBeforeArchived { get; set; }
        public bool AllowDirectQuery { get; set; }
		/// <summary>
		/// Minimum size for a single group. If there aren't enough similar items to form a group, then those items won't be grouped and will instead be shown individually (alongside any other groups).
		/// </summary>
		public int MinGroupSize { get; set; }

        public IEnumerable<ContentItem> FilterChildren(IEnumerable<ContentItem> previousChildren, Query query, GroupFactoryDelegate groupFactory)
        {
            switch (GroupBy)
            {
                case GroupChildrenMode.RecentWithArchive:
                    return ChildrenWithArchive(previousChildren, query, groupFactory);
                case GroupChildrenMode.Pages:
                    return ChildrenByPage(previousChildren, query, groupFactory);
                case GroupChildrenMode.PagesAfterTreshold:
                    return ChildrenUntilTresholdThenPages(previousChildren, query, groupFactory);
                case GroupChildrenMode.PublishedYear:
                    return ChildrenByYear(previousChildren, query, groupFactory);
                case GroupChildrenMode.PublishedYearMonth:
                    return ChildrenByYearMonth(previousChildren, query, groupFactory);
                case GroupChildrenMode.PublishedYearMonthDay:
                    return ChildrenByYearMonthDay(previousChildren, query, groupFactory);
                case GroupChildrenMode.AlphabeticalIndex:
                    return ChildrenByAlphabeticalIndex(previousChildren, query, groupFactory);
                case GroupChildrenMode.Type:
                    return ChildrenByType(previousChildren, query, groupFactory);
                case GroupChildrenMode.ZoneName:
                    return ChildrenByGroup(previousChildren, query, groupFactory);
                default:
                    return previousChildren;
            }
        }

	    private IEnumerable<IGrouping<TKey, T>> GroupByWithMinSize<T, TKey>(IEnumerable<T> enumerable, Func<T, TKey> groupSelector)
	    {
		    var groups = enumerable.GroupBy(groupSelector).ToList();

		    return groups;
	    }

        private IEnumerable<ContentItem> ChildrenByGroup(IEnumerable<ContentItem> previousChildren, Query query, GroupFactoryDelegate childFactory)
        {
            if (AllowDirectQuery)
            {
                var zones = query.Parent.Children.FindZoneNames().ToList();

                return query.Parent.Children.FindPages()
                    .Concat(zones.Where(z => !string.IsNullOrEmpty(z))
                        .Select(z => childFactory(query.Parent, z, "virtual-grouping/" + z, () => query.Parent.Children.FindParts(z))));
            }

            return GroupByWithMinSize(previousChildren, c => c.ZoneName)
                .OrderBy(g => g.Key)
                .SelectMany(g => g.Key == null ? (IEnumerable<ContentItem>)g : new ContentItem[] { childFactory(query.Parent, g.Key, "virtual-grouping/" + g.Key, () => g) });
        }

        private IEnumerable<ContentItem> ChildrenByType(IEnumerable<ContentItem> previousChildren, Query query, GroupFactoryDelegate childFactory)
        {
            if (AllowDirectQuery)
            {
	            var types = query.Parent.Children.Select(query.AsParameters(), "class")
		            .Select(r => (string) r["class"])
		            .Distinct()
		            .OrderBy(t => t);

                return types.Select(t => childFactory(query.Parent, t, "virtual-grouping/" + t, () => query.Parent.Children.Find(query.AsParameters() & Parameter.Equal("class", t))));
            }

            return GroupByWithMinSize(previousChildren, c => c.GetContentType()) // previousChildren.GroupBy(c => c.GetContentType())
                .OrderBy(g => g.Key)
                .Select(g => childFactory(query.Parent, Map.GetOrCreateDefinition(g.Key).Title, "virtual-grouping/" + g.Key.FullName, () => g));
        }

        private IEnumerable<ContentItem> ChildrenByAlphabeticalIndex(IEnumerable<ContentItem> previousChildren, Query query, GroupFactoryDelegate childFactory)
        {
            if (AllowDirectQuery)
            {
                var letters = query.Parent.Children.Select(query.AsParameters(), "Title")
                    .Select(r => (string)r["Title"])
                    .Select(t => t.FirstOrDefault())
                    .Distinct()
                    .OrderBy(l => l);

                return letters.Select(l => childFactory(query.Parent, l.ToString().ToUpper(), "virtual-grouping/" + l, () => query.Parent.Children.Find(query.AsParameters() & Parameter.Like("Title", l + "%"))));
            }

			return GroupByWithMinSize(previousChildren, c => string.IsNullOrEmpty(c.Title) ? '-' : c.Title.ToUpper().FirstOrDefault())
                .Select(g => childFactory(query.Parent, g.Key.ToString(), "virtual-grouping/" + g.Key, () => g));
        }

        private IEnumerable<ContentItem> ChildrenByYearMonthDay(IEnumerable<ContentItem> previousChildren, Query query, GroupFactoryDelegate childFactory)
        {
            if (AllowDirectQuery)
            {
                var dates = query.Parent.Children.Select(query.AsParameters(), "Published")
                    .Select(r => (DateTime?)r["Published"])
                    .Select(p => p.HasValue ? (DateTime?)p.Value.Date : null)
                    .Distinct()
                    .OrderByDescending(d => d);
                return dates.Select(ym => childFactory(query.Parent, ym.HasValue ? ym.Value.ToShortDateString() : "-", "virtual-grouping/" + (ym.HasValue ? ym.Value.ToString("yyyy-MM-dd") : "-"), () => query.Parent.Children.Find(query.AsParameters() & (ym.HasValue ? (Parameter.GreaterOrEqual("Published", ym.Value) & Parameter.LessThan("Published", ym.Value.AddDays(1))) : Parameter.IsNull("Published")))));
            }

			return GroupByWithMinSize(previousChildren, c => c.Published.HasValue ? c.Published.Value.Date.ToShortDateString() : "-")
                .Select(g => childFactory(query.Parent, g.Key, "virtual-grouping/" + g.Key, () => g));
        }

        private IEnumerable<ContentItem> ChildrenByYearMonth(IEnumerable<ContentItem> previousChildren, Query query, GroupFactoryDelegate childFactory)
        {
            if (AllowDirectQuery)
            {
                var yearsMonths = query.Parent.Children.Select(query.AsParameters(), "Published")
                    .Select(r => (DateTime?)r["Published"])
                    .Select(p => p.HasValue ? (DateTime?)new DateTime(p.Value.Year, p.Value.Month, 1) : null)
                    .Distinct()
                    .OrderByDescending(d => d);
                return yearsMonths.Select(ym => childFactory(query.Parent, ToString(ym), "virtual-grouping/" + ToString(ym), () => query.Parent.Children.Find(query.AsParameters() & (ym.HasValue ? (Parameter.GreaterOrEqual("Published", ym.Value) & Parameter.LessThan("Published", ym.Value.AddMonths(1))) : Parameter.IsNull("Published")))));
            }

			return GroupByWithMinSize(previousChildren, c => c.Published.HasValue ? c.Published.Value.Date.ToString("yyyy-MM") : "-")
                .Select(g => childFactory(query.Parent, g.Key, "virtual-grouping/" + g.Key, () => g));
        }

        private static string ToString(DateTime? ym)
        {
            return ym.HasValue ? ym.Value.ToString("yyyy-MM") : "-";
        }

        private IEnumerable<ContentItem> ChildrenByYear(IEnumerable<ContentItem> previousChildren, Query query, GroupFactoryDelegate childFactory)
        {
            if (AllowDirectQuery)
            {
                var years = query.Parent.Children
                    .Select(query.AsParameters(), "Published")
                    .Select(r => (DateTime?)r["Published"])
                    .Select(p => p.HasValue ? p.Value.Year.ToString() : "-")
                    .Distinct()
                    .OrderByDescending(d => d);

                return years.Select(y => childFactory(query.Parent, y, "virtual-grouping/" + y, () => query.Parent.Children.Find(query.AsParameters() & (y != "-" ? (Parameter.GreaterOrEqual("Published", new DateTime(int.Parse(y), 1, 1)) & Parameter.LessThan("Published", new DateTime(int.Parse(y) + 1, 1, 1))) : Parameter.IsNull("Published")))));
            }

			return GroupByWithMinSize(previousChildren, c => c.Published.HasValue ? c.Published.Value.Date.ToString("yyyy") : "-")
                .Select(g => childFactory(query.Parent, g.Key, "virtual-grouping/" + g.Key, () => g));
        }

        private IEnumerable<ContentItem> ChildrenUntilTresholdThenPages(IEnumerable<ContentItem> previousChildren, Query query, GroupFactoryDelegate childFactory)
        {
            if (AllowDirectQuery)
            {
                int dbCount = query.Parent.Children.FindCount(query.AsParameters());

                if (dbCount < StartPagingTreshold)
                    return previousChildren;
            
                var unpaged = query.Parent.Children.Find(query.AsParameters().Take(StartPagingTreshold));

                return unpaged.Concat(
                    Enumerable.Range(0, (dbCount - StartPagingTreshold + PageSize - 1) / PageSize)
                    .Select(i => childFactory(query.Parent, (StartPagingTreshold + i * PageSize + 1) + "-" + (StartPagingTreshold + i * PageSize + PageSize), "virtual-grouping/" + i, () => query.Parent.Children.Find(query.AsParameters().Skip(StartPagingTreshold + i * PageSize).Take(PageSize)))));
            }

            var prevChildren = previousChildren as ContentItem[] ?? previousChildren.ToArray();
            var page = prevChildren.Take(StartPagingTreshold).ToList();
	        if (page.Count < StartPagingTreshold)
		        return page;
			return page.Concat(
                    Enumerable.Range(0, (prevChildren.Length - StartPagingTreshold + PageSize - 1) / PageSize)
                        .Select(i => childFactory(query.Parent, (StartPagingTreshold + i * PageSize + 1) + "-" + (StartPagingTreshold + i * PageSize + PageSize), "virtual-grouping/" + i, () => prevChildren.Skip(StartPagingTreshold + i * PageSize).Take(PageSize))));
        }

        private IEnumerable<ContentItem> ChildrenByPage(IEnumerable<ContentItem> previousChildren, Query query, GroupFactoryDelegate childFactory)
        {
            if (AllowDirectQuery)
            {
                var count = query.Parent.Children.FindCount(query.AsParameters());
                return Enumerable.Range(0, (count + PageSize - 1) / PageSize)
                    .Select(i => childFactory(query.Parent, (i * PageSize + 1) + "-" + (i * PageSize + PageSize), "virtual-grouping/" + i, () => query.Parent.Children.Find(query.AsParameters().Skip(i * PageSize).Take(PageSize))));
            }

            int pageIndex = 0;
            return previousChildren
                .GroupBy(c => pageIndex++ / PageSize)
                .Select(g => childFactory(query.Parent, (g.Key * PageSize + 1) + "-" + (g.Key * PageSize + PageSize), "virtual-grouping/" + g.Key, () => g))
                .ToList();
        }

        private IEnumerable<ContentItem> ChildrenWithArchive(IEnumerable<ContentItem> previousChildren, Query query, GroupFactoryDelegate childFactory)
        {
            var archiveDate = Utility.CurrentTime().AddDays(-DaysBeforeArchived);

            if (AllowDirectQuery)
            {
                var unarchived = query.Parent.Children.Find(query.AsParameters() & (Parameter.GreaterOrEqual("Published", archiveDate) | Parameter.IsNull("Published")));

                var archivedQuery = query.AsParameters() & Parameter.LessThan("Published", archiveDate);
                if (query.Parent.Children.FindCount(archivedQuery) > 0)
                    return unarchived.Concat(new ContentItem[] { childFactory(query.Parent, "Archive", "virtual-grouping/archive", () => query.Parent.Children.Find(archivedQuery)) });
                
                return unarchived;
            }

            return previousChildren.Where(c => c.Published == null || archiveDate < c.Published)
                .Concat(new[] { childFactory(query.Parent, "Archive", "virtual-grouping/archive", () => previousChildren.Where(c => c.Published <= archiveDate)) });
        }

        public void Set(DefinitionMap dependency)
        {
            Map = dependency;
        }
    }
}
