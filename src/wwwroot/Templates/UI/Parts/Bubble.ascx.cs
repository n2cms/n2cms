using System;
using N2.Templates.Items;

namespace N2.Templates.UI.Parts
{
    public partial class Bubble : Templates.Web.UI.TemplateUserControl<ContentItem, BubbleItem>
    {
        protected override void OnInit(EventArgs e)
        {
            Resources.Register.StyleSheet(Page, "~/Faq/UI/Css/Faq.css");
			
            base.OnInit(e);
        }
    }
}