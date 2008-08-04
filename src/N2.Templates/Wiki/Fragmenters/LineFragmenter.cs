using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki.Fragmenters
{
    public class LineFragmenter : AbstractFragmenter
    {
        public LineFragmenter()
        {
            Expression = CreateExpression(@"[\r\n]*(</?\s*(br|p)(\s[^>]>|>))[\r\n]*|[\r\n]+"); 
        }
    }
}
