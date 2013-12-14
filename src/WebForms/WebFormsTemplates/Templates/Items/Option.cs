using N2.Details;
using N2.Integrity;

namespace N2.Templates.Items
{
    [WithEditableTitle("Text", 10)]
    [RestrictParents(typeof(OptionSelectQuestion))]
    [PartDefinition("Option")]
    public class Option : AbstractItem
    {
        [N2.Details.EditableNumber("Answers", 100)]
        public virtual int Answers
        {
            get { return (int)(GetDetail("Answers") ?? 0); }
            set { SetDetail("Answers", value, 0); }
        }
    }
}
