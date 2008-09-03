using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Details;
using N2.Templates.Items;
using SingleSelectControl=N2.Templates.WebControls.SingleSelectControl;

namespace N2.Templates.Items
{
    [Definition("Single Select (radiobuttons)", "SingleSelect")]
    public class SingleSelect : OptionSelectQuestion, IContainable
    {
        [EditableCheckBox("Display vertically", 19)]
        public virtual bool Vertical
        {
            get { return (bool)(GetDetail("Vertical") ?? true); }
            set { SetDetail("Vertical", value); }
        }

        public Control AddTo(Control container)
        {
            SingleSelectControl ssc = new SingleSelectControl(this, Vertical ?  RepeatDirection.Vertical : RepeatDirection.Horizontal);
            container.Controls.Add(ssc);
            return ssc;
        }
    }
}