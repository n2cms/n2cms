using System;

namespace N2.Templates.UI.Layouts.Parts
{
	public partial class Columns : Web.UI.TemplateUserControl<Items.AbstractContentPage, Items.LayoutParts.Columns>
	{
		public Columns()
		{
			CssClass += " cf";
		}
	}
}