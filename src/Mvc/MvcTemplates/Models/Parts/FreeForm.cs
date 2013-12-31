using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;
using N2.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Templates.Mvc.Models.Parts
{
    [PartDefinition("Free form",
        Description = "A form that can be sumitted and sent to an email address or viewed online.",
        SortOrder = 250,
        IconUrl = "~/Content/Img/report.png")]
    [FieldSetContainer("Email", "Email", 180)]
    public class FreeForm : PartBase
    {
        [EditableFreeTextArea("Form", 100)]
        [DisplayableTokens]
        public virtual string Form { get; set; }

        // submit

        [EditableFreeTextArea("SubmitText", 140)]
        public virtual string SubmitText { get; set; }

        // email

        [EditableText("Mail from", 120, ContainerName = "Email")]
        public virtual string MailFrom
        {
            get { return (string)(GetDetail("MailFrom") ?? string.Empty); }
            set { SetDetail("MailFrom", value, string.Empty); }
        }

        [EditableText("Mail to", 122, ContainerName = "Email")]
        public virtual string MailTo
        {
            get { return (string)(GetDetail("MailTo") ?? string.Empty); }
            set { SetDetail("MailTo", value, string.Empty); }
        }

        [EditableText("Mail subject", 124, ContainerName = "Email")]
        public virtual string MailSubject
        {
            get { return (string)(GetDetail("MailSubject") ?? string.Empty); }
            set { SetDetail("MailSubject", value, string.Empty); }
        }

        [EditableText("Mail intro", 126, ContainerName = "Email")]
        [EditorModifier("TextMode", TextBoxMode.MultiLine)]
        public virtual string MailBody
        {
            get { return (string)(GetDetail("MailBody") ?? string.Empty); }
            set { SetDetail("MailBody", value, string.Empty); }
        }
    }
}
