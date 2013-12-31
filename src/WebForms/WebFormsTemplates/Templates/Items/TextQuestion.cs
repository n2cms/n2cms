using System.Web.UI;
using N2.Templates.Web.UI.WebControls;
using N2.Web.Parts;

namespace N2.Templates.Items
{
    [PartDefinition("Text question (textbox)")]
    public class TextQuestion : Question, IAddablePart
    {
        [N2.Details.EditableNumber("Rows", 110, Columns = 5)]
        public virtual int Rows
        {
            get { return (int)(GetDetail("Rows") ?? 1); }
            set { SetDetail("Rows", value, 1); }
        }

        [N2.Details.EditableNumber("Columns", 120, Columns = 5)]
        public virtual int? Columns
        {
            get { return (int?)GetDetail("Columns"); }
            set { SetDetail("Columns", value); }
        }

        public virtual Control AddTo(Control container)
        {
            TextControl t = new TextControl(this);
            container.Controls.Add(t);
            return t;
        }
    }
}
