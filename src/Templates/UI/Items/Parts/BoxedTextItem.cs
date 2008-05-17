using N2.Details;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.UI.Items.Parts
{
	[Definition("Boxed Text", "BoxedText")]
	[AllowedZones(Zones.Left, Zones.Right, Zones.RecursiveLeft, Zones.RecursiveRight, Zones.SiteLeft, Zones.SiteRight)]
	[WithEditableTitle]
	public class BoxedTextItem : AbstractItem
	{
		[EditableFreeTextArea("Text", 100)]
		public virtual string Text
		{
			get { return (string)(GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value, string.Empty); }
		}
		
		public override string TemplateUrl
		{
			get { return "~/Parts/BoxedText.ascx"; }
		}

		public override string IconUrl
		{
			get { return "~/Img/application_view_columns.png"; }
		}
	}
}
