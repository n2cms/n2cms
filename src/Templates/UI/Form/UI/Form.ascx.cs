using System;
using System.Net.Mail;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Resources;
using N2.Templates.Items;
using N2.Templates.Survey.Web.UI;
using N2.Templates.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Templates.Form.UI
{
	public partial class Form : TemplateUserControl<AbstractContentPage, Items.Form>
	{
		protected MultiView mv;
		protected Zone zq;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Register.StyleSheet(Page, "~/Form/UI/Css/Form.css", Media.All);
		}

		protected void btnSubmit_Command(object sender, CommandEventArgs args)
		{
			StringBuilder sb = new StringBuilder(CurrentItem.MailBody);
			foreach (Control c in zq.Controls)
			{
				IQuestionControl q = c as IQuestionControl;
				if (q != null)
				{
					sb.AppendFormat("{0}: {1}{2}", q.Question, q.AnswerText, Environment.NewLine);
				}
			}
			MailMessage mm = new MailMessage(CurrentItem.MailFrom, CurrentItem.MailTo);
			mm.Subject = CurrentItem.MailSubject;
			mm.Body = sb.ToString();

			SmtpClient sc = Find.RootItem.GetSmtpClient();
			sc.Send(mm);

			mv.ActiveViewIndex = 1;
		}
	}
}