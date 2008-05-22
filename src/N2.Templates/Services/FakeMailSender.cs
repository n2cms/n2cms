using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace N2.Templates.Services
{
	public class FakeMailSender : IMailSender
	{
		public void Send(System.Net.Mail.MailMessage mail)
		{
			Trace.WriteLine("FakeMailSender: Not sending email message to " + mail.To + " from " + mail.From + ": " + mail.Subject);
			Trace.WriteLine(mail.Body);
		}

		public void Send(string from, string recipients, string subject, string body)
		{
			Trace.WriteLine("FakeMailSender: Not sending email message to " + recipients + " from " + from + ": " + subject);
			Trace.WriteLine(body);
		}
	}
}
