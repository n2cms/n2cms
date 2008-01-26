using N2.Integrity;

namespace N2.Templates.Items.LayoutParts
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
