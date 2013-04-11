using N2.Configuration;
using N2.Engine;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace N2.Persistence.Search
{
	[Service]
	[Service(typeof(IIndexer), Replaces = typeof(EmptyIndexer), Configuration = "remote")]
	public class IndexerClient : IIndexer
	{
		private readonly Engine.Logger<IndexerClient> logger;
		private string serverUrl;

		public IndexerClient(DatabaseSection config)
		{
			serverUrl = config.Search.Server.Url.Replace("://*", "://localhost");
		}

		//public virtual void Clear()
		//{
		//	logger.Debug("Clearing index");

		//	if (accessor.IndexExists())
		//	
		//		accessor.ClearLock();
		//		var w = accessor.GetWriter();
		//		if (w.NumDocs() > 0)
		//		{
		//			w.DeleteAll();
		//			w.Commit();
		//			accessor.RecreateSearcher();
		//		}
		//		accessor.ClearLock();
		//	}
		//}

		//public virtual void Unlock()
		//{
		//	logger.Debug("Unlocking index");

		//	accessor.GetDirectory().ClearLock("write.lock");
		//}

		//public virtual void Optimize()
		//{
		//	logger.Debug("Optimizing index");

		//	if (accessor.IndexExists())
		//	{
		//		var d = accessor.GetDirectory();
		//		var iw = accessor.GetWriter();
		//		if (iw.NumDocs() > 0)
		//		{
		//			iw.Optimize(true);
		//			iw.Commit();
		//		}
		//	}
		//}

		//public virtual void Update(IndexableDocument document)
		//{
		//	logger.Debug("Updating item #" + document.ID);

		//	var doc = new Document();
		//	foreach (var field in document.Values)
		//		doc.Add(new Field(field.Name, field.Value, field.Stored ? Field.Store.YES : Field.Store.NO, field.Analyzed ? Field.Index.ANALYZED : Field.Index.NOT_ANALYZED));

		//	WriteToIndex(document.ID, doc);
		//}

		//private void WriteToIndex(int itemID, Document doc)
		//{
		//	if (doc != null)
		//	{
		//		lock (accessor)
		//		{
		//			var iw = accessor.GetWriter();
		//			try
		//			{
		//				iw.UpdateDocument(new Term(TextExtractor.Properties.ID, itemID.ToString()), doc);
		//				iw.Commit();
		//			}
		//			catch (AlreadyClosedException ex)
		//			{
		//				logger.Error(ex);
		//				try
		//				{
		//					iw = accessor.RecreateWriter().GetWriter();
		//					iw.UpdateDocument(new Term(TextExtractor.Properties.ID, itemID.ToString()), doc);
		//					iw.Commit();
		//				}
		//				catch (Exception ex2)
		//				{
		//					logger.Error(ex2);
		//					iw.Dispose();
		//					accessor.ClearLock();
		//				}
		//			}
		//			catch (Exception ex)
		//			{
		//				logger.Error(ex);
		//				iw.Dispose();
		//				accessor.ClearLock();
		//			}
		//			finally
		//			{
		//				accessor.RecreateSearcher();
		//			}
		//		}
		//	}
		//}

		//public virtual void Delete(int itemID)
		//{
		//	logger.Debug("Deleting item #" + itemID);

		//	lock (accessor)
		//	{
		//		var iw = accessor.GetWriter();
		//		var s = accessor.GetSearcher();
		//		string trail = GetTrail(s, new Term(TextExtractor.Properties.ID, itemID.ToString()));
		//		if (trail == null)
		//			return; // not indexed

		//		var query = new PrefixQuery(new Term(TextExtractor.Properties.Trail, trail));
		//		iw.DeleteDocuments(query);
		//		iw.Commit();
		//		accessor.RecreateSearcher();
		//	}
		//}

		//private string GetTrail(IndexSearcher s, Term t)
		//{
		//	return s.Search(new TermQuery(t), 1)
		//		.ScoreDocs
		//		.Select(d => s.Doc(d.Doc).Get(TextExtractor.Properties.Trail))
		//		.FirstOrDefault();
		//}

		//public virtual IndexStatistics GetStatistics()
		//{
		//	return new IndexStatistics
		//	{
		//		TotalDocuments = accessor.GetWriter().GetReader().NumDocs()
		//	};
		//}

		public void Update(IndexableDocument document)
		{
			Request("PUT", "items/" + document.ID + "/", new JavaScriptSerializer().Serialize(document));
		}

		public void Delete(int itemID)
		{
			Request("DELETE", "items/" + itemID + "/", "");
		}

		public IndexStatistics GetStatistics()
		{
			var response = Request("GET", "index/statistics", "");
			return new JavaScriptSerializer().Deserialize<IndexStatistics>(response);
		}

		public void Clear()
		{
			var response = Request("POST", "index/clear", "");
		}

		public void Optimize()
		{
			var response = Request("POST", "index/optimize", "");
		}

		public void Unlock()
		{
			var response = Request("POST", "index/unlock", "");
		}

		private string Request(string httpMethod, string path, string requestBody)
		{
			HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(serverUrl + path);
			hwr.Method = httpMethod;
			hwr.ContentType = "application/json";
			if (string.IsNullOrEmpty(requestBody))
				hwr.ContentLength = 0;
			else
			{
				using (var s = hwr.GetRequestStream())
				using (var tw = new StreamWriter(s))
				{
					tw.Write(requestBody);
				}
			}

			using (var wr = hwr.GetResponse())
			{
				if (wr.ContentLength == 0)
					return "";

				using (var s = wr.GetResponseStream())
				using (var sr = new StreamReader(s))
				{
					return sr.ReadToEnd();
				}
			}
		}
	}
}
