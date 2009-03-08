using N2.Details;
using N2.Integrity;

namespace N2.Templates.Items
{
    [Definition("Boxed Text", "BoxedText")]
    [AllowedZones(Zones.Left, Zones.Right, Zones.RecursiveLeft, Zones.RecursiveRight, Zones.SiteLeft, Zones.SiteRight)]
    [WithEditableTitle]
    public class BoxedTextItem : AbstractItem
    {
        [EditableFreeTextArea("Text", 100)]
        public virtual string Text
        {
            get { return (string)(GetDetail("Text") ?? string.Empty); }
            set { SetDetail("Text", value, string.Empty); }
        }

        protected override string IconName
        {
            get { return "application_view_columns"; }
        }

        protected override string TemplateName
        {
            get { return "BoxedText"; }
        }
    }
}