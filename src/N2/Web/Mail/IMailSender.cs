using System;
namespace N2.Web.Mail
{
	[Obsolete]
	public interface IMailSender
	{
		void Send(System.Net.Mail.MailMessage mail);
		void Send(string from, string recipients, string subject, string body);
	}
}
