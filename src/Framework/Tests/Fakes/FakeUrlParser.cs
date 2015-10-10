using N2.Definitions;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Tests.Fakes
{
	public class FakeUrlParser : IUrlParser
	{
		public event EventHandler<PageNotFoundEventArgs> PageNotFound;

		public event EventHandler<UrlEventArgs> BuildingUrl;

		public event EventHandler<UrlEventArgs> BuiltUrl;
		
		private ContentItem startPage;

		public FakeUrlParser(ContentItem startPage = null)
		{
			this.startPage = startPage;
		}

		public ContentItem StartPage
		{
			get { return startPage; }
		}

		public ContentItem CurrentPage
		{
			get { throw new NotImplementedException(); }
		}

		public Url BuildUrl(ContentItem item)
		{
			return string.Join("/", Find.EnumerateParents(item).TakeWhile(ci => !IsRootOrStartPage(ci)).SelectMany(ci => ci.Name));
		}

		public bool IsRootOrStartPage(ContentItem item)
		{
			return item is IStartPage
				|| item.Parent == null;
		}

		public PathData FindPath(Url url, ContentItem startNode = null, string remainingPath = null)
		{
			throw new NotImplementedException();
		}

		public PathData ResolvePath(Url url, ContentItem startNode = null, string remainingPath = null)
		{
			throw new NotImplementedException();
		}

		public ContentItem Parse(string url)
		{
			throw new NotImplementedException();
		}

		public string StripDefaultDocument(string path)
		{
			throw new NotImplementedException();
		}
	}
}
