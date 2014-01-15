using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Items
{
    [PartDefinition("Two column container",
        IconUrl = "~/Templates/UI/Img/text_columns.png")]
    [AvailableZone("Left", "ColumnLeft"), AvailableZone("Right", "ColumnRight")]
    [AllowedZones("Content")]
    public class Columns : AbstractItem
    {
        protected override string TemplateName
        {
            get { return "Columns"; }
        }
    }
}
