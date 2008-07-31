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

        int maxErrorReportsPerHour = -1;
        object syncLock = new object();
        long errorsThisHour = 0;
        int hour = DateTime.Now.Hour;

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
            maxErrorReportsPerHour = config.Errors.MaxErrorReportsPerHour;
            mailSender = new StaticMailSender();
        }

        public ErrorHandler(EngineSection config, IMailSender mailSender)
            :this(config)
        {
            this.mailSender = mailSender;
        }

        public void Notify(Exception ex)
        {
            errorCount++;

            if (ex is HttpUnhandledException)
            {
                ex = ex.InnerException;
            }

            if (ex != null)
            {
                Trace.TraceError("ErrorHandler.Handle: " + FormatError(ex));
                if (action == ErrorAction.Email)
                {
                    try
                    {
                        if (mailSender != null && !string.IsNullOrEmpty(mailTo))
                        {
                            lock (syncLock)
                            {
                                if (DateTime.Now.Hour == hour)
                                {
                                    ++errorsThisHour;
                                }
                                else
                                {
                                    hour = DateTime.Now.Hour;
                                    errorsThisHour = 0;
                                }
                                if (maxErrorReportsPerHour < 0 || errorsThisHour <= maxErrorReportsPerHour)
                                    mailSender.Send(mailFrom ?? "errors@somesite.com", mailTo, "Error Report: " + ex.Message, FormatError(ex));
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        Trace.TraceError("ErrorHandler.Handle: exception handling exception" + ex2);
                    }
                }
            }
        }

        private static string FormatError(Exception ex)
        {
            HttpContext ctx = HttpContext.Current;
            StringBuilder body = new StringBuilder();
            if (ctx != null)
            {
                if (ctx.Request != null)
                {
                    body.Append("Url: ").AppendLine(ctx.Request.RawUrl);
                    if(ctx.Request.UrlReferrer != null)
                        body.Append("Referrer: ").AppendLine(ctx.Request.UrlReferrer.ToString());
                    body.Append("User Address: ").AppendLine(ctx.Request.UserHostAddress);
                }
                if (ctx.User != null)
                {
                    body.Append("User: ").AppendLine(ctx.User.Identity.Name);
                }
            }
            body.Append(ex.ToString());
            return body.ToString();            
        }
    }
}
