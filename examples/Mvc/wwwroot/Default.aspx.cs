using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Routing;

namespace MvcTest
{
	public partial class Default : Page
	{
        protected override void OnInit(EventArgs args)
        {
            HttpContext httpContext = HttpContext.Current;
            HttpContextBase contextWrapper = new HttpContextWrapper(httpContext);
            RouteData data = GetRouteData(contextWrapper);
            data.RouteHandler.GetHttpHandler(new RequestContext(contextWrapper, data)).ProcessRequest(httpContext);
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
	}
}
