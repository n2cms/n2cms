using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.UI.Items.LayoutParts
{
	[Definition("Two column container", "Columns")]
	[AvailableZone("Left", "ColumnLeft"), AvailableZone("Right", "ColumnRight")]
	[AllowedZones("Content")]
	public class Columns : AbstractItem
	{
		public override string TemplateUrl
		{
			get
			{
				return "~/Layouts/Parts/Columns.ascx";
			}
		}

		public override string IconUrl
		{
			get
			{
				return "~/Img/text_columns.png";
			}
		}
	}
}
