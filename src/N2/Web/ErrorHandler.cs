using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Web;
using N2.Configuration;
using N2.Web.Mail;
using NHibernate;
using N2.Security;
using N2.Persistence.NH;
using N2.Installation;
using N2.Edit.Installation;
using N2.Engine;
using System.Reflection;

namespace N2.Web
{
    /// <summary>
    /// Is notified of errors in the web context and tries to do something barely useful about them.
    /// </summary>
	[Service(typeof(IErrorHandler))]
    public class ErrorHandler : IErrorHandler
    {
        ISecurityManager security;
    	readonly InstallationManager installer;
    	IWebContext context;

        ErrorAction action = ErrorAction.None;
        IMailSender mailSender = null;
        string mailTo = null;
        string mailFrom = null;
        long errorCount = 0;

        int maxErrorReportsPerHour = -1;
        object syncLock = new object();
        long errorsThisHour = 0;
        int hour = DateTime.Now.Hour;
        bool handleWrongClassException = true;

        /// <summary>Total number of errors since startup.</summary>
        public long ErrorCount
        {
            get { return errorCount; }
        }

        public ErrorHandler(IWebContext context, ISecurityManager security, InstallationManager installer)
        {
            this.context = context;
            this.security = security;
        	this.installer = installer;
        }

        public ErrorHandler(IWebContext context, ISecurityManager security, InstallationManager installer, EngineSection config)
            : this(context, security, installer)
        {
            action = config.Errors.Action;
            mailTo = config.Errors.MailTo;
            mailFrom = config.Errors.MailFrom;
            maxErrorReportsPerHour = config.Errors.MaxErrorReportsPerHour;
            handleWrongClassException = config.Errors.HandleWrongClassException;
            mailSender = new StaticMailSender();
        }

		public ErrorHandler(IWebContext context, ISecurityManager security, InstallationManager installer, IMailSender mailSender, EngineSection config)
            :this(context, security, installer, config)
        {
            this.mailSender = mailSender;
        }

        public void Notify(Exception ex)
        {
            errorCount++;

            if (ex is HttpUnhandledException)
                ex = ex.InnerException;
			if (ex is TargetInvocationException)
				ex = ex.InnerException;
			
            if (ex != null)
            {
                Trace.TraceError("ErrorHandler.Notify: " + FormatError(ex));
                
                UpdateErrorCount();
                if (action == ErrorAction.Email)
                {
                    TryNotifyingByEmail(ex);
                }
                if (handleWrongClassException)
                {
                    TryHandleWrongClassException(ex);
                }
            }
        }

        private void UpdateErrorCount()
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
            }
        }

        private void TryNotifyingByEmail(Exception ex)
        {
            if (mailSender != null && !string.IsNullOrEmpty(mailTo))
            {
                try
                {
                    if (maxErrorReportsPerHour < 0 || errorsThisHour <= maxErrorReportsPerHour)
                        mailSender.Send(mailFrom ?? "errors@somesite.com", mailTo, "Error Report: " + ex.Message, FormatError(ex));
                }
                catch (Exception ex2)
                {
                    Trace.TraceError("ErrorHandler.Handle: exception handling exception" + ex2);
                }
            }
        }

        private void TryHandleWrongClassException(Exception ex)
        {
			if (ex == null)
				return;

			if (!security.IsAdmin(context.User))
				return;

            if (ex is WrongClassException)
            {
            	WrongClassException wex = ex as WrongClassException;
            	RedirectToFix(wex);
				return;
            }
        	if (ex is LazyInitializationException)
			{
				try
				{
					installer.CheckDatabase();
				}
				catch(WrongClassException wex)
				{
					RedirectToFix(wex);
				}
				catch
				{
				}
				return;
			}
			
			// recusively check inner exception to handle exceptions within child requests
			TryHandleWrongClassException(ex.InnerException);
        }

    	void RedirectToFix(WrongClassException wex)
    	{
    		string url = Url.Parse("~/N2/Installation/FixClass.aspx").AppendQuery("id", wex.Identifier);
    		context.Response.Redirect(url);
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
