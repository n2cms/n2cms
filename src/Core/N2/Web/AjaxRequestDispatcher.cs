using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Text;
using System.Web;
using Castle.MicroKernel;
using N2.Security;

namespace N2.Web
{
	/// <summary>
	/// Dispatches an ajax request to the handler specified by the action 
	/// parameter.
	/// </summary>
	public class AjaxRequestDispatcher
	{
		private readonly ISecurityManager security;
		private readonly IDictionary<string, IAjaxService> handlers = new Dictionary<string, IAjaxService>();

		public AjaxRequestDispatcher(ISecurityManager security)
		{
			this.security = security;
		}

		public IDictionary<string, IAjaxService> Handlers
		{
			get { return handlers; }
		}

		public void AddHandler(IAjaxService service)
		{
			handlers[service.Name] = service;
		}

		public void RemoveHandler(IAjaxService service)
		{
			handlers.Remove(service.Name);
		}

		public virtual string Handle(HttpContext context)
		{
			try
			{
				string action = context.Request["action"];
				if (Handlers.ContainsKey(action))
				{
					IAjaxService service = Handlers[action];

					if (service.RequiresEditAccess && !security.IsEditor(context.User))
						throw new PermissionDeniedException(null, context.User);

					string responseText = service.Handle(context.Request.QueryString);
					return responseText;
				}
				else
					throw new N2Exception("Couln't find any handler for the action: " + action);
			}
			catch(Exception ex)
			{
				return WriteException(ex, context.User);
			}
		}

		protected string WriteException(Exception ex, IPrincipal user)
		{
			string format = "{{error:true, message:\"{0}\"}}";
			if (security.IsAdmin(user))
				format += @"
/*
{1}
*/";
			return string.Format(format,
			                     ex.Message,
			                     ex.ToString().Replace(Environment.NewLine, "\\r\\n").Replace("'", "\"")
				);
		}
	}
}