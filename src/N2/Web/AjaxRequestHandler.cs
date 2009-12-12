using System.Web;
using N2.Web;

namespace N2.Web
{
	/// <summary>
	/// Handles ajax requests.
	/// </summary>
	/// <example>
	/// web.config:
	/// ...
	/// &lt;httpHandlers&gt;
	///		&lt;add path="*.n2.ashx" verb="*" type="N2.Web.AjaxRequestHandler, N2" /&gt;
	///	&lt;/httpHandlers&gt;
	/// ...
	/// </example>
	public class AjaxRequestHandler : IHttpHandler
	{
		private readonly bool isReusable = false;

		public void ProcessRequest(HttpContext context)
		{
			AjaxRequestDispatcher dispatcher = Context.Current.Resolve<AjaxRequestDispatcher>();
			string response = dispatcher.Handle(context);
			context.Response.ContentType = "text/plain";
            //context.Response.CacheControl = "no-cache";
			context.Response.Write(response);
		}

		public bool IsReusable
		{
			get { return isReusable; }
		}
	}
}