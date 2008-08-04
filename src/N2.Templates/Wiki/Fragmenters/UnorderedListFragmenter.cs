using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki.Fragmenters
{
    public class UnorderedListFragmenter : AbstractFragmenter
    {
        public UnorderedListFragmenter()
        {
            Expression = CreateExpression(@"^[*]+\s*|(?<=^[*].*?)$");
        }
    }
}
