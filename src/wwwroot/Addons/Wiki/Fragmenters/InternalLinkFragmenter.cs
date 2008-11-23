using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace N2.Addons.Wiki.Fragmenters
{
    public class InternalLinkFragmenter : RegexFragmenter
    {
        public InternalLinkFragmenter()
            : base(@"\[\[[^\[\]]*?\]\]")
        {
        }
    }
}
