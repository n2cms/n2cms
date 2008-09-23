using System;
using System.Collections.Generic;
using System.Web;

namespace N2.Addons.AddonCatalog.Items
{
    [Flags]
    public enum CodeCategory
    {
        None = 0,
        Library = 1,
        ThemeLayout = 2,
        Page = 4,
        Part = 8
    }
}
