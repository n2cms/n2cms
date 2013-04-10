using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
	[Service]
	[Service(typeof(IIndexer), Replaces = typeof(EmptyIndexer), Configuration = "lucene")]
	public class LuceneIndexer : IIndexer
	{
		private readonly Engine.Logger<LuceneIndexer> logger;
		private LuceneAccesor accessor;

		public LuceneIndexer(LuceneAccesor accessor)
		{
			this.accessor = accessor;
		}

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

		public virtual void Unlock()
		{
			logger.Debug("Unlocking index");

			accessor.GetDirectory().ClearLock("write.lock");
		}

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

		public virtual void Update(IndexableDocument document)
		{
			logger.Debug("Updating item #" + document.ID);

			var doc = new Document();
			foreach (var field in document.Values)
				doc.Add(new Field(field.Name, field.Value, field.Stored ? Field.Store.YES : Field.Store.NO, field.Analyzed ? Field.Index.ANALYZED : Field.Index.NOT_ANALYZED));

			WriteToIndex(document.ID, doc);
		}

		private void WriteToIndex(int itemID, Document doc)
		{
			if (doc != null)
			{
				lock (accessor)
				{
					var iw = accessor.GetWriter();
					try
					{
						iw.UpdateDocument(new Term(TextExtractor.Properties.ID, itemID.ToString()), doc);
						iw.Commit();
					}
					catch (AlreadyClosedException ex)
					{
						logger.Error(ex);
						try
						{
							iw = accessor.RecreateWriter().GetWriter();
							iw.UpdateDocument(new Term(TextExtractor.Properties.ID, itemID.ToString()), doc);
							iw.Commit();
						}
						catch (Exception ex2)
						{
							logger.Error(ex2);
							iw.Dispose();
							accessor.ClearLock();
						}
					}
					catch (Exception ex)
					{
						logger.Error(ex);
						iw.Dispose();
						accessor.ClearLock();
					}
					finally
					{
						accessor.RecreateSearcher();
					}
				}
			}
		}

		public virtual void Delete(int itemID)
		{
			logger.Debug("Deleting item #" + itemID);

			lock (accessor)
			{
				var iw = accessor.GetWriter();
				var s = accessor.GetSearcher();
				string trail = GetTrail(s, new Term(TextExtractor.Properties.ID, itemID.ToString()));
				if (trail == null)
					return; // not indexed

				var query = new PrefixQuery(new Term(TextExtractor.Properties.Trail, trail));
				iw.DeleteDocuments(query);
				iw.Commit();
				accessor.RecreateSearcher();
			}
		}

		private string GetTrail(IndexSearcher s, Term t)
		{
			return s.Search(new TermQuery(t), 1)
				.ScoreDocs
				.Select(d => s.Doc(d.Doc).Get(TextExtractor.Properties.Trail))
				.FirstOrDefault();
		}

		public virtual IndexStatistics GetStatistics()
		{
			return new IndexStatistics
			{
				TotalDocuments = accessor.GetWriter().GetReader().NumDocs()
			};
		}
	}
}
