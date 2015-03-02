using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Templates.Mvc.Models.Pages;
using N2.Web.UI;
using N2.Templates.Mvc.Details;
using N2.Templates.Mvc.Models.Parts.Questions;

namespace N2.Templates.Mvc.Models.Parts
{
    [PartDefinition("Form",
        Description = "A form that can be sumitted and sent to an email address.",
        SortOrder = 251,
        IconUrl = "~/Content/Img/report.png")]
    [RestrictParents(typeof (ContentPageBase))]
    [RestrictChildren(typeof(IQuestion))]
    [AvailableZone("Questions", "Questions")]
    [WithEditableTitle(Required = false, ContainerName = Form.ContentTab)]
    [TabContainer(Form.FieldsTab, "Fields", 0, CssClass = "tabPanel formTab")]
    [TabContainer(Form.ContentTab, "Content", 10, CssClass = "tabPanel formTab")]
    [TabContainer(Form.EmailTab, "Email", 20, CssClass = "tabPanel formTab")]
    public class Form : PartBase, ISurvey
    {
        public const string FieldsTab = "formFieldsTab";
        public const string ContentTab = "formContentTab";
        public const string EmailTab = "formEmailTab";

        [EditableFreeTextArea("Intro text", 100, ContainerName = ContentTab)]
        public virtual string IntroText
        {
            get { return (string) (GetDetail("IntroText") ?? string.Empty); }
            set { SetDetail("IntroText", value, string.Empty); }
        }

        [EditableFreeTextArea("SubmitText", 140, ContainerName = ContentTab)]
        public virtual string SubmitText
        {
            get { return (string) (GetDetail("SubmitText") ?? string.Empty); }
            set { SetDetail("SubmitText", value, string.Empty); }
        }

        [EditableChildren("Form fields", "Questions", "FormFields", 110, ContainerName = FieldsTab)]
        public virtual IEnumerable<IQuestion> FormFields
        {
            get { return Children.OfType<IQuestion>(); }
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
    }
}
