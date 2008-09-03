using System;
using FaqList=N2.Templates.Items.FaqList;

namespace N2.Templates.UI.Views
{
    public partial class FaqList : Web.UI.TemplatePage<Templates.Items.FaqList>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Resources.Register.StyleSheet(this, "~/Faq/UI/Css/Faq.css", N2.Resources.Media.All);
        }
    }
}