using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Web.Parts;
using N2.Engine;
using N2.Web.Mvc.Html;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.UI;
using N2.Web.Mvc;
using N2.Web.UI;
using N2.Definitions;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.IO;
using N2.Web;

namespace N2.Templates.Mvc.Services
{
	[Adapts(typeof(IWebFormsAddable))]
	public class MvcOnWebFormPartAdapter : PartsAdapter
	{
		IControllerMapper controllerMapper;



		public MvcOnWebFormPartAdapter(IControllerMapper controllerMapper)
		{
			this.controllerMapper = controllerMapper;
		}



		public override Control AddTo(ContentItem item, Control container)
		{
			Panel p = new Panel();
			p.ID = "placeholder" + item.ID;
			container.Controls.Add(p);

			container.Page.InitComplete += (s,a) =>
				{
					var cw = new ControllerWrapper(item, controllerMapper);
					container.Page.Controls.Add(cw);
					container.Page.ClientScript.RegisterStartupScript(typeof(MvcOnWebFormPartAdapter),
						"i" + item.ID,
						 @"handleExternalForm('" + cw.ClientID + "', '" + p.ClientID + "');",
						 addScriptTags: true);
				};
			return p;
		}



		class ControllerWrapper : Control, IViewDataContainer
		{
			IControllerMapper controllerMapper;
			ContentItem item;
			static int mvcVersion = new System.Reflection.AssemblyName(typeof(WebFormView).Assembly.FullName).Version.Major;
			private string controllerName;
			private Type itemType;
			RouteCollection routes;
			private HttpContextWrapper httpContext;

			public ControllerWrapper(ContentItem item, IControllerMapper controllerMapper)
			{
				this.ID = "cw" + item.ID;
				ViewData = new ViewDataDictionary(item);
				this.item = item;
				this.controllerMapper = controllerMapper;
				itemType = item.GetContentType();
				controllerName = controllerMapper.GetControllerName(itemType);
				routes = RouteTable.Routes;
				httpContext = new HttpContextWrapper(HttpContext.Current);
			}

			protected override void OnInit(EventArgs e)
			{
				base.OnInit(e);

				if (Page.Request.Headers["X-Requested-With"] == "XMLHttpRequest" && IsRouteForThisController())
				{
					RenderController(httpContext.Response.Output);
					httpContext.Response.End();
				}
			}

			#region IViewDataContainer Members

			public ViewDataDictionary ViewData { get; set; }

			#endregion

			protected override void Render(HtmlTextWriter writer)
			{
				writer.Write("<div id='" + ClientID + "'>");
				RenderController(writer);
				writer.Write("</div>");
			}

			private void RenderController(TextWriter writer)
			{
				if (string.IsNullOrEmpty(controllerName))
				{
					N2.Engine.Logger.WarnFormat("Found no controller for type {0}", itemType);
					return;
				}

				RouteData data = null;
				if (IsRouteForThisController())
					// post to mvc part
					data = routes.GetRouteData(httpContext);

				if (data == null)
					data = GetRouteData(new RequestContext(httpContext, new RouteData()), item, controllerName);

				var rc = new RequestContext(httpContext, data);
				var controllerFactory = ControllerBuilder.Current.GetControllerFactory();
				var c = controllerFactory.CreateController(rc, (string)data.Values[ContentRoute.ControllerKey]);
				c.Execute(rc);
				controllerFactory.ReleaseController(c);
			}

			private bool IsRouteForThisController()
			{
				return httpContext.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/" + controllerName);
			}

			private RouteData GetRouteData(RequestContext requestContext, ContentItem item, string controllerName)
			{
                var data = new RouteData();
				data.Values[ContentRoute.ControllerKey] = controllerName;
				data.Values[ContentRoute.ActionKey] = "Index";
				data.Values[ContentRoute.ContentItemKey] = item.ID;

				// retrieve the virtual path so we can figure out if this item is routed through an area
				var vpd = routes.GetVirtualPath(requestContext, data.Values);
				if (vpd == null)
					throw new InvalidOperationException("Unable to render " + item + " (" + data.Values.ToQueryString() + " did not match any route)");

				data.Values["area"] = vpd.DataTokens["area"];
                data.Route = vpd.Route;
				data.ApplyCurrentPath(new PathData(item.ClosestPage(), item));
				return data;
			}
		}
	}
}
