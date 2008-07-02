using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace N2.Templates.Wiki.Fragmenters
{
    public class InternalLinkFragmenter : AbstractFragmenter
    {
        public InternalLinkFragmenter()
        {
            Expression = CreateExpression(@"\[\[[^\[\]]*?\]\]");
        }
    }
}
