using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Demo
{
	[N2.Item("Register to get login", "Register")]
    [N2.Integrity.AllowedZones("Right")]
	[N2.Integrity.AllowedParents(typeof(N2.Templates.Items.IStructuralPage))]
	[N2.Details.WithEditableTitle("Title", 20)]
    [N2.Details.WithEditable("Zone", typeof(N2.Web.UI.WebControls.ZoneSelector), "SelectedValue", 30, "ZoneName")]
	public class RegisterItem : N2.Templates.Items.AbstractItem
	{
		[N2.Details.Editable("Text", typeof(TextBox), "Text", 100)]
		[N2.Web.UI.EditorModifier("TextMode", TextBoxMode.MultiLine)]
		public virtual string Text
		{
			get { return (string)GetDetail("Text"); }
			set { SetDetail("Text", value); }
		}

		[N2.Details.Editable("Submit Text", typeof(TextBox), "Text", 100)]
		[N2.Web.UI.EditorModifier("TextMode", TextBoxMode.MultiLine)]
		public virtual string SubmitText
		{
			get { return (string)GetDetail("SubmitText"); }
			set { SetDetail("SubmitText", value); }
		}

		[N2.Details.Editable("From", typeof(TextBox), "Text", 200)]
		public virtual string From
		{
			get { return (string)GetDetail("From"); }
			set { SetDetail("From", value); }
		}

		[N2.Details.Editable("CC", typeof(TextBox), "Text", 210)]
		//[N2.Web.UI.EditorModifier("Enabled", false)]
		public virtual string CC
		{
			get { return (string)GetDetail("CC"); }
			set { SetDetail("CC", value); }
		}

		[N2.Details.Editable("Subject", typeof(TextBox), "Text", 210)]
		//[N2.Web.UI.EditorModifier("Enabled", false)]
		public virtual string Subject
		{
			get { return (string)GetDetail("Subject"); }
			set { SetDetail("Subject", value); }
		}

		[N2.Details.Editable("Mail Body", typeof(TextBox), "Text", 220)]
		[N2.Web.UI.EditorModifier("TextMode", TextBoxMode.MultiLine)]
		[N2.Web.UI.EditorModifier("Rows", 10)]
		//[N2.Web.UI.EditorModifier("Enabled", false)]
		public virtual string Body
		{
			get { return (string)GetDetail("Body"); }
			set { SetDetail("Body", value); }
		}

		public override string TemplateUrl
		{
			get { return "~/Demo/Register.ascx"; }
		}

		public override string IconUrl
		{
			get
			{
				return "~/Edit/Img/Ico/key.gif";
			}
		}

		public override bool IsPage
		{
			get { return false; }
		}


		//[N2.Details.Editable("Text", typeof(TextBox), "Text", 100)]
		//[N2.Web.UI.EditorModifier("TextMode", TextBoxMode.MultiLine)]
		//public virtual string Text
		//{
		//    get { return (string)GetDetail("Text"); }
		//    set { SetDetail("Text", value); }
		//}

		//[N2.Details.Editable("Submit Text", typeof(TextBox), "Text", 100)]
		//[N2.Web.UI.EditorModifier("TextMode", TextBoxMode.MultiLine)]
		//public virtual string SubmitText
		//{
		//    get { return (string)GetDetail("SubmitText"); }
		//    set { SetDetail("SubmitText", value); }
		//}

		//[N2.Details.Editable("Notification from email", typeof(TextBox), "Text", 200)]
		//public virtual string NotificationFrom
		//{
		//    get { return (string)GetDetail("NotificationFrom"); }
		//    set { SetDetail("NotificationFrom", value); }
		//}

		//[N2.Details.Editable("Notification to email", typeof(TextBox), "Text", 210)]
		//public virtual string NotificationTo
		//{
		//    get { return (string)GetDetail("NotificationTo"); }
		//    set { SetDetail("NotificationTo", value); }
		//}

		////[N2.Details.Editable("Subject", typeof(TextBox), "Text", 210)]
		////public virtual string Subject
		////{
		////    get { return (string)GetDetail("Subject"); }
		////    set { SetDetail("Subject", value); }
		////}

		////[N2.Details.Editable("Mail Body", typeof(TextBox), "Text", 220)]
		////[N2.Web.UI.EditorModifier("TextMode", TextBoxMode.MultiLine)]
		////[N2.Web.UI.EditorModifier("Rows", 10)]
		////public virtual string Body
		////{
		////    get { return (string)GetDetail("Body"); }
		////    set { SetDetail("Body", value); }
		////}

		//public override string TemplateUrl
		//{
		//    get { return "~/Demo/Register.ascx"; }
		//}

		//public override string IconUrl
		//{
		//    get
		//    {
		//        return "~/Edit/Img/Ico/key.gif";
		//    }
		//}

		//public override bool IsPage
		//{
		//    get { return false; }
		//}
	}
}
