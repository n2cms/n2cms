using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using N2.Engine;
using N2.Engine.Globalization;
using N2.Web;
using System;
using Lucene.Net.Store;

namespace N2.Persistence.Search
{
	/// <summary>
	/// Wraps the usage of lucene to index content items.
	/// </summary>
	[Service(typeof(IContentIndexer), Replaces = typeof(EmptyIndexer), Configuration = "lucene")]
	public class LuceneContentIndexer : IContentIndexer
	{
		private TextExtractor extractor;
		private IIndexer indexer;

		public LuceneContentIndexer(IIndexer indexer, TextExtractor extractor)
		{
			this.extractor = extractor;
			this.indexer = indexer;
		}

		/// <summary>Clears the index.</summary>
		public virtual void Clear()
		{
			indexer.Clear();
		}

		/// <summary>Unlocks the index.</summary>
		public virtual void Unlock()
		{
			indexer.Unlock();
		}

		/// <summary>Optimizes the index.</summary>
		public virtual void Optimize()
		{
			indexer.Optimize();
		}

		/// <summary>Updates the index with the given item.</summary>
		/// <param name="item">The item containing content to be indexed.</param>
		public virtual void Update(ContentItem item)
		{
			if (!IsIndexable(item))
				return;

			if (!item.IsPage)
			    Update(N2.Content.Traverse.ClosestPage(item));

			Update(extractor.CreateDocument(item));
		}

		public virtual bool IsIndexable(ContentItem item)
		{
			if (item == null || item.ID == 0)
				return false;
			return extractor.IsIndexable(item);
		}

		/// <summary>Delets an item from the index and any descendants.</summary>
		/// <param name="itemID">The id of the item to delete.</param>
		public virtual void Delete(int itemID)
		{
			indexer.Delete(itemID);
		}

		public IndexableDocument CreateDocument(ContentItem item)
		{
			return extractor.CreateDocument(item);
		}

		public void Update(IndexableDocument document)
		{
			indexer.Update(document);
		}

        public IndexStatistics GetStatistics()
        {
			return indexer.GetStatistics();
        }

		public static class Properties
		{
			public const string ID = "ID";
			public const string Title = "Title";
			public const string Name = "Name";
			public const string SavedBy = "SavedBy";
			public const string Created = "Created";
			public const string Updated = "Updated";
			public const string Published = "Published";
			public const string Expires = "Expires";
			public const string Url = "Url";
			public const string Path = "Path";
			public const string AncestralTrail = "AncestralTrail";
			public const string Trail = "Trail";
			public const string AlteredPermissions = "AlteredPermissions";
			public const string State = "State";
			public const string IsPage = "IsPage";
			public const string Roles = "Roles";
			public const string Types = "Types";
			public const string Language = "Language";
			public const string Visible = "Visible";
			public const string SortOrder = "SortOrder";

			public static HashSet<string> All = new HashSet<string> { ID, Title, Name, SavedBy, Created, Updated, Published, Expires, Url, Path, AncestralTrail, Trail, AlteredPermissions, State, IsPage, Roles, Types, Language, Visible };
		}
	}
}
