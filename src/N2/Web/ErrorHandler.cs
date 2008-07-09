using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Web;
using N2.Configuration;
using N2.Web.Mail;

namespace N2.Web
{
    public class ErrorHandler : N2.Web.IErrorHandler
    {
        ErrorAction action = ErrorAction.None;
        IMailSender mailSender = null;
        string mailTo = null;
        string mailFrom = null;
        long errorCount = 0;

        public long ErrorCount
        {
            get { return errorCount; }
        }

        public ErrorHandler()
        {
        }

        public ErrorHandler(EngineSection config)
        {
            action = config.Errors.Action;
            mailTo = config.Errors.MailTo;
            mailFrom = config.Errors.MailFrom;
        }

        public ErrorHandler(EngineSection config, IMailSender mailSender)
            :this(config)
        {
            this.mailSender = mailSender;
        }

        public void Handle(Exception ex)
        {
            errorCount++;

            if (ex is HttpUnhandledException)
            {
                ex = ex.InnerException;
            }

            if (ex != null)
            {
                Trace.WriteLine("ErrorHandler.Handle: " + ex);
                if (action == ErrorAction.Email)
                {
                    try
                    {
                        if (mailSender != null && !string.IsNullOrEmpty(mailTo))
                            mailSender.Send(mailFrom ?? "errors@somesite.com", mailTo, "Error Report: " + ex.Message, ex.ToString());
                    }
                    catch (Exception ex2)
                    {
                        Trace.WriteLine("ErrorHandler.Handle: exception handling exception" + ex2);
                    }
                }
            }
        }
    }
}
