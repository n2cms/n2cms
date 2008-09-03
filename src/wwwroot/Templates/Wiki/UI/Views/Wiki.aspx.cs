using System;

namespace N2.Templates.Wiki.UI
{
    public partial class Wiki : WikiTemplatePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            N2.Resources.Register.StyleSheet(this, "~/Wiki/UI/Css/Wiki.css");
        }
    }
}
