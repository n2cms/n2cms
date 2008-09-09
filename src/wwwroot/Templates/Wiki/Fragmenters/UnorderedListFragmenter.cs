using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki.Fragmenters
{
    public class UnorderedListFragmenter : ListFragmenter
    {
        public UnorderedListFragmenter()
            : base(@"^(?<Value>[*]+)\s*(?<Contents>[^\r\n]*)[\r\n]*")
        {
        }
    }
}
