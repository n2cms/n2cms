using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Definitions;
using N2.Templates.WebControls;
using OptionSelectQuestion=N2.Templates.Items.OptionSelectQuestion;

namespace N2.Templates.Items
{
    [Definition("Multiple Select (check boxes)")]
    public class MultipleSelect : OptionSelectQuestion, IContainable
    {
        [EditableCheckBox("Display vertically", 19)]
        public virtual bool Vertical
        {
            get { return (bool)(GetDetail("Vertical") ?? true); }
            set { SetDetail("Vertical", value); }
        }

        public Control AddTo(Control container)
        {
            MultipleSelectControl ssc = new MultipleSelectControl(this, Vertical ? RepeatDirection.Vertical : RepeatDirection.Horizontal);
            container.Controls.Add(ssc);
            return ssc;
        }
    }
}