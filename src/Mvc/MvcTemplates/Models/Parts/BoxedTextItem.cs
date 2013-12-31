using N2.Details;
using N2.Integrity;

namespace N2.Templates.Mvc.Models.Parts
{
    [PartDefinition("Boxed Text",
        Name = "BoxedText",
        IconUrl = "~/Content/Img/application_view_columns.png",
        SortOrder = -100)]
    [WithEditableTitle]
    public class BoxedTextItem : PartBase
    {
        [EditableFreeTextArea("Text", 100)]
        public virtual string Text
        {
            get { return (string) (GetDetail("Text") ?? string.Empty); }
            set { SetDetail("Text", value, string.Empty); }
        }
    }
}
