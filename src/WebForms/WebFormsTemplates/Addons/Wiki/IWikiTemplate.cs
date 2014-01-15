using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Addons.Wiki
{
    public interface IWikiTemplate
    {
        ViewContext Viewed { get; set; }
    }
}
