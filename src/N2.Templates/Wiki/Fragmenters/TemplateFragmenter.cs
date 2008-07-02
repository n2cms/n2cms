using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace N2.Templates.Wiki.Fragmenters
{
    public class TemplateFragmenter : AbstractFragmenter
    {
        public TemplateFragmenter()
        {
            Expression = new Regex("{{.+?}}");
        }
    }
}
