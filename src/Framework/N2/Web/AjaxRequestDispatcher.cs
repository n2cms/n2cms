using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Web;
using N2.Engine;
using N2.Security;

namespace N2.Web
{
    /// <summary>
    /// Dispatches an ajax request to the handler specified by the action 
    /// parameter.
    /// </summary>
    [Service]
    public class AjaxRequestDispatcher
    {
        private readonly ISecurityManager security;
        private readonly IDictionary<string, IAjaxService> handlers = new Dictionary<string, IAjaxService>();

        public AjaxRequestDispatcher(ISecurityManager security, IAjaxService[] ajaxHandler)
        {
            this.security = security;
            foreach (var handler in ajaxHandler)
                AddHandler(handler);
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

        public virtual void Handle(HttpContextBase context)
        {
            try
            {
                string action = context.Request["action"];
                if (!Handlers.ContainsKey(action))
                    throw new InvalidOperationException("Couln't find any handler for the action: " + action);

                IAjaxService service = Handlers[action];

                if (service.RequiresEditAccess && !security.IsEditor(context.User))
                    throw new PermissionDeniedException(null, context.User);

                if (!service.IsValidHttpMethod(context.Request.HttpMethod))
                    throw new HttpException((int)HttpStatusCode.MethodNotAllowed, "This service requires HTTP POST method");

                service.Handle(context);
            }
            catch (Exception ex)
            {
                context.Response.Status = ((int)HttpStatusCode.InternalServerError).ToString() + " Internal Server Error";
                context.Response.Write(WriteException(ex, context.User));
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
