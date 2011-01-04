using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

namespace N2.Addons.Wiki.Fragmenters
{
    public class ExternalLinkFragmenter : RegexFragmenter
    {
        public ExternalLinkFragmenter()
            : base(@"(\[[^\[\]]*?\])|(\w+://[\w.:/?=&;#]+)")
        {
        }
    }
}
