using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Addons.Wiki.Fragmenters
{
    public class FormatFragmenter : RegexFragmenter
    {
        public FormatFragmenter()
            :base("'{2,}[^']+?'{2,}")
        {
        }
    }
}
