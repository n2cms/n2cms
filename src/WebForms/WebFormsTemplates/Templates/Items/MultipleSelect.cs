using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Templates.Web.UI.WebControls;
using N2.Web.Parts;

namespace N2.Templates.Items
{
    [PartDefinition("Multiple Select (check boxes)")]
    public class MultipleSelect : OptionSelectQuestion, IAddablePart
    {
        [EditableCheckBox("Display vertically", 19)]
        public virtual bool Vertical
        {
            get { return (bool)(GetDetail("Vertical") ?? true); }
            set { SetDetail("Vertical", value); }
        }

        public virtual Control AddTo(Control container)
        {
            MultipleSelectControl ssc = new MultipleSelectControl(this, Vertical ? RepeatDirection.Vertical : RepeatDirection.Horizontal);
            container.Controls.Add(ssc);
            return ssc;
        }
    }
}
