using N2.Details;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Items
{
    [Definition("Text", "Text")]
    [AllowedZones(AllowedZones.All)]
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