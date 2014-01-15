using System;
using System.Collections.Generic;
using System.Text;
using N2.Web.Parsing;

namespace N2.Addons.Wiki
{
    public class ViewContext
    {
        public WikiRenderer Renderer { get; set; }
        public IArticle Article { get; set; }
        public Component Previous { get; set; }
        public Component Fragment { get; set; }
        public Component Next { get; set; }
        public IDictionary<string, object> State { get; set; }
    }
}
