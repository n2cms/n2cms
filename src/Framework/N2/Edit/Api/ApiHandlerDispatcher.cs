using System;
using System.Linq;
using System.Web;
using N2.Web;

namespace N2.Management.Api
{
	public class ApiHandlerDispatcher : IHttpHandler
	{
		public bool IsReusable => true;

		public void ProcessRequest(HttpContext context)
		{
			var dir = Url.ResolveTokens("{ManagementUrl}/Api/");
			if (!context.Request.FilePath.StartsWith(dir, StringComparison.InvariantCultureIgnoreCase))
				return;

			var name = context.Request.FilePath.Substring(dir.Length).Replace(".ashx", "Handler");

			// Use case insensitive match when finding the handler which matches the request
			var handler =
				Context.Current.Container.ResolveAll<IApiHandler>()
					.FirstOrDefault(h => h.GetType().Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

			handler?.ProcessRequest(context.GetHttpContextBase());
		}
	}
}
