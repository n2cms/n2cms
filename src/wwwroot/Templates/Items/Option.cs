using N2.Details;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Items
{
    [WithEditableTitle("Text", 10)]
    [RestrictParents(typeof(OptionSelectQuestion))]
    [Definition("Option", "Option")]
    public class Option : Templates.Items.AbstractItem
    {
        [N2.Details.EditableTextBox("Answers", 100)]
        public virtual int Answers
        {
            get { return (int)(GetDetail("Answers") ?? 0); }
            set { SetDetail("Answers", value, 0); }
        }
    }
}