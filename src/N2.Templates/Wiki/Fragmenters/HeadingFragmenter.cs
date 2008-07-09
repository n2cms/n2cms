using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki.Fragmenters
{
    public class HeadingFragmenter : AbstractFragmenter
    {
        public HeadingFragmenter()
        {
            Expression = CreateExpression(@"=+[^=\]]*?=+");
        }
    }
}
