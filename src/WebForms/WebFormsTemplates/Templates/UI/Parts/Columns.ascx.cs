using System;

namespace N2.Templates.UI.Parts
{
    public partial class Columns : Web.UI.TemplateUserControl<ContentItem, Templates.Items.Columns>
    {
        public Columns()
        {
            CssClass += " cf";
        }
    }
}
