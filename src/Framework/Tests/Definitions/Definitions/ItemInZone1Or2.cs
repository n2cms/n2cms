namespace N2.Tests.Definitions.Definitions
{
    [PartDefinition]
    [N2.Integrity.AllowedZones("Zone1", "Zone2")]
    public class ItemInZone1Or2 : N2.ContentItem
    {
    }
}
