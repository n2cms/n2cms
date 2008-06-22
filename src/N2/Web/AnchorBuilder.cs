using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web
{
    public class AnchorBuilder : TagBuilder
    {
        string url;
        string query = null;
        public AnchorBuilder(string url, string text)
            : base("a", text)
        {
        }
    }
}
