using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Templates.Wiki.Fragmenters
{
    public class UserInfoFragmenter : AbstractFragmenter
    {
        public UserInfoFragmenter()
        {
            Expression = CreateExpression("[~]{3,5}");
        }
    }
}
