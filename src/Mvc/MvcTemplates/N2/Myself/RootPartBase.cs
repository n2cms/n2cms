using N2.Definitions;
using N2.Integrity;

namespace N2.Management.Myself
{
    [RestrictParents(typeof(IRootPage))]
    [AllowedZones("Left", "Center", "Right")]
    public abstract class RootPartBase : ContentItem, ISystemNode, IManagementHomePart
    {
    }
}
