using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace N2.Web.UI.WebControls
{
    public class DropDownListWithValidation : System.Web.UI.WebControls.DropDownList
    {
        protected override ControlCollection CreateControlCollection()
        {
            return new ControlCollection(this);
        }
    }
}
