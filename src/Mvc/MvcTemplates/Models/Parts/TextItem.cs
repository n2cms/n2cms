using N2.Details;
using N2.Integrity;

namespace N2.Templates.Mvc.Models.Parts
{
    [PartDefinition("Text", Name = "Text",
        IconUrl = "~/Content/Img/text_align_left.png",
        SortOrder = -90)]
    public class TextItem : PartBase
    {
        [EditableFreeTextArea("Text", 100)]
        public virtual string Text
        {
            get { return (string) (GetDetail("Text") ?? string.Empty); }
            set { SetDetail("Text", value, string.Empty); }
        }
    }
}
