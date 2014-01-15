using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Addons.Wiki
{
    public class RenderingContext
    {
        public ContentItem Wiki { get; set; }
        public ContentItem Article { get; set; }
        public Fragment Fragment { get; set; }
    }
}
