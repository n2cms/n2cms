using N2.Definitions.Static;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Edit
{
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
			PageSize = 10;
			DaysBeforeArchived = 365;
		}

		private DefinitionMap map;
		protected DefinitionMap Map
		{
			get { return map ?? (map = Context.Current.Resolve<DefinitionMap>()); }
			set { map = value; }
		}

		public GroupChildrenMode GroupBy { get; set; }
		public int PageSize { get; set; }
		public int DaysBeforeArchived { get; set; }
		public bool AllowDirectQuery { get; set; }

		//public PathData GetPath(ContentItem parent, string name, string argument)
		//{
		//	switch (GroupBy)
		//	{
		//		case GroupChildrenMode.RecentWithArchive:
		//			var archiveDate = Utility.CurrentTime().AddDays(-DaysBeforeArchived);
		//			return new PathData(new ChildGroupContainer(parent, "Archive", name));//, parent.Children.Where(c => c.Published <= archiveDate)));
		//		case GroupChildrenMode.Pages:
		//			int page = int.Parse(argument);
		//			return new PathData(new ChildGroupContainer(parent, page + "-" + (page + PageSize), "virtual-grouping/" + page));//, parent.Children.Skip(page * PageSize).Take(PageSize)));
		//		case GroupChildrenMode.PublishedYear:
		//			int year = int.Parse(argument);
		//			return new PathData(new ChildGroupContainer(parent, year.ToString(), "virtual-grouping/" + year));//, parent.Children.Where(c => c.Published.HasValue && c.Published.Value.Year == year)));
		//		case GroupChildrenMode.PublishedYearMonth:
		//			string yearMonth = argument;
		//			return new PathData(new ChildGroupContainer(parent, yearMonth, "virtual-grouping/" + yearMonth));//, parent.Children));
		//		case GroupChildrenMode.PublishedYearMonthDay:
		//			string date = argument;
		//			return new PathData(new ChildGroupContainer(parent, date, "virtual-grouping/" + date));//, parent.Children));
		//		case GroupChildrenMode.AlphabeticalIndex:
		//			string letter = argument;
		//			return new PathData(new ChildGroupContainer(parent, letter, "virtual-grouping/" + letter));//, parent.Children.Where(c => c.Title.StartsWith(letter, StringComparison.InvariantCultureIgnoreCase))));
		//		case GroupChildrenMode.Type:
		//			string type = argument;
		//			var definition = Map.GetOrCreateDefinition(Type.GetType(type));
		//			return new PathData(new ChildGroupContainer(parent, definition.Title, "virtual-grouping/" + type));//, parent.Children.Where(c => definition.ItemType == c.GetContentType())));
		//		case GroupChildrenMode.ZoneName:
		//			string zoneName = argument;
		//			return new PathData(new ChildGroupContainer(parent, zoneName, "virtual-grouping/" + zoneName));//, parent.Children.Where(c => c.ZoneName == zoneName)));
		//	}
		//	return PathData.Empty;
		//}

		public IEnumerable<ContentItem> FilterChildren(IEnumerable<ContentItem> previousChildren, Query query)
		{
			switch (GroupBy)
			{
				case GroupChildrenMode.RecentWithArchive:
					return ChildrenWithArchive(previousChildren, query);
				case GroupChildrenMode.Pages:
					return ChildrenByPage(previousChildren, query);
				case GroupChildrenMode.PublishedYear:
					return ChildrenByYear(previousChildren, query);
				case GroupChildrenMode.PublishedYearMonth:
					return ChildrenByYearMonth(previousChildren, query);
				case GroupChildrenMode.PublishedYearMonthDay:
					return ChildrenByYearMonthDay(previousChildren, query);
				case GroupChildrenMode.AlphabeticalIndex:
					return ChildrenByAlphabeticalIndex(previousChildren, query);
				case GroupChildrenMode.Type:
					return ChildrenByType(previousChildren, query);
				case GroupChildrenMode.ZoneName:
					return ChildrenByGroup(previousChildren, query);
				default:
					return previousChildren;
			}
		}

		private IEnumerable<ContentItem> ChildrenByGroup(IEnumerable<ContentItem> previousChildren, Query query)
		{
			if (AllowDirectQuery)
			{
				var zones = query.Parent.Children.FindZoneNames().ToList();

				return query.Parent.Children.FindPages()
					.Concat(zones.Where(z => !string.IsNullOrEmpty(z))
						.Select(z => new ChildGroupContainer(query.Parent, z, "virtual-grouping/" + z, () => query.Parent.Children.FindParts(z))));
			}

			return previousChildren.GroupBy(c => c.ZoneName)
				.OrderBy(g => g.Key)
				.SelectMany(g => g.Key == null ? (IEnumerable<ContentItem>)g : new ContentItem[] { new ChildGroupContainer(query.Parent, g.Key, "virtual-grouping/" + g.Key, () => g) });
		}

		private IEnumerable<ContentItem> ChildrenByType(IEnumerable<ContentItem> previousChildren, Query query)
		{
			if (AllowDirectQuery)
			{
				var types = query.Parent.Children.Select(query.AsParameters(), "class")
					.Select(r => (string)r["class"])
					.Distinct()
					.OrderBy(t => t);

				return types.Select(t => new ChildGroupContainer(query.Parent, t, "virtual-grouping/" + t, () => query.Parent.Children.Find(query.AsParameters() & Parameter.Equal("class", t))));
			}

			return previousChildren.GroupBy(c => c.GetContentType())
				.OrderBy(g => g.Key)
				.Select(g => new ChildGroupContainer(query.Parent, Map.GetOrCreateDefinition(g.Key).Title, "virtual-grouping/" + g.Key.FullName, () => g));
		}

		private IEnumerable<ContentItem> ChildrenByAlphabeticalIndex(IEnumerable<ContentItem> previousChildren, Query query)
		{
			if (AllowDirectQuery)
			{
				var letters = query.Parent.Children.Select(query.AsParameters(), "Title")
					.Select(r => (string)r["Title"])
					.Select(t => t.FirstOrDefault())
					.Distinct()
					.OrderBy(l => l);

				return letters.Select(l => new ChildGroupContainer(query.Parent, l.ToString().ToUpper(), "virtual-grouping/" + l, () => query.Parent.Children.Find(query.AsParameters() & Parameter.Like("Title", l + "%"))));
			}

			return previousChildren.GroupBy(c => string.IsNullOrEmpty(c.Title) ? '-' : c.Title.ToUpper().FirstOrDefault())
				.Select(g => new ChildGroupContainer(query.Parent, g.Key.ToString(), "virtual-grouping/" + g.Key, () => g));
		}

		private IEnumerable<ContentItem> ChildrenByYearMonthDay(IEnumerable<ContentItem> previousChildren, Query query)
		{
			if (AllowDirectQuery)
			{
				var dates = query.Parent.Children.Select(query.AsParameters(), "Published")
					.Select(r => (DateTime?)r["Published"])
					.Select(p => p.HasValue ? (DateTime?)p.Value.Date : null)
					.Distinct()
					.OrderByDescending(d => d);
				return dates.Select(ym => new ChildGroupContainer(query.Parent, ym.HasValue ? ym.Value.ToShortDateString() : "-", "virtual-grouping/" + (ym.HasValue ? ym.Value.ToString("yyyy-MM-dd") : "-"), () => query.Parent.Children.Find(query.AsParameters() & (ym.HasValue ? (Parameter.GreaterOrEqual("Published", ym.Value) & Parameter.LessThan("Published", ym.Value.AddDays(1))) : Parameter.IsNull("Published")))));
			}

			return previousChildren.GroupBy(c => c.Published.HasValue ? c.Published.Value.Date.ToShortDateString() : "-")
				.Select(g => new ChildGroupContainer(query.Parent, g.Key, "virtual-grouping/" + g.Key, () => g));
		}

		private IEnumerable<ContentItem> ChildrenByYearMonth(IEnumerable<ContentItem> previousChildren, Query query)
		{
			if (AllowDirectQuery)
			{
				var yearsMonths = query.Parent.Children.Select(query.AsParameters(), "Published")
					.Select(r => (DateTime?)r["Published"])
					.Select(p => p.HasValue ? (DateTime?)new DateTime(p.Value.Year, p.Value.Month, 1) : null)
					.Distinct()
					.OrderByDescending(d => d);
				return yearsMonths.Select(ym => new ChildGroupContainer(query.Parent, ToString(ym), "virtual-grouping/" + ToString(ym), () => query.Parent.Children.Find(query.AsParameters() & (ym.HasValue ? (Parameter.GreaterOrEqual("Published", ym.Value) & Parameter.LessThan("Published", ym.Value.AddMonths(1))) : Parameter.IsNull("Published")))));
			}

			return previousChildren.GroupBy(c => c.Published.HasValue ? c.Published.Value.Date.ToString("yyyy-MM") : "-")
				.Select(g => new ChildGroupContainer(query.Parent, g.Key, "virtual-grouping/" + g.Key, () => g));
		}

		private static string ToString(DateTime? ym)
		{
			return ym.HasValue ? ym.Value.ToString("yyyy-MM") : "-";
		}

		private IEnumerable<ContentItem> ChildrenByYear(IEnumerable<ContentItem> previousChildren, Query query)
		{
			if (AllowDirectQuery)
			{
				var years = query.Parent.Children
					.Select(query.AsParameters(), "Published")
					.Select(r => (DateTime?)r["Published"])
					.Select(p => p.HasValue ? p.Value.Year.ToString() : "-")
					.Distinct()
					.OrderByDescending(d => d);

				return years.Select(y => new ChildGroupContainer(query.Parent, y, "virtual-grouping/" + y, () => query.Parent.Children.Find(query.AsParameters() & (y != "-" ? (Parameter.GreaterOrEqual("Published", new DateTime(int.Parse(y), 1, 1)) & Parameter.LessThan("Published", new DateTime(int.Parse(y) + 1, 1, 1))) : Parameter.IsNull("Published")))));
			}

			return previousChildren.GroupBy(c => c.Published.HasValue ? c.Published.Value.Date.ToString("yyyy") : "-")
				.Select(g => new ChildGroupContainer(query.Parent, g.Key, "virtual-grouping/" + g.Key, () => g));
		}

		private IEnumerable<ContentItem> ChildrenByPage(IEnumerable<ContentItem> previousChildren, Query query)
		{
			if (AllowDirectQuery)
			{
				var count = query.Parent.Children.FindCount(query.AsParameters());
				// 0/2 - > 0
				// 1/2 - > 1
				// 2/2 - > 1
				// 3/2 - > 2

				return Enumerable.Range(0, (count + PageSize - 1) / PageSize)
					.Select(i => new ChildGroupContainer(query.Parent, (i * PageSize + 1) + "-" + (i * PageSize + PageSize), "virtual-grouping/" + i, () => query.Parent.Children.Find(query.AsParameters().Skip(i * PageSize).Take(PageSize))));
			}

			int pageIndex = 0;
			return previousChildren
				.GroupBy(c => pageIndex++ / PageSize)
				.Select(g => new ChildGroupContainer(query.Parent, (g.Key * PageSize + 1) + "-" + (g.Key * PageSize + PageSize), "virtual-grouping/" + g.Key, () => g))
				.ToList();
		}

		private IEnumerable<ContentItem> ChildrenWithArchive(IEnumerable<ContentItem> previousChildren, Query query)
		{
			var archiveDate = Utility.CurrentTime().AddDays(-DaysBeforeArchived);

			if (AllowDirectQuery)
			{
				var unarchived = query.Parent.Children.Find(query.AsParameters() & (Parameter.GreaterOrEqual("Published", archiveDate) | Parameter.IsNull("Published")));

				var archivedQuery = query.AsParameters() & Parameter.LessThan("Published", archiveDate);
				if (query.Parent.Children.FindCount(archivedQuery) > 0)
					return unarchived.Concat(new ContentItem[] { new ChildGroupContainer(query.Parent, "Archive", "virtual-grouping/archive", () => query.Parent.Children.Find(archivedQuery)) });
				
				return unarchived;
			}

			return previousChildren.Where(c => c.Published == null || archiveDate < c.Published)
				.Concat(new[] { new ChildGroupContainer(query.Parent, "Archive", "virtual-grouping/archive", () => previousChildren.Where(c => c.Published <= archiveDate)) });
		}

		public void Set(DefinitionMap dependency)
		{
			Map = dependency;
		}
	}
}