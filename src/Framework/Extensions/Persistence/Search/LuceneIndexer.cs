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

namespace N2.Persistence.Search
{
	/// <summary>
	/// Wraps the usage of lucene to index content items.
	/// </summary>
	[Service(typeof(IIndexer), Replaces = typeof(EmptyIndexer), Configuration = "lucene")]
	public class LuceneIndexer : IIndexer
	{
		private readonly Engine.Logger<LuceneIndexer> logger;
		LuceneAccesor accessor;
		TextExtractor extractor;

		public LuceneIndexer(LuceneAccesor accessor, TextExtractor extractor)
		{
		    this.accessor = accessor;
			this.extractor = extractor;
		}



		/// <summary>Clears the index.</summary>
		public virtual void Clear()
		{
			logger.Debug("Clearing index");

			if (accessor.IndexExists())
			{
				accessor.ClearLock();
				var w = accessor.GetWriter();
				if (w.NumDocs() > 0)
				{
					w.DeleteAll();
					w.Commit();
					accessor.RecreateSearcher();
				}
				accessor.ClearLock();
			}
		}

		/// <summary>Unlocks the index.</summary>
		public virtual void Unlock()
		{
			logger.Debug("Unlocking index");
			accessor.GetDirectory().ClearLock("write.lock");
		}

		/// <summary>Optimizes the index.</summary>
		public virtual void Optimize()
		{
			logger.Debug("Optimizing index");

			if (accessor.IndexExists())
			{
				var d = accessor.GetDirectory();
				var iw = accessor.GetWriter();
				if (iw.NumDocs() > 0)
				{
					iw.Optimize(true);
					iw.Commit();
				}
			}
		}

		/// <summary>Updates the index with the given item.</summary>
		/// <param name="item">The item containing content to be indexed.</param>
		public virtual void Update(ContentItem item)
		{
			if(item == null || item.ID == 0)
				return;

			logger.Debug("Updating item #" + item.ID);

			if (!item.IsPage)
			    Update(Find.ClosestPage(item));

			lock (accessor)
			{
				var iw = accessor.GetWriter();

				if (!extractor.IsIndexable(item))
					return;

				var doc = CreateDocument(item);
				if (doc == null)
					return;
				iw.UpdateDocument(new Term(Properties.ID, item.ID.ToString()), doc);
				iw.Commit();
				accessor.RecreateSearcher();
			}
		}

		/// <summary>Delets an item from the index and any descendants.</summary>
		/// <param name="itemID">The id of the item to delete.</param>
		public virtual void Delete(int itemID)
		{
			logger.Debug("Deleting item #" + itemID);

			lock (accessor)
			{
				var iw = accessor.GetWriter();
				var s = accessor.GetSearcher();
				string trail = GetTrail(s, new Term(Properties.ID, itemID.ToString()));
				if (trail == null)
					return; // not indexed

				var query = new PrefixQuery(new Term(Properties.Trail, trail));
				iw.DeleteDocuments(query);
				iw.Commit();
				accessor.RecreateSearcher();
			}
		}

		private string GetTrail(IndexSearcher s, Term t)
		{
			return s.Search(new TermQuery(t), 1)
				.scoreDocs
				.Select(d => s.Doc(d.doc).Get(Properties.Trail))
				.FirstOrDefault();
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

		public virtual Document CreateDocument(ContentItem item)
		{
			var doc = new Document();
			doc.Add(new Field(Properties.ID, item.ID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field(Properties.Title, item.Title ?? "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(Properties.Name, item.Name ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(Properties.SavedBy, item.SavedBy ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(Properties.Created, DateTools.DateToString(item.Created.ToUniversalTime(), DateTools.Resolution.SECOND), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(Properties.Updated, DateTools.DateToString(item.Updated.ToUniversalTime(), DateTools.Resolution.SECOND), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(Properties.Published, item.Published.HasValue ? DateTools.DateToString(item.Published.Value.ToUniversalTime(), DateTools.Resolution.SECOND) : "", Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(Properties.Expires, item.Expires.HasValue ? DateTools.DateToString(item.Expires.Value.ToUniversalTime(), DateTools.Resolution.SECOND) : "", Field.Store.YES, Field.Index.NOT_ANALYZED));
			if (item.IsPage)
			{
				try
				{
					doc.Add(new Field(Properties.Url, item.Url, Field.Store.YES, Field.Index.NOT_ANALYZED));
				}
				catch (TemplateNotFoundException ex)
				{
					logger.Warn("Failed to retrieve Url on " + item, ex);
					return null;
				}
			}
            doc.Add(new Field(Properties.Path, item.Path ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(Properties.AncestralTrail, (item.AncestralTrail ?? ""), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(Properties.Trail, Utility.GetTrail(item), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(Properties.AlteredPermissions, ((int)item.AlteredPermissions).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(Properties.State, ((int)item.State).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(Properties.IsPage, item.IsPage.ToString().ToLower(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(Properties.Roles, GetRoles(item), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(Properties.Types, GetTypes(item), Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field(Properties.Language, GetLanguage(item), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field(Properties.Visible, item.Visible.ToString().ToLower(), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field(Properties.SortOrder, item.SortOrder.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

			var texts = extractor.Extract(item);
			foreach (var t in texts)
				doc.Add(new Field("Detail." + t.Name, t.TextContent, Field.Store.NO, Field.Index.ANALYZED));
			string text = extractor.Join(texts);
			doc.Add(new Field("Text", text, Field.Store.NO, Field.Index.ANALYZED));
			
			using(var sw = new StringWriter())
			{
				AppendPartsRecursive(item, sw);
				doc.Add(new Field("PartsText", sw.ToString(), Field.Store.NO, Field.Index.ANALYZED));
			}
			return doc;
		}

		private string GetLanguage(ContentItem item)
		{
			var language = Find.Closest<ILanguage>(item);
			if(language == null)
				return "";

			return language.LanguageCode ?? "";
		}

		private static string GetTypes(ContentItem item)
		{
			string types = string.Join(" ",
				Utility.GetBaseTypesAndSelf(item.GetContentType())
				.Union(item.GetContentType().GetInterfaces()
					.Where(t => t.GetCustomAttributes(typeof(SearchableTypeAttribute), false).Any())).Select(t => t.Name).ToArray());
			return types;
		}

		private static string GetRoles(ContentItem item)
		{
			string roles = string.Join(" ", item.AuthorizedRoles.Select(r => r.Role).ToArray());
			if (string.IsNullOrEmpty(roles))
				roles = "Everyone";
			return roles;
		}

		private void AppendPartsRecursive(ContentItem parent, StringWriter partTexts)
		{
			foreach (var part in parent.Children.FindParts())
			{
				partTexts.WriteLine(part.Title);
				string text = extractor.Join(extractor.Extract(part));
				partTexts.WriteLine(text);

				AppendPartsRecursive(part, partTexts);
			}
		}

		private static string CombineTexts(IDictionary<string, string> texts)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var value in texts.Values)
				sb.AppendLine(value);
			return sb.ToString();
		}

        public IndexStatistics GetStatistics()
        {
            return new IndexStatistics
            {
                TotalDocuments = accessor.GetWriter().GetReader().NumDocs()
            };
        }
    }
}
