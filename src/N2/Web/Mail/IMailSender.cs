using System;
namespace N2.Web.Mail
{
	public interface IMailSender
	{
		void Send(System.Net.Mail.MailMessage mail);
		void Send(string from, string recipients, string subject, string body);
	}
}
