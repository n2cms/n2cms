using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web
{
    /// <summary>
    /// Exposes information about url's that can't be parsed into a page.
    /// </summary>
    public class PageNotFoundEventArgs : ItemEventArgs
    {
        private string url;

        public PageNotFoundEventArgs(string url)
            : base(null)
        {
            this.url = url;
        }

        /// <summary>The url that didn't match any page.</summary>
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
    }
}
