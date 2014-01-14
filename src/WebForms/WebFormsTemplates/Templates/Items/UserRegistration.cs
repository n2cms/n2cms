using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Security.Details;

namespace N2.Templates.Items
{
    [PartDefinition("Register",
        IconUrl = "~/Templates/UI/Img/user_add.png")]
    [AllowedZones(Zones.Content)]
    [N2.Web.UI.FieldSetContainer("registration", "Registration", 30)]
    [N2.Web.UI.FieldSetContainer("verification", "Verification", 40)]
    public class UserRegistration : AbstractItem
    {
        private const string VerificationBody = @"Please verify your e-mail. Click here:
{VerificationUrl}";

        [EditableLink("Success Page", 10, ContainerName = "registration")]
        public virtual ContentItem SuccessPage
        {
            get { return (ContentItem)GetDetail("SuccessPage"); }
            set { SetDetail("SuccessPage", value); }
        }

        [EditableText("VerificationSubject", 100, ContainerName = "verification")]
        public virtual string VerificationSubject
        {
            get { return (string)(GetDetail("VerificationSubject") ?? string.Empty); }
            set { SetDetail("VerificationSubject", value, string.Empty); }
        }

        [EditableLink("Verified Page", 10, ContainerName = "verification")]
        public virtual ContentItem VerifiedPage
        {
            get { return (ContentItem)GetDetail("VerifiedPage"); }
            set { SetDetail("VerifiedPage", value); }
        }

        [EditableText("VerificationText", 110, TextMode = TextBoxMode.MultiLine, Rows = 4, ContainerName = "verification")]
        public virtual string VerificationText
        {
            get { return (string)(GetDetail("VerificationText") ?? VerificationBody); }
            set { SetDetail("VerificationText", value, VerificationBody); }
        }

        [EditableCheckBox("Require Verification", 90, ContainerName = "verification")]
        public virtual bool RequireVerification
        {
            get { return (bool)(GetDetail("RequireVerification") ?? true); }
            set { SetDetail("RequireVerification", value, true); }
        }

        [EditableText("Verification Sender Email", 120, ContainerName = "verification")]
        public virtual string VerificationSender
        {
            get { return (string)(GetDetail("VerificationSender") ?? string.Empty); }
            set { SetDetail("VerificationSender", value, string.Empty); }
        }

        [EditableRoles(Title = "Assign users to these roles", ContainerName = "registration")]
        public virtual DetailCollection Roles
        {
            get { return GetDetailCollection("Roles", true); }
        }

        protected override string TemplateName
        {
            get { return "Register"; }
        }
    }
}
