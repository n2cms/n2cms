using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.TemplateWeb.Domain
{
	[Definition("My data", "DataItem")]
	[AvailableZone("Main", "Main"),
		AllowedZones("Left", "Right"),
		RestrictParents(typeof (MyPageData), typeof (MySpecialPageData))]
	[WithEditable("Zone", typeof (ZoneSelector), "SelectedValue", 50, "ZoneName")]
	[WithEditableTitle("Title", 10)]
	public class MyItemData : ContentItem
	{
		[EditableTextBox("Text", 110)]
		[EditorModifier("TextMode", TextBoxMode.MultiLine)]
		public virtual string Text
		{
			get { return (string) GetDetail("Text"); }
			set { SetDetail<string>("Text", value); }
		}

		[Editable("File", typeof (UrlSelector), "Url", 30)]
		public virtual string FileUrl
		{
			get { return (string) GetDetail("FileUrl"); }
			set { SetDetail("FileUrl", value); }
		}

		public override string TemplateUrl
		{
			get { return "~/Uc/MyItem.ascx"; }
		}

		public override bool IsPage
		{
			get { return false; }
		}
	}
}