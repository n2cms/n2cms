using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace N2.Templates.Services
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
			if (string.IsNullOrEmpty(host)) throw new ArgumentNullException("host");

			SmtpClient client = new SmtpClient(host, port);
			if (!string.IsNullOrEmpty(user))
			{
				client.Credentials = new NetworkCredential(user, password);
			}
			return client;
		}
	}
}
