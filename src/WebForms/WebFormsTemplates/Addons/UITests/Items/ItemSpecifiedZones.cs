using N2.Integrity;

namespace N2.Addons.UITests.Items
{
    [RestrictParents(typeof(AdaptiveItemPage), typeof(ItemAllNamed))]
    [AllowedZones("Left", "TestZone", "ItemAllNamed")]
    public class ItemSpecifiedZones : AbstractItem
    {
    }
}
