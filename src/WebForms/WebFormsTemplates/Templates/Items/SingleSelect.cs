using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Web.Parts;
using SingleSelectControl=N2.Templates.Web.UI.WebControls.SingleSelectControl;

namespace N2.Templates.Items
{
    public enum SingleSelectType
    {
        RadioButtons,
        DropDown,
        ListBox
    }

    [PartDefinition("Single Select (radio buttons/drop down/list box)")]
    public class SingleSelect : OptionSelectQuestion, IAddablePart
    {
        [EditableEnum("Selection type", 18, typeof(SingleSelectType))]
        public virtual SingleSelectType SelectionType
        {
            get { return (SingleSelectType)GetDetail("SelectionType", (int)SingleSelectType.RadioButtons); }
            set { SetDetail("SelectionType", (int)value); }
        }

        [EditableCheckBox("Display vertically (applicable to radio buttons)", 19)]
        public virtual bool Vertical
        {
            get { return (bool)(GetDetail("Vertical") ?? true); }
            set { SetDetail("Vertical", value); }
        }

        public virtual Control AddTo(Control container)
        {
            SingleSelectControl ssc = new SingleSelectControl(this);
            container.Controls.Add(ssc);
            return ssc;
        }
    }
}
