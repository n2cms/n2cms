using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki.Fragmenters
{
    public class FormatFragmenter : AbstractFragmenter
    {
        public FormatFragmenter()
        {
            Expression = CreateExpression("'{2,}[^']+?'{2,}");
        }
    }
}
