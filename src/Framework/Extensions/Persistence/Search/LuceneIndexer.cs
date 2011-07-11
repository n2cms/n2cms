using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Plugin;
using N2.Engine;
using N2.Plugin.Scheduling;
using Lucene.Net.Store;
using System.IO;
using N2.Web;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using System.Globalization;
using Lucene.Net.Search;
using System.Diagnostics;
using N2.Engine.Globalization;

namespace N2.Persistence.Search
{
	/// <summary>
	/// Wraps the usage of lucene to index content items.
	/// </summary>
	[Service(typeof(IIndexer))]
	public class LuceneIndexer : IIndexer
	{
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
			Trace.WriteLine("Clearing index");

			if (accessor.IndexExists())
			{
				var d = accessor.GetDirectory();
				d.ClearLock("write.lock"); ;
				var w = accessor.GetWriter();
				if (w.NumDocs() > 0)
				{
					try
					{
						w.DeleteAll();
						w.Commit();
						accessor.RecreateSearcher();
					}
					finally
					{
						//w.Close(true);
					}
				}
				d.ClearLock("write.lock"); ;
				//d.Close();
			}
		}

		/// <summary>Unlocks the index.</summary>
		public virtual void Unlock()
		{
			Trace.WriteLine("Unlocking index");
			accessor.GetDirectory().ClearLock("write.lock");
		}

		/// <summary>Optimizes the index.</summary>
		public virtual void Optimize()
		{
			Trace.WriteLine("Optimizing index");

			if (accessor.IndexExists())
			{
				var d = accessor.GetDirectory();
				var iw = accessor.GetWriter();
				if (iw.NumDocs() > 0)
				{
					try
					{
						iw.Optimize(true);
						iw.Commit();
					}
					finally
					{
						//iw.Close();
					}
				}
				//d.Close();
			}
		}

		/// <summary>Updates the index with the given item.</summary>
		/// <param name="item">The item containing content to be indexed.</param>
		public virtual void Update(ContentItem item)
		{
			if(item == null || item.ID == 0)
				return;

			Trace.WriteLine("Updating item #" + item.ID);

			if (!item.IsPage)
			    Update(Find.ClosestPage(item));

			var iw = accessor.GetWriter();

			if (!extractor.IsIndexable(item))
				return;

			var doc = CreateDocument(item);
			iw.UpdateDocument(new Term("ID", item.ID.ToString()), doc);
			iw.Commit();
			accessor.RecreateSearcher();
		}

		/// <summary>Delets an item from the index and any descendants.</summary>
		/// <param name="itemID">The id of the item to delete.</param>
		public virtual void Delete(int itemID)
		{
			Trace.WriteLine("Deleting item #" + itemID);

			var iw = accessor.GetWriter();
			var s = accessor.GetSearcher();
			try
			{
				string trail = GetTrail(s, new Term("ID", itemID.ToString()));
				var query = new PrefixQuery(new Term("Trail", trail));
				iw.DeleteDocuments(query);
				iw.Commit();
				accessor.RecreateSearcher();
			}
			finally
			{
				//iw.Close(waitForMerges:true);
			}
		}



		private string GetTrail(IndexSearcher s, Term t)
		{
			return s.Search(new TermQuery(t), 1)
				.scoreDocs
				.Select(d => s.Doc(d.doc).Get("Trail"))
				.FirstOrDefault();
		}

		public virtual Document CreateDocument(ContentItem item)
		{
			var doc = new Document();
			doc.Add(new Field("ID", item.ID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Title", item.Title ?? "", Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("Name", item.Name ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("SavedBy", item.SavedBy ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Created", DateTools.DateToString(item.Created.ToUniversalTime(), DateTools.Resolution.SECOND), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Updated", DateTools.DateToString(item.Updated.ToUniversalTime(), DateTools.Resolution.SECOND), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Published", item.Published.HasValue ? DateTools.DateToString(item.Published.Value.ToUniversalTime(), DateTools.Resolution.SECOND) : "", Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Expires", item.Expires.HasValue ? DateTools.DateToString(item.Expires.Value.ToUniversalTime(), DateTools.Resolution.SECOND) : "", Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Url", item.Url, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Path", item.Path ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("AncestralTrail", (item.AncestralTrail ?? ""), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Trail", Utility.GetTrail(item), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("AlteredPermissions", ((int)item.AlteredPermissions).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("State", ((int)item.State).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("IsPage", item.IsPage.ToString().ToLower(), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Roles", GetRoles(item), Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("Types", GetTypes(item), Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("Language", GetLanguage(item), Field.Store.YES, Field.Index.NOT_ANALYZED));

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
	}
}
