using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using N2.Configuration;
using N2.Engine;
using N2.Web;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace N2.Persistence.Search
{
    /// <summary>
    /// Simplifies access to the lucene API.
    /// </summary>
    [Service(Configuration = "Lucene")]
    public class LuceneAccesor : IDisposable
    {
        private Engine.Logger<LuceneAccesor> logger;
        string indexPath;
        public long LockTimeout { get; set; }
        Directory directory;
        IndexSearcher searcher;

        public LuceneAccesor(string indexPath)
        {
            LockTimeout = 2000L;
            this.indexPath = indexPath;
        }

        public LuceneAccesor(IWebContext webContext, DatabaseSection config)
        {
            LockTimeout = 2000L;
            indexPath = Path.Combine(webContext.MapPath(config.Search.IndexPath), config.Search.Client.InstanceName);
        }

        public IndexWriter GetWriter()
        {
            lock (this)
            {
                return CreateWriter(GetDirectory(), GetAnalyzer());
            }
        }

        protected virtual IndexWriter CreateWriter(Directory d, Analyzer a)
        {
            try
            {
                return CreateWriterNoTry(d, a);
            }
            catch (Lucene.Net.Store.LockObtainFailedException)
            {
                logger.Warn("Failed to obtain lock, deleting it and retrying.");
                ClearLock();
                return CreateWriterNoTry(d, a);
            }
        }

        private IndexWriter CreateWriterNoTry(Directory d, Analyzer a)
        {
            var indexExists = IndexExists();
			logger.Info("Creating index writer, indexExists=" + indexExists);
            var iw = new IndexWriter(d, a, create: !indexExists, mfl: IndexWriter.MaxFieldLength.UNLIMITED);
            iw.WriteLockTimeout = LockTimeout;
            return iw;
        }

        public virtual Analyzer GetAnalyzer()
        {
            return new PerFieldAnalyzerWrapper(
                new StandardAnalyzer(Version.LUCENE_30),
                new Dictionary<string, Analyzer>
                {
                    { "ID", new WhitespaceAnalyzer() },
                    { "AlteredPermissions", new WhitespaceAnalyzer() },
                    { "Roles", new WhitespaceAnalyzer() },
                    { "State", new WhitespaceAnalyzer() },
                    { "IsPage", new WhitespaceAnalyzer() },
                    { "Language", new SimpleAnalyzer() }
                });
        }

        public virtual Directory GetDirectory()
        {
            lock (this)
            {
                if (!System.IO.Directory.Exists(indexPath))
				{
					logger.Info("Creating directory: " + indexPath);
					System.IO.Directory.CreateDirectory(indexPath);
				}
                
                var d = directory ?? (directory = new SimpleFSDirectory(new DirectoryInfo(indexPath), new SimpleFSLockFactory()));
                return d;
            }
        }

        public IndexSearcher GetSearcher()
        {
            lock (this)
            {
                return searcher ?? (searcher = CreateSearcher(GetDirectory()));
            }
        }

        protected virtual IndexSearcher CreateSearcher(Directory dir)
        {
			if (!dir.FileExists("segments.gen"))
			{
				logger.Info("No segments.gen (whatever that means)");
				GetWriter().Commit();
			}
            var s = new IndexSearcher(dir, readOnly: true);
            return s;
        }

        public virtual QueryParser GetQueryParser()
        {
            return new QueryParser(Version.LUCENE_30, "Text", GetAnalyzer());
        }

        public void Dispose()
        {
            lock (this)
            {
				logger.Info("Disposing");
				searcher = null;
                directory = null;
            }
        }

        public LuceneAccesor RecreateSearcher()
        {
            lock (this)
            {
                searcher = null;
                return this;
            }
        }

        public virtual bool IndexExists()
        {
            return System.IO.Directory.Exists(indexPath) 
                && GetDirectory().IndexExists();
        }

        public virtual void ClearLock()
        {
			logger.Info("Clearing lock");
			var d = GetDirectory();
            d.ClearLock("write.lock"); ;
        }
    }
}
