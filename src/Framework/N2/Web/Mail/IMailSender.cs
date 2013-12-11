namespace N2.Web.Mail
{
    /// <summary>
    /// Sends email messages.
    /// </summary>
    public interface IMailSender
    {
        /// <summary>Sends the given mail message.</summary>
        /// <param name="mail">The mail to send.</param>
        void Send(System.Net.Mail.MailMessage mail);

        void Send(string from, string recipients, string subject, string body);
    }
}
