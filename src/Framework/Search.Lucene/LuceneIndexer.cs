using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace N2.Persistence.Search
{
    [Service]
    [Service(typeof(IIndexer), Replaces = typeof(EmptyIndexer), Configuration = "Lucene")]
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
            logger.Info("Clearing index");

            if (accessor.IndexExists())
            {
                accessor.ClearLock();
                using (var iw = accessor.GetWriter())
                {
                    if (iw.NumDocs() > 0)
                    {
                        iw.DeleteAll();
                        iw.PrepareCommit();
                        iw.Commit();
                        accessor.RecreateSearcher();
                    }
                }
                accessor.ClearLock();
            }
        }

        public virtual void Unlock()
        {
			logger.Info("Unlocking index");

            accessor.GetDirectory().ClearLock("write.lock");
        }

        public virtual void Optimize()
        {
			logger.Info("Optimizing index");

            if (accessor.IndexExists())
            {
                var d = accessor.GetDirectory();
                using (var iw = accessor.GetWriter())
                {
                    if (iw.NumDocs() > 0)
                    {
                        iw.Optimize(doWait: true);
                        iw.PrepareCommit();
                        iw.Commit();
                    }
                }
            }
        }

        public virtual void Update(IndexableDocument document)
        {
			logger.Info("Updating item #" + document.ID);

            var doc = new Document();
            foreach (var field in document.Values)
                doc.Add(new Field(field.Name, field.Value, field.Stored ? Field.Store.YES : Field.Store.NO, field.Analyzed ? Field.Index.ANALYZED : Field.Index.NOT_ANALYZED));

            WriteToIndex(document.ID, doc);
        }

        private void WriteToIndex(int itemID, Document doc)
        {
            if (doc == null)
                return;
            lock (accessor)
            {
                var iw = accessor.GetWriter();
                try
                {
                    iw.UpdateDocument(new Term(TextExtractor.Properties.ID, itemID.ToString()), doc);
                    iw.PrepareCommit();
                    iw.Commit();
                }
                catch (AlreadyClosedException ex)
                {
                    logger.Error(ex);
                    try
                    {
                        iw = accessor.GetWriter();
                        iw.UpdateDocument(new Term(TextExtractor.Properties.ID, itemID.ToString()), doc);
                        iw.PrepareCommit();
                        iw.Commit();
                    }
                    catch (Exception ex2)
                    {
                        logger.Error(ex2);
                        iw.Dispose();
                        accessor.ClearLock();
                    }
                }
                catch (ThreadAbortException ex)
                {
                    logger.Warn(ex);
                    iw.Rollback();
                    iw.Dispose(waitForMerges: false);
                    accessor.ClearLock();
                    throw;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    iw.Rollback();
                    iw.Dispose();
                    accessor.ClearLock();
                }
                finally
                {
                    iw.Dispose(waitForMerges: true);
                    accessor.RecreateSearcher();
                }
            }
        }

        public virtual void Delete(int itemID)
        {
			logger.Info("Deleting item #" + itemID);

            lock (accessor)
            {
                using (var iw = accessor.GetWriter())
                {
                    var s = accessor.GetSearcher();
                    string trail = GetTrail(s, new Term(TextExtractor.Properties.ID, itemID.ToString()));
                    if (trail == null)
                        return; // not indexed

                    var query = new PrefixQuery(new Term(TextExtractor.Properties.Trail, trail));
                    iw.DeleteDocuments(query);
                    iw.PrepareCommit();
                    iw.Commit();
                }
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
