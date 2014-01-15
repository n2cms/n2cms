using System;
using System.Net.Mail;

namespace N2.Web.Mail
{
    [Obsolete]
    public class StaticMailSender : SmtpMailSender
    {
        string host;
        int port = 25;
        string user = null;
        string password = null;

        public StaticMailSender()
        {
        }

        public StaticMailSender(string host)
        {
            this.host = host;
        }

        public StaticMailSender(string host, int port)
            : this(host)
        {
            this.port = port;
        }

        public StaticMailSender(string host, int port, string user, string password)
            : this(host, port)
        {
            this.user = user;
            this.password = password;
        }


        protected override SmtpClient GetSmtpClient()
        {
            return CreateSmtpClient(host, port, user, password);
        }
    }
}
