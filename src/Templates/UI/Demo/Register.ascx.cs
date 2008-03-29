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

namespace Demo
{
	public partial class Register : N2.Web.UI.UserControl<N2.ContentItem, RegisterItem>
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void btnSubmit_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTP.Server"]))
			{
				SendEmail();
			}

			this.ltText.Text = this.CurrentItem.SubmitText;
			this.plhSubmit.Visible = false;
		}

		private void SendEmail()
		{
			string smtpServer = ConfigurationManager.AppSettings["SMTP.Server"];
			string smtpUN = ConfigurationManager.AppSettings["SMTP.UN"];
			string smtpPW = ConfigurationManager.AppSettings["SMTP.PW"];

			MailMessage mm = new MailMessage(this.CurrentItem.From, this.txtEmail.Text);
			if(!string.IsNullOrEmpty(CurrentItem.CC))
				mm.CC.Add(CurrentItem.CC);
			mm.Subject = this.CurrentItem.Subject;
			mm.Body = string.Format(this.CurrentItem.Body, this.txtPraise.Text);

			SmtpClient sc = new SmtpClient(smtpServer);
			sc.Credentials = new System.Net.NetworkCredential(smtpUN, smtpPW);
			sc.Send(mm);
		}
	}
}