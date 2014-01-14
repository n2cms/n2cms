using N2.Details;
using N2.Integrity;
using N2.Web.UI.WebControls;

namespace N2.Templates.Mvc.Models.Parts
{
    [PartDefinition("Login", Name = "Login",
        IconUrl = "~/Content/Img/key.png")]
    [WithEditableTitle("Title", 10)]
    [RestrictCardinality]
    public class LoginItem : PartBase
    {
        [DisplayableHeading(4)]
        public override string Title
        {
            get { return base.Title; }
            set { base.Title = value; }
        }

        [EditableFreeTextArea("Text", 100)]
        public virtual string Text
        {
            get { return (string) (GetDetail("Text") ?? string.Empty); }
            set { SetDetail("Text", value, string.Empty); }
        }

        [EditableText("Failure Text", 110)]
        public virtual string FailureText
        {
            get { return (string) (GetDetail("FailureText") ?? string.Empty); }
            set { SetDetail("FailureText", value, string.Empty); }
        }

        [EditableText("Logout Text", 120)]
        public virtual string LogoutText
        {
            get { return GetDetail("LogoutText", "Logout"); }
            set { SetDetail("LogoutText", value, string.Empty); }
        }

        [EditableLink("Logout page", 135)]
        public virtual ContentItem LogoutPage
        {
            get { return (ContentItem) GetDetail("LogoutPage"); }
            set { SetDetail("LogoutPage", value); }
        }

        [EditableLink("Register page", 130)]
        public virtual ContentItem RegisterPage
        {
            get { return (ContentItem) GetDetail("RegisterPage"); }
            set { SetDetail("RegisterPage", value); }
        }

        [EditableLink("Login page", 140)]
        public virtual ContentItem LoginPage
        {
            get { return (ContentItem) GetDetail("LoginPage"); }
            set { SetDetail("LoginPage", value); }
        }
    }
}
