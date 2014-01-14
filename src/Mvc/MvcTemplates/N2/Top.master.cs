using System.Configuration;
using System.Web.Configuration;

namespace N2.Management
{
    public partial class Top : System.Web.UI.MasterPage
    {
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                var section = (AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication");
                logout.Visible = section.Mode == AuthenticationMode.Forms;
            }
            catch (System.Exception)
            {
            }
        }
    }
}
