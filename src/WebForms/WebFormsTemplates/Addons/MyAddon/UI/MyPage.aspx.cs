using System;
using N2.Resources;
using N2.Templates.Web.UI;

namespace N2.Addons.MyAddon.UI
{
    public partial class MyPage : TemplatePage<Items.MyPage>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Register.StyleSheet(this, "~/Addons/MyAddon/UI/MyStyle.css");
        }
    }
}
