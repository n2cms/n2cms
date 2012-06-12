using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web;
using N2.Collections;

namespace N2.Persistence.Sources
{
	[ContentSource]
	public class DatabaseSource : SourceBase
	{
		private IHost host;
		private IRepository<ContentItem> repository;

		public DatabaseSource(IHost host, IRepository<ContentItem> repository)
		{
			this.host = host;
			this.repository = repository;
		}

		public override IEnumerable<ContentItem> AppendChildren(IEnumerable<ContentItem> previousChildren, Query query)
		{
			IEnumerable<ContentItem> items;
			if (!query.OnlyPages.HasValue)
				items = query.Parent.Children;
			else if (query.OnlyPages.Value)
				items = query.Parent.Children.FindPages();
			else
				items = query.Parent.Children.FindParts();

			if (query.Filter != null)
				items = items.Where(query.Filter);

			return previousChildren.Union(items);
		}

		public override bool HasChildren(Query query)
		{
			if (query.Parent.ChildState == CollectionState.IsEmpty)
				return false;

			bool onlyPages = query.OnlyPages ?? false;

			if (query.Parent.ChildState.IsAny(CollectionState.ContainsVisiblePublicPages | CollectionState.ContainsHiddenPublicPages))
				return true;
			if (!onlyPages && query.Parent.ChildState.IsAny(CollectionState.ContainsPublicParts))
				return true;
			if (query.Filter != null && query.Parent.ChildState.IsAny(CollectionState.ContainsHiddenSecuredPages | CollectionState.ContainsVisibleSecuredPages))
				return true;
			if (!onlyPages && query.Filter != null && query.Parent.ChildState.IsAny(CollectionState.ContainsSecuredParts))
				return true;

			return base.HasChildren(query);
		}

		public override bool IsProvidedBy(ContentItem item)
		{
			return true;
		}

		public override PathData ResolvePath(string path)
		{
			var root = repository.Get(host.CurrentSite.RootItemID);
			return ResolvePath(root, path);
		}

		public override PathData ResolvePath(ContentItem startingPoint, string path)
		{
			return startingPoint.FindPath(path);
		}



		public override void Save(ContentItem item)
		{
			throw new NotImplementedException();
		}

		public override void Delete(ContentItem item)
		{
			throw new NotImplementedException();
		}

		public override void Move(ContentItem source, ContentItem destination)
		{
			throw new NotImplementedException();
		}

		public override ContentItem Copy(ContentItem source, ContentItem destination)
		{
			throw new NotImplementedException();
		}
	}
}
