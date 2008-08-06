using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki.Renderers
{
    public class OrderedListRenderer : ListRenderer
    {
        protected override string BeginTag
        {
            get { return "<ol>"; }
        }

        protected override string EndTag
        {
            get { return "</ol>"; }
        }
    }
}
