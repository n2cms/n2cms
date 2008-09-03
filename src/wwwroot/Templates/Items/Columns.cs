using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Items
{
    [Definition("Two column container", "Columns")]
    [AvailableZone("Left", "ColumnLeft"), AvailableZone("Right", "ColumnRight")]
    [AllowedZones("Content")]
    public class Columns : AbstractItem
    {
        protected override string IconName
        {
            get { return "text_columns"; }
        }

        protected override string TemplateName
        {
            get { return "Columns"; }
        }
    }
}