using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki
{
    public interface IWikiTemplate
    {
        ViewContext Viewed { get; set; }
    }
}
