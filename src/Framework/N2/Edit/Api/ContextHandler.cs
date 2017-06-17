using System;
using System.Net;
using System.Web;
using N2.Edit;
using N2.Edit.Installation;
using N2.Engine;
using N2.Web;

namespace N2.Management.Api
{
	[Service(typeof(IApiHandler))]
	public class ContextHandler : IHttpHandler, IApiHandler
	{
		private readonly IEngine engine;

		public ContextHandler()
			: this(Context.Current)
		{
		}

		public ContextHandler(IEngine engine)
		{
			this.engine = engine;
		}

		//private SelectionUtility Selection => engine.RequestContext.HttpContext.GetSelectionUtility(engine);
		private SelectionUtility Selection { get { return engine.RequestContext.HttpContext.GetSelectionUtility(engine); } }

		public void ProcessRequest(HttpContextBase context)
		{
			switch (context.Request.QueryString["mode"])
			{
				case "interface":
					context.Response.WriteJson(engine.Resolve<InterfaceBuilder>().GetInterfaceDefinition(context, Selection));
					return;
				case "full":
					try
					{
						var Interface = engine.Resolve<InterfaceBuilder>().GetInterfaceDefinition(context, Selection);
						var Context = engine.Resolve<ContextBuilder>().GetInterfaceContextData(context, Selection);
						context.Response.WriteJson(new
						{
							Interface,
							Context
						});
					}
					catch (Exception ex)
					{
						context.Response.Write(ex.ToString());
						context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					}
					return;
				case "messages":
					context.Response.WriteJson(engine.Resolve<ContextBuilder>().GetMessages(context, Selection));
					return;
				case "status":
					var status = engine.Resolve<InstallationManager>().GetStatus();
					context.Response.WriteJson(new
					{
						Running = status.Level == SystemStatusLevel.UpAndRunning,
						status.Level,
						Message = status.Level == SystemStatusLevel.UpAndRunning ? "All systems nominal" : status.ToStatusString()
					});
					return;
				default:
					context.Response.WriteJson(engine.Resolve<ContextBuilder>().GetInterfaceContextData(context, Selection));
					return;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			ProcessRequest(context.GetHttpContextBase());
		}

		public bool IsReusable => false;
	}
}
