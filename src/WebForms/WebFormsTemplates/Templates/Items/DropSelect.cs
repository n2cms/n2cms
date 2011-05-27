using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Templates.Items;
using N2.Web.Parts;
using System.Web.UI;
using N2.Templates.Web.UI.WebControls;
using N2.Persistence;
using N2.Templates.Details;

namespace N2.Templates.Items
{

    [N2.Definition("Single Select (Dropdown)", "Dropdown")]
    public class DropSelect : OptionSelectQuestion, IAddablePart
    {

        [EditableOptions(Title = "Options", SortOrder = 20)]
        public override IList<Option> Options
        {
            get
            {
                List<Option> options = new List<Option>();
                foreach (Option o in GetChildren())
                {
                    if (o.Title == "Select")
                    {
                        options.Insert(0, o);
                    }
                    else
                    {
                        options.Add(o);
                    }
                }
                return options;
            }
        }




        [N2.Details.EditableTextBox("Width", 120)]
        public virtual System.Nullable<int> Width
        {
            get { return (System.Nullable<int>)GetDetail("Width"); }
            set { SetDetail("Width", value); }
        }


        Control IAddablePart.AddTo(Control container)
        {
            Web.UI.WebControls.DropDowncontrol ssc = new Web.UI.WebControls.DropDowncontrol(this);
            container.Controls.Add(ssc);
           
            return ssc;
        }

    }
}