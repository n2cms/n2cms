using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web
{
    public class UrlEventArgs : ItemEventArgs
    {
        public UrlEventArgs(ContentItem item)
            : base(item)
        {
        }

        public string Url { get; set; }
    }
}
