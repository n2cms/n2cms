using System;

namespace N2.Addons.Wiki.UI.Views
{
    public partial class Wiki : WikiTemplatePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            N2.Resources.Register.StyleSheet(this, "~/Addons/Wiki/UI/Css/Wiki.css");
        }
    }
}
