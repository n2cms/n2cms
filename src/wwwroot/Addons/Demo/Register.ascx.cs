using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net.Mail;
using Demo.Items;

namespace Demo
{
	public partial class Register : N2.Web.UI.UserControl<N2.ContentItem, RegisterItem>
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Page.User.IsInRole("Editors") || Page.User.Identity.Name == "admin")
			{
				plhSubmit.Visible = false;
				ltText.Text = CurrentItem.AuthenticatedText;
			}
		}

		protected void btnSubmit_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTP.Server"]))
			{
				SendEmail(txtEmail.Text, CurrentItem.CC, CurrentItem.Subject, CurrentItem.Body);
			}

			this.ltText.Text = this.CurrentItem.SubmitText;
			this.plhSubmit.Visible = false;
		}

		protected void btnAutoLogin_click(object sender, EventArgs e)
		{
			if (FormsAuthentication.Authenticate(CurrentItem.Username, CurrentItem.Password))
			{
				SendEmail(CurrentItem.From, null, "N2 demo autologin", "Someone from " + Request.UserHostAddress + " logged in.");
				FormsAuthentication.RedirectFromLoginPage(CurrentItem.Username, false);
			}
		}

		private void SendEmail(string to, string cc, string subject, string body)
		{
			string smtpServer = ConfigurationManager.AppSettings["SMTP.Server"];
			string smtpUN = ConfigurationManager.AppSettings["SMTP.UN"];
			string smtpPW = ConfigurationManager.AppSettings["SMTP.PW"];

			MailMessage mm = new MailMessage(CurrentItem.From, to);
			if(!string.IsNullOrEmpty(cc))
				mm.CC.Add(cc);
			mm.Subject = subject;
			mm.Body = string.Format(body, this.txtPraise.Text);

			SmtpClient sc = new SmtpClient(smtpServer);
			sc.Credentials = new System.Net.NetworkCredential(smtpUN, smtpPW);
			sc.Send(mm);
		}
	}
}