using System.Net.Mail;
using N2.Persistence;
using N2.Web;
using N2.Web.Mail;

namespace N2.Templates.Services
{
	/// <summary>
	/// Sends the mail message through the smtp server configured on the root page.
	/// </summary>
	public class DynamicMailSender : SmtpMailSender
	{
		IPersister persister;
		IHost host;

		public DynamicMailSender(IPersister persister, IHost host)
		{
			this.persister = persister;
			this.host = host;
		}

		protected override SmtpClient GetSmtpClient()
		{
			ContentItem root = persister.Get(host.CurrentSite.RootItemID);

			string smtpHost = root["SmtpHost"] as string;
			int port = (int)(root["SmtpPort"] != null ? root["SmtpPort"] : 25);
			string user = root["SmtpUser"] as string;
			string password = root["SmtpPassword"] as string;

			return CreateSmtpClient(smtpHost, port, user, password);
		}
	}
}
