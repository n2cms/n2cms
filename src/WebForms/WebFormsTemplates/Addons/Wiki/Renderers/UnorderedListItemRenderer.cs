using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Addons.Wiki.Renderers
{
    public class UnorderedListItemRenderer : ListRenderer
    {
        protected override string BeginTag
        {
            get { return "<ul>"; }
        }

        protected override string EndTag
        {
            get { return "</ul>"; }
        }
    }
}
