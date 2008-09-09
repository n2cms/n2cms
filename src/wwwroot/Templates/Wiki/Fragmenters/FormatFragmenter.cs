using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki.Fragmenters
{
    public class FormatFragmenter : RegexFragmenter
    {
        public FormatFragmenter()
            :base("'{2,}[^']+?'{2,}")
        {
        }
    }
}
