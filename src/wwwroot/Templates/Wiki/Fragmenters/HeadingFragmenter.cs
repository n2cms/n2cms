using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki.Fragmenters
{
    public class HeadingFragmenter : RegexFragmenter
    {
        public HeadingFragmenter()
            : base(@"(?<Value>=+)(?<Contents>[^=\]]*?)=+[\r\n]*")
        {
        }
    }
}
