using System;

namespace N2.Templates.UI.Views
{
    public partial class FaqList : Web.UI.TemplatePage<Items.FaqList>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Resources.Register.StyleSheet(this, "~/Templates/UI/Css/Faq.css", N2.Resources.Media.All);
        }
    }
}
