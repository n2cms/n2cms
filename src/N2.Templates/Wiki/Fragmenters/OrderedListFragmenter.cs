using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki.Fragmenters
{
    public class OrderedListFragmenter : AbstractFragmenter
    {
        public OrderedListFragmenter()
        {
            Expression = CreateExpression(@"^[#]+\s*|(?<=^[#].*?)$");
        }
    }
}
