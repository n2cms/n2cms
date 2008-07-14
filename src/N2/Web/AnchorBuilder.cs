using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web
{
    public class AnchorBuilder : TagBuilder
    {
        string url;
        public AnchorBuilder(Url url, string text)
            : base("a", text)
        {
            this.url = url;
        }
    }
}
