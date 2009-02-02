<%@ Page Language="C#" %>
<!-- Please do not delete this file.  It is used to ensure that ASP.NET MVC is activated by IIS when a user makes a "/" request to the server. -->

<script runat="server">
	protected override void OnInit(EventArgs args)
	{
		HttpContext httpContext = HttpContext.Current;
		HttpContextBase contextWrapper = new HttpContextWrapper(httpContext);
		RouteData data = GetRouteData(contextWrapper);
		IHttpHandler handler = data.RouteHandler.GetHttpHandler(new RequestContext(contextWrapper, data));
		handler.ProcessRequest(httpContext);
	}

	private RouteData GetRouteData(HttpContextBase context)
	{
		foreach (RouteBase route in RouteTable.Routes)
		{
			RouteData routeData = route.GetRouteData(context);
			if (routeData != null)
			{
				return routeData;
			}
		}
		return null;
	}
</script>