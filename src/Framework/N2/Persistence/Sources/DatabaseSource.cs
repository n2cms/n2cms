using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web;
using System.Diagnostics;

namespace N2.Persistence.Sources
{
	[ContentSource]
	public class DatabaseSource : SourceBase
	{
		private IHost host;
		private IContentItemRepository repository;

		public DatabaseSource(IHost host, IContentItemRepository repository)
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




		public override ContentItem Get(object id)
		{
			return repository.Get(id);
		}

		public override void Save(ContentItem item)
		{
			using (var tx = repository.BeginTransaction())
			{
				// update updated date unless it's a version being saved
				if (!item.VersionOf.HasValue)
					item.Updated = Utility.CurrentTime();
				// empty string names not allowed, null is replaced with item id
				if (string.IsNullOrEmpty(item.Name))
					item.Name = null;

				item.AddTo(item.Parent);
				
				// make sure the ordering is the same next time these siblings are loaded
				var unsavedItems = item.Parent.EnsureChildrenSortOrder();
				foreach (var itemToSave in unsavedItems.Union(new [] { item }))
				{
					repository.SaveOrUpdate(itemToSave);
				}

				// ensure a name, fallback to id
				if (string.IsNullOrEmpty(item.Name))
				{
					item.Name = item.ID.ToString();
					repository.SaveOrUpdate(item);
				}

				tx.Commit();
			}
		}

		public override void Delete(ContentItem item)
		{
			using (var tx = repository.BeginTransaction())
			{
				// delete inbound references, these would cuase fk violation in the database
				repository.RemoveReferencesToRecursive(item);

				DeleteRecursive(item, item);

				tx.Commit();
			}
		}

		private void DeleteRecursive(ContentItem topItem, ContentItem itemToDelete)
		{
			DeletePreviousVersions(itemToDelete);

			try
			{
				Trace.Indent();
				List<ContentItem> children = new List<ContentItem>(itemToDelete.Children);
				foreach (ContentItem child in children)
					DeleteRecursive(topItem, child);
			}
			finally
			{
				Trace.Unindent();
			}

			itemToDelete.AddTo(null);

			Trace.TraceInformation("DatabaseSource.DeleteRecursive " + itemToDelete);
			repository.Delete(itemToDelete);
		}

		private void DeletePreviousVersions(ContentItem itemNoMore)
		{
			var previousVersions = repository.Find("VersionOf.ID", itemNoMore.ID);

			int count = 0;
			foreach (ContentItem version in previousVersions)
			{
				repository.Delete(version);
				count++;
			}

			Trace.TraceInformation("DatabaseSource.DeletePreviousVersions " + count + " of " + itemNoMore);
		}

		public override ContentItem Move(ContentItem source, ContentItem destination)
		{
			using (var tx = repository.BeginTransaction())
			{
				Trace.TraceInformation("ContentPersister.MoveAction " + source + " to " + destination);
				source.AddTo(destination);
				Save(source);
				tx.Commit();
			}
			return source;
		}

		public override ContentItem Copy(ContentItem source, ContentItem destination)
		{
			Trace.TraceInformation("ContentPersister.Copy " + source + " to " + destination);
			ContentItem cloned = source.Clone(includeChildren:true);
			if (cloned.Name == source.ID.ToString())
				cloned.Name = null;
			cloned.Parent = destination;

			Save(cloned);

			return cloned;
		}
	}
}
