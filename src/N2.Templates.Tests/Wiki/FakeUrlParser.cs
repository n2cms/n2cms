using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web;

namespace N2.Templates.Tests.Wiki
{
    public class FakeUrlParser : IUrlParser
    {
        #region IUrlParser Members

        public event EventHandler<PageNotFoundEventArgs> PageNotFound;

        public string DefaultExtension
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
            return "/" + item.Name + ".aspx";
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

        #endregion
    }
}
