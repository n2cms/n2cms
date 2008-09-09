using System.Diagnostics;
using N2.Web.Mail;

namespace N2.Templates.Services
{
	public class FakeMailSender : IMailSender
	{
		public void Send(System.Net.Mail.MailMessage mail)
		{
			Trace.TraceInformation("FakeMailSender: Not sending email message to " + mail.To + " from " + mail.From + ": " + mail.Subject);
            Trace.TraceInformation(mail.Body);
		}

		public void Send(string from, string recipients, string subject, string body)
		{
            Trace.TraceInformation("FakeMailSender: Not sending email message to " + recipients + " from " + from + ": " + subject);
            Trace.TraceInformation(body);
		}
	}
}
