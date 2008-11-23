using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Addons.Wiki.Fragmenters
{
    public class TextFragmenter : RegexFragmenter
    {
        public TextFragmenter()
            : base(".+")
        {
        }
    }
}
