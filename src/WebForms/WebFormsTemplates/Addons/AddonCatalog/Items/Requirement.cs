using System;

namespace N2.Addons.AddonCatalog.Items
{
    [Flags]
    public enum Requirement 
    {
        None = 0,
        N2 = 1, 
        N2_Templates = 2,
        ASPNET_35 = 4,
        FullTrust = 8
    }
}
