using System;
using System.Web;
using N2.Engine;
using N2.Plugin;

namespace N2.Web
{
    /// <summary>
    /// Handles ajax requests.
    /// </summary>
    /// <example>
    /// web.config:
    /// ...
    /// &lt;httpHandlers&gt;
    ///     &lt;add path="*.n2.ashx" verb="*" type="N2.Web.AjaxRequestHandler, N2" /&gt;
    /// &lt;/httpHandlers&gt;
    /// ...
    /// </example>
    [Service]
    public class AjaxRequestHandler : IHttpHandler, IAutoStart
    {
        AjaxRequestDispatcher dispatcher;
        EventBroker broker;

        public AjaxRequestHandler()
        {
        }

        public AjaxRequestHandler(AjaxRequestDispatcher dispatcher, EventBroker broker)
        {
            this.dispatcher = dispatcher;
            this.broker = broker;
        }



        public bool IsReusable { get; set; }



        public void ProcessRequest(HttpContext context)
        {
            if (dispatcher == null)
                dispatcher = N2.Context.Current.Resolve<AjaxRequestDispatcher>();

            dispatcher.Handle(new HttpContextWrapper(context));
        }

        void PostResolveRequestCache(object sender, EventArgs args)
        {
            HttpApplication app = sender as HttpApplication;
            if (app.Context.Handler != null)
                return;
            if (!app.Context.Request.AppRelativeCurrentExecutionFilePath.EndsWith(".n2.ashx"))
                return;

            app.Context.RemapHandler(this);
        }

        #region IAutoStart Members

        public void Start()
        {
            broker.PostResolveRequestCache += PostResolveRequestCache;
        }

        public void Stop()
        {
            broker.PostResolveRequestCache -= PostResolveRequestCache;
        }

        #endregion
    }
}
