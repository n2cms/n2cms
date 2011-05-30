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

namespace N2.Persistence.Search
{
	/// <summary>
	/// Simplifies access to the lucene API.
	/// </summary>
	[Service]
	public class LuceneAccesor
	{
		string indexPath;
		public long LockTimeout { get; set; }

		public LuceneAccesor(IWebContext webContext, DatabaseSection config)
		{
			LockTimeout = 2000L;
			indexPath = Path.Combine(webContext.MapPath(config.Search.IndexPath), "Pages");
		}

		public IndexWriter GetWriter()
		{
			var d = GetDirectory();
			var a = GetAnalyzer();
			return GetWriter(d, a);
		}

		public virtual IndexWriter GetWriter(Directory d, Analyzer a)
		{
			var iw = new IndexWriter(d, a, create: !d.IndexExists(), mfl: IndexWriter.MaxFieldLength.UNLIMITED);
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
			var d = new SimpleFSDirectory(new DirectoryInfo(indexPath), new SimpleFSLockFactory());
			return d;
		}

		public IndexSearcher GetSearcher()
		{
			var dir = GetDirectory();
			return GetSearcher(dir);
		}

		public virtual IndexSearcher GetSearcher(Directory dir)
		{
			var s = new IndexSearcher(dir, readOnly: true);
			return s;
		}

		public virtual QueryParser GetQueryParser()
		{
			return new QueryParser(Version.LUCENE_29, "Text", GetAnalyzer());
		}
	}
}
