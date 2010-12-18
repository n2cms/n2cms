using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Web;
using N2.Configuration;
using N2.Edit.Installation;
using N2.Engine;
using N2.Security;
using N2.Web.Mail;
using NHibernate;

namespace N2.Web
{
	/// <summary>
	/// Is notified of errors in the web context and tries to do something barely useful about them.
	/// </summary>
	[Service(typeof (IErrorHandler))]
	public class ErrorHandler : IErrorHandler
	{
		private readonly EditSection editConfig;
		private readonly ErrorAction action = ErrorAction.None;
		private readonly IWebContext context;
		private readonly bool handleWrongClassException = true;
		private readonly InstallationManager installer;
		private readonly string mailFrom;
		private readonly IMailSender mailSender;
		private readonly string mailTo;

		private readonly int maxErrorReportsPerHour = -1;
		private readonly ISecurityManager security;
		private readonly object syncLock = new object();
		private long errorCount;
		private long errorsThisHour;
		private int hour = DateTime.Now.Hour;

		public ErrorHandler(IWebContext context, ISecurityManager security, InstallationManager installer)
		{
			this.context = context;
			this.security = security;
			this.installer = installer;
		}

		public ErrorHandler(IWebContext context, ISecurityManager security, InstallationManager installer,
		                    EngineSection config, EditSection editConfig)
			: this(context, security, installer)
		{
			this.editConfig = editConfig;
			action = config.Errors.Action;
			mailTo = config.Errors.MailTo;
			mailFrom = config.Errors.MailFrom;
			maxErrorReportsPerHour = config.Errors.MaxErrorReportsPerHour;
			handleWrongClassException = config.Errors.HandleWrongClassException;
			mailSender = new SmtpMailSender();
		}

		public ErrorHandler(IWebContext context, ISecurityManager security, InstallationManager installer,
		                    IMailSender mailSender, EngineSection config, EditSection editConfig)
			: this(context, security, installer, config, editConfig)
		{
			this.mailSender = mailSender;
		}

		/// <summary>Total number of errors since startup.</summary>
		public long ErrorCount
		{
			get { return errorCount; }
		}

		#region IErrorHandler Members

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

		#endregion

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

		private void RedirectToFix(WrongClassException wex)
		{
			string url = Url.Parse(editConfig.Installer.FixClassUrl).ResolveTokens().AppendQuery("id", wex.Identifier);
			context.ClearError();
			context.Response.Redirect(url);
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
			body.Append(ex.ToString());
			return body.ToString();
		}
	}
}