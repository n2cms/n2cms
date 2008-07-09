using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki
{
    public class ViewContext
    {
        public IArticle Article { get; set; }
        public Fragment Fragment { get; set; }
    }
}
