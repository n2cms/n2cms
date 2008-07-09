using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace N2.Web.Mail
{
	public abstract class SmtpMailSender : IMailSender
	{
		public void Send(MailMessage mail)
		{
			SmtpClient client = GetSmtpClient();
			client.Send(mail);
		}

		public void Send(string from, string recipients, string subject, string body)
		{
			SmtpClient client = GetSmtpClient();
			client.Send(from, recipients, subject, body);
		}

		protected abstract SmtpClient GetSmtpClient();

		protected SmtpClient CreateSmtpClient(string host, int port, string user, string password)
		{
            SmtpClient client;

            if (string.IsNullOrEmpty(host))
                client = new SmtpClient();
            else
                client = new SmtpClient(host, port);

			if (!string.IsNullOrEmpty(user))
			{
				client.Credentials = new NetworkCredential(user, password);
			}
			return client;
		}
	}
}
