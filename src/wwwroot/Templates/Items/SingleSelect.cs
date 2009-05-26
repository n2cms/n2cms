using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Web.Parts;
using SingleSelectControl=N2.Templates.Web.UI.WebControls.SingleSelectControl;

namespace N2.Templates.Items
{
    [PartDefinition("Single Select (radio buttons)")]
	public class SingleSelect : OptionSelectQuestion, IAddablePart
    {
        [EditableCheckBox("Display vertically", 19)]
        public virtual bool Vertical
        {
            get { return (bool)(GetDetail("Vertical") ?? true); }
            set { SetDetail("Vertical", value); }
        }

		public virtual Control AddTo(Control container)
        {
            SingleSelectControl ssc = new SingleSelectControl(this, Vertical ?  RepeatDirection.Vertical : RepeatDirection.Horizontal);
            container.Controls.Add(ssc);
            return ssc;
        }
    }
}