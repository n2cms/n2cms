using System;
using N2.Web;

namespace N2.Templates.Tests.Wiki
{
    public class FakeUrlParser : IUrlParser
    {
		public FakeUrlParser()
		{
			PageNotFound += delegate { };
		}
        public event EventHandler<PageNotFoundEventArgs> PageNotFound;

        public string Extension
        {
            get { throw new NotImplementedException(); }
        }

        public ContentItem StartPage
        {
            get { throw new NotImplementedException(); }
        }

        public ContentItem CurrentPage
        {
            get { throw new NotImplementedException(); }
        }

        public Site CurrentSite
        {
            get { throw new NotImplementedException(); }
        }

        public string BuildUrl(ContentItem item)
        {
            Url url = "/" + item.Name + ".aspx";
            foreach (ContentItem parent in N2.Templates.Find.EnumerateParents(item))
            {
                url = url.PrependSegment(parent.Name);
            }
            return url;
        }

        public bool IsRootOrStartPage(ContentItem item)
        {
            throw new NotImplementedException();
        }

        public ContentItem Parse(string url)
        {
            throw new NotImplementedException();
        }

        public ContentItem ParsePage(string url)
        {
            throw new NotImplementedException();
        }

		public PathData ResolvePath(Url url)
		{
			throw new NotImplementedException();
		}

		public string StripDefaultDocument(string path)
		{
			return path;
		}
	}
}
