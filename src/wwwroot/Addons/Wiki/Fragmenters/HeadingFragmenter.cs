using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Addons.Wiki.Fragmenters
{
    public class HeadingFragmenter : RegexFragmenter
    {
        public HeadingFragmenter()
            : base(@"(?<Value>=+)(?<Contents>[^=\]]*?)=+[\r\n]*")
        {
        }
    }
}
