using System;
using System.Collections;
using System.IO;
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
using System.Diagnostics;

namespace N2.Persistence.Search
{
	/// <summary>
	/// Simplifies access to the lucene API.
	/// </summary>
	[Service(Configuration = "lucene")]
	public class LuceneAccesor : IDisposable
	{
		private Engine.Logger<LuceneAccesor> logger;
		string indexPath;
		public long LockTimeout { get; set; }
		Directory directory;
		IndexSearcher searcher;
		IndexWriter writer;

		public LuceneAccesor(IWebContext webContext, DatabaseSection config)
		{
			LockTimeout = 2000L;
			indexPath = Path.Combine(webContext.MapPath(config.Search.IndexPath), "Pages");
		}

		~LuceneAccesor()
		{
			Dispose();
		}

		public IndexWriter GetWriter()
		{
			lock (this)
			{
				return writer ?? (writer = CreateWriter(GetDirectory(), GetAnalyzer()));
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
				logger.Debug("Failed to obtain lock, deleting it and retrying.");
				ClearLock();
				return CreateWriterNoTry(d, a);
			}
		}

		private IndexWriter CreateWriterNoTry(Directory d, Analyzer a)
		{
			var indexExists = IndexExists();
			logger.Debug("Creating index writer, index exists: " + indexExists);
			var iw = new IndexWriter(d, a, create: !indexExists, mfl: IndexWriter.MaxFieldLength.UNLIMITED);
			iw.SetWriteLockTimeout(LockTimeout);
			return iw;
		}

		public virtual Analyzer GetAnalyzer()
		{
			return new PerFieldAnalyzerWrapper(
				new StandardAnalyzer(Version.LUCENE_29),
				new Hashtable
				{
					{ "ID", new WhitespaceAnalyzer() },
					{ "AlteredPermissions", new WhitespaceAnalyzer() },
					{ "Roles", new WhitespaceAnalyzer() },
					{ "State", new WhitespaceAnalyzer() },
					{ "IsPage", new WhitespaceAnalyzer() },
				});
		}

		public virtual Directory GetDirectory()
		{
			lock (this)
			{
				if (!System.IO.Directory.Exists(indexPath))
					System.IO.Directory.CreateDirectory(indexPath);
				
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
			var s = new IndexSearcher(dir, readOnly: true);
			return s;
		}

		public virtual QueryParser GetQueryParser()
		{
			return new QueryParser(Version.LUCENE_29, "Text", GetAnalyzer());
		}

		public void Dispose()
		{
			lock (this)
			{
				if (writer != null)
					writer.Close(waitForMerges: true);
				writer = null;
				searcher = null;
				directory = null;
			}
		}

		public void RecreateSearcher()
		{
			lock (this)
			{
				searcher = null;
			}
		}

		public virtual bool IndexExists()
		{
			return System.IO.Directory.Exists(indexPath) && GetDirectory().IndexExists();
		}

		public virtual void ClearLock()
		{
			var d = GetDirectory();
			d.ClearLock("write.lock"); ;
		}
	}
}
