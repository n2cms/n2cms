using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Addons.Wiki.Renderers
{
    public class OrderedListItemRenderer : ListRenderer
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
