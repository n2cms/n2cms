using System.Collections.Generic;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;
using N2.Web.UI;

namespace N2.Templates.Items
{
    [PartDefinition("Form", 
		Description = "A form that can be sumitted and sent to an email address.",
		SortOrder = 250,
		IconUrl = "~/Templates/UI/Img/report.png")]
    [AllowedZones("Content", "ColumnLeft", "ColumnRight")]
    [RestrictParents(typeof (AbstractContentPage))]
    [RestrictChildren(typeof (Question))]
    [AvailableZone("Questions", "Questions")]
    [WithEditableTitle(Required = false, ContainerName = Form.ContentTab)]
	[TabContainer(Form.FieldsTab, "Fields", 0, CssClass = "tabPanel formTab")]
	[TabContainer(Form.ContentTab, "Content", 10, CssClass = "tabPanel formTab")]
	[TabContainer(Form.EmailTab, "Email", 20, CssClass = "tabPanel formTab")]
	public class Form : AbstractItem, ISurvey
    {
		public const string FieldsTab = "formFieldsTab";
		public const string ContentTab = "formContentTab";
		public const string EmailTab = "formEmailTab";

        [EditableFreeTextArea("Intro text", 100, ContainerName = ContentTab)]
        public virtual string IntroText
        {
            get { return (string)(GetDetail("IntroText") ?? string.Empty); }
            set { SetDetail("IntroText", value, string.Empty); }
        }

		[EditableFreeTextArea("SubmitText", 140, ContainerName = ContentTab)]
        public virtual string SubmitText
        {
            get { return (string)(GetDetail("SubmitText") ?? string.Empty); }
            set { SetDetail("SubmitText", value, string.Empty); }
        }

		[EditableChildren("Form fields", "Questions", "FormFields", 110, ContainerName = FieldsTab)]
        public virtual IList<ContentItem> FormFields
        {
            get { return GetChildren(); }
        }

		[EditableText("Mail from", 120, ContainerName = EmailTab)]
        public virtual string MailFrom
        {
            get { return (string) (GetDetail("MailFrom") ?? string.Empty); }
            set { SetDetail("MailFrom", value, string.Empty); }
        }

		[EditableText("Mail to", 122, ContainerName = EmailTab)]
        public virtual string MailTo
        {
            get { return (string) (GetDetail("MailTo") ?? string.Empty); }
            set { SetDetail("MailTo", value, string.Empty); }
        }

		[EditableText("Mail subject", 124, ContainerName = EmailTab)]
        public virtual string MailSubject
        {
            get { return (string) (GetDetail("MailSubject") ?? string.Empty); }
            set { SetDetail("MailSubject", value, string.Empty); }
        }

		[EditableText("Mail intro", 126, ContainerName = EmailTab)]
        [EditorModifier("TextMode", TextBoxMode.MultiLine)]
        public virtual string MailBody
        {
            get { return (string) (GetDetail("MailBody") ?? string.Empty); }
            set { SetDetail("MailBody", value, string.Empty); }
        }

        protected override string TemplateName
        {
            get { return "Form"; }
        }
    }
}