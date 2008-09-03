using N2.Details;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Items
{
    [Definition("Text", "Text")]
    [AllowedZones(Zones.Content, Zones.Left, Zones.Right, Zones.RecursiveAbove, Zones.RecursiveLeft, Zones.RecursiveRight, Zones.SiteLeft, Zones.SiteRight, Zones.ColumnLeft, Zones.ColumnRight)]
    public class TextItem : AbstractItem
    {
        [EditableFreeTextArea("Text", 100)]
        public virtual string Text
        {
            get { return (string)(GetDetail("Text") ?? string.Empty); }
            set { SetDetail("Text", value, string.Empty); }
        }

        protected override string IconName
        {
            get { return "text_align_left"; }
        }

        protected override string TemplateName
        {
            get { return "Text"; }
        }
    }
}