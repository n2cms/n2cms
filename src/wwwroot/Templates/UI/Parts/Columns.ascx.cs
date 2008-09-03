using System;

namespace N2.Templates.UI.Parts
{
    public partial class Columns : Web.UI.TemplateUserControl<Templates.Items.AbstractContentPage, Templates.Items.Columns>
    {
        public Columns()
        {
            CssClass += " cf";
        }
    }
}