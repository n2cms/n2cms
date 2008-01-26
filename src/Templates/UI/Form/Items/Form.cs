using System.Collections.Generic;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;
using N2.Templates.Survey.Items;
using N2.Web.UI;

namespace N2.Templates.Form.Items
{
	[Definition("Form", "Form", "A form that can be sumitted and sent to an email address.", "", 250)]
	[AllowedZones("Content", "ColumnLeft", "ColumnRight")]
	[RestrictParents(typeof (AbstractContentPage))]
	[AllowedChildren(typeof (Question))]
	[AvailableZone("Questions", "Questions")]
	[WithEditableTitle(Required = false)]
	[Divider("divider1", 105)]
	[Divider("divider2", 115)]
	public class Form : AbstractItem
	{
		[EditableFreeTextArea("Intro text", 100)]
		public virtual string IntroText
		{
			get { return (string)(GetDetail("IntroText") ?? string.Empty); }
			set { SetDetail("IntroText", value, string.Empty); }
		}

		[EditableFreeTextArea("SubmitText", 140)]
		public virtual string SubmitText
		{
			get { return (string)(GetDetail("SubmitText") ?? string.Empty); }
			set { SetDetail("SubmitText", value, string.Empty); }
		}

		[EditableChildren("Form fields", "Questions", "FormFields", 110)]
		public virtual IList<ContentItem> FormFields
		{
			get { return GetChildren(); }
		}

		[EditableTextBox("Mail from", 120)]
		public virtual string MailFrom
		{
			get { return (string) (GetDetail("MailFrom") ?? string.Empty); }
			set { SetDetail("MailFrom", value, string.Empty); }
		}

		[EditableTextBox("Mail to", 122)]
		public virtual string MailTo
		{
			get { return (string) (GetDetail("MailTo") ?? string.Empty); }
			set { SetDetail("MailTo", value, string.Empty); }
		}

		[EditableTextBox("Mail subject", 124)]
		public virtual string MailSubject
		{
			get { return (string) (GetDetail("MailSubject") ?? string.Empty); }
			set { SetDetail("MailSubject", value, string.Empty); }
		}

		[EditableTextBox("Mail intro", 126)]
		[EditorModifier("TextMode", TextBoxMode.MultiLine)]
		public virtual string MailBody
		{
			get { return (string) (GetDetail("MailBody") ?? string.Empty); }
			set { SetDetail("MailBody", value, string.Empty); }
		}

		public override string TemplateUrl
		{
			get { return "~/Form/UI/Form.ascx"; }
		}

		public override string IconUrl
		{
			get { return "~/Form/UI/Img/report.png"; }
		}
	}
}