using N2.Integrity;

namespace N2.Templates.Mvc.Models.Parts
{
    [PartDefinition("Two column container",
        IconUrl = "~/Content/Img/text_columns.png")]
    [AvailableZone("Left", "ColumnLeft"), AvailableZone("Right", "ColumnRight")]
    [AllowedZones("Content")]
    public class Columns : PartBase
    {
    }
}
