using System;
using System.Reflection;
using System.Text;
using System.Web;
using N2.Configuration;
using N2.Edit.Installation;
using N2.Engine;
using N2.Plugin;
using N2.Web.Mail;
using NHibernate;
using System.Data.SqlClient;

namespace N2.Web
{
    [Obsolete("Use IErrorNotifier")]
    public interface IErrorHandler
    {
        void Notify(Exception ex);
    }

    [Obsolete("Use IErrorNotifier")]
    [Service(typeof(IErrorHandler))]
    public class ObsoleteErrorHandler : IErrorHandler
    {
        IErrorNotifier notifier;

        public ObsoleteErrorHandler(IErrorNotifier notifier)
        {
            this.notifier = notifier;
        }

        #region IErrorHandler Members

        public void Notify(Exception ex)
        {
            notifier.Notify(ex);
        }

        #endregion
    }

    [Service(typeof (IErrorNotifier))]
    public class ErrorNotifier : IErrorNotifier
    {
        #region IErrorNotifier Members

        public void Notify(Exception ex)
        {
            if (ErrorOccured != null)
                ErrorOccured(this, new ErrorEventArgs { Error = ex });
        }

        public event EventHandler<ErrorEventArgs> ErrorOccured;

        #endregion
    }

    /// <summary>
    /// Is notified of errors in the web context and tries to do something barely useful about them.
    /// </summary>
    [Service]
    public class ErrorHandler : IAutoStart
    {
        private readonly Logger<ErrorHandler> logger;
        private readonly IErrorNotifier notifier;
        private readonly ErrorAction action = ErrorAction.None;
        private readonly IWebContext context;
        private readonly bool handleWrongClassException = true;
        private readonly InstallationManager installer;
        private readonly IExceptionFilter exceptionFilter;
        private readonly string mailFrom;
        private readonly IMailSender mailSender;
        private readonly string mailTo;

        private string beginUrl;
        private readonly int maxErrorReportsPerHour = -1;
        private readonly object syncLock = new object();
        private long errorCount;
        private long errorsThisHour;
        private int hour = N2.Utility.CurrentTime().Hour;
        private bool handleSqlException;

        public ErrorHandler(IErrorNotifier notifier, IWebContext context, InstallationManager installer, IExceptionFilter exceptionFilter, IMailSender mailSender, 
            ConfigurationManagerWrapper configuration)
        {
            this.notifier = notifier;
            this.context = context;
            this.installer = installer;
            this.exceptionFilter = exceptionFilter;
            this.mailSender = mailSender;

            beginUrl = configuration.Sections.Management.Installer.WelcomeUrl;
            action = configuration.Sections.Engine.Errors.Action;
            mailTo = configuration.Sections.Engine.Errors.MailTo;
            mailFrom = configuration.Sections.Engine.Errors.MailFrom;
            maxErrorReportsPerHour = configuration.Sections.Engine.Errors.MaxErrorReportsPerHour;
            handleWrongClassException = configuration.Sections.Engine.Errors.HandleWrongClassException;
            handleSqlException = configuration.Sections.Engine.Errors.SqlExceptionHandling == ExceptionResolutionMode.RefreshGet;
        }

        /// <summary>Total number of errors since startup.</summary>
        public long ErrorCount
        {
            get { return errorCount; }
        }

        #region IErrorHandler Members

        public void Handle(object sender, ErrorEventArgs e)
        {
            errorCount++;

            Exception ex = e.Error;
            if (ex != null)
            {
                Logger.Error("ErrorHandler.Notify: " + FormatError(ex));

                UpdateErrorCount();
                if (action == ErrorAction.Email)
                {
                    if (exceptionFilter.IsMessageworthy(ex))
                    {
                        TryNotifyingByEmail(ex);
                    }
                }
                if (handleWrongClassException)
                {
                    if (ex is HttpUnhandledException)
                        ex = ex.InnerException;
                    if (ex is HttpException)
                        ex = ex.InnerException;
                    if (ex is TargetInvocationException)
                        ex = ex.InnerException;

                    TryHandleWrongClassException(ex);
                }
                if (handleSqlException)
                {
                    TryHandleSqlException(ex);
                }
            }
        }

        #endregion

        private void UpdateErrorCount()
        {
            lock (syncLock)
            {
                if (N2.Utility.CurrentTime().Hour == hour)
                {
                    ++errorsThisHour;
                }
                else
                {
                    hour = N2.Utility.CurrentTime().Hour;
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
                    Logger.Error("ErrorHandler.Handle: exception handling exception", ex2);
                }
            }
        }

        private void TryHandleWrongClassException(Exception ex)
        {
            if (ex == null)
                return;

            if (ex is WrongClassException)
            {
                var wex = ex as WrongClassException;
                RedirectToFix(wex);
                return;
            }
            if (ex is LazyInitializationException)
            {
                try
                {
                    installer.CheckDatabase();
                }
                catch (WrongClassException wex)
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

        private bool TryHandleSqlException(Exception ex)
        {
            var sex = ex as SqlException;
            if (sex != null)
            {
                if (!context.IsWeb || context.HttpContext == null || context.HttpContext.Request == null)
                    return false;
                if (context.HttpContext.Request.HttpMethod != "GET")
                    return false;

                // this is a way we try to recover when lots of writes are going on
                if (sex.Number == 1205 && string.IsNullOrEmpty(context.HttpContext.Request["dex"]))
                    context.HttpContext.Response.Redirect(context.HttpContext.Request.RawUrl.ToUrl().SetQueryParameter("dex", 1));
                if (sex.Number == -2 && string.IsNullOrEmpty(context.HttpContext.Request["tex"]))
                    context.HttpContext.Response.Redirect(context.HttpContext.Request.RawUrl.ToUrl().SetQueryParameter("tex", 1));
                return true;
            }

            if (ex is HttpUnhandledException)
                return TryHandleSqlException(ex.InnerException);
            if (ex is HttpException)
                return TryHandleSqlException(ex.InnerException);
            if (ex is TargetInvocationException)
                return TryHandleSqlException(ex.InnerException);
            if (ex is ADOException)
                return TryHandleSqlException(ex.InnerException);

            return false;
        }

        private void RedirectToFix(WrongClassException wex)
        {
            if (context.HttpContext != null)
            {
                string url = Url.Parse(beginUrl).ResolveTokens()
                    .AppendQuery("action", "fixClass")
                    .AppendQuery("id", wex.Identifier);
                logger.Warn("Redirecting to '" + url + "' to fix exception: " + wex);
                context.HttpContext.ClearError();
                context.HttpContext.Response.Redirect(url);
            }
        }

        private static string FormatError(Exception ex)
        {
            HttpContext ctx = HttpContext.Current;
            var body = new StringBuilder();
            if (ctx != null)
            {
                if (ctx.Request != null)
                {
                    body.Append("Url: ").AppendLine(ctx.Request.Url.ToString());
                    if (ctx.Request.UrlReferrer != null)
                        body.Append("Referrer: ").AppendLine(ctx.Request.UrlReferrer.ToString());
                    body.Append("User Address: ").AppendLine(ctx.Request.UserHostAddress);
                }
                if (ctx.User != null)
                {
                    body.Append("User: ").AppendLine(ctx.User.Identity.Name);
                }
            }
            body.Append(ex);
            return body.ToString();
        }

        #region IAutoStart Members

        public void Start()
        {
            notifier.ErrorOccured += Handle;
        }

        public void Stop()
        {
            notifier.ErrorOccured -= Handle;
        }

        #endregion
    }
}
