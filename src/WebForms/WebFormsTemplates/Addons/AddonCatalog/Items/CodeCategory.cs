using System;

namespace N2.Addons.AddonCatalog.Items
{
    [Flags]
    public enum CodeCategory
    {
        None = 0,
        Library = 1,
        ThemeLayout = 2,
        Pages = 4,
        Parts = 8
    }
}
