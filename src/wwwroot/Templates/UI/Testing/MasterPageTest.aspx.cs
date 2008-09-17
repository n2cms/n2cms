using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Templates.UI.Testing
{
    public partial class MasterPageTest : System.Web.UI.Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            try
            {
                Theme = N2.Templates.Find.StartPage.Theme;
            }
            catch
            {
                //swallow
            }
        }
    }
}
