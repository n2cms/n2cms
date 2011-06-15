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

			public ControllerWrapper(ContentItem item, IControllerMapper controllerMapper)
			{
				this.ID = "cw" + item.ID;
				ViewData = new ViewDataDictionary(item);
				this.item = item;
				this.controllerMapper = controllerMapper;
			}

			public HtmlHelper<ContentItem> Html { get; set; }

			#region IViewDataContainer Members

			public ViewDataDictionary ViewData { get; set; }

			#endregion

			protected override void Render(HtmlTextWriter writer)
			{
				// fake an mvc context and render an mvc part
				var rd = CreateRouteData();
				var vc = CreateViewContext(rd, writer);
				var html = new HtmlHelper<ContentItem>(vc, this);

				RenderTemplate(item, html);
			}

			private void RenderTemplate(ContentItem item, HtmlHelper helper)
			{
				Type itemType = item.GetContentType();
				string controllerName = controllerMapper.GetControllerName(itemType);
				if (string.IsNullOrEmpty(controllerName))
				{
					Trace.TraceWarning("Found no controller for type " + itemType);
					return;
				}

				RouteValueDictionary values = GetRouteValues(helper, item, controllerName);

				helper.ViewContext.Writer.Write("<div id='" + ClientID + "'>");
				helper.RenderAction("Index", values);
				helper.ViewContext.Writer.Write("</div>");
			}

			private static RouteValueDictionary GetRouteValues(HtmlHelper helper, ContentItem item, string controllerName)
			{
				var values = new RouteValueDictionary();
				values[ContentRoute.ControllerKey] = controllerName;
				values[ContentRoute.ActionKey] = "Index";
				values[ContentRoute.ContentItemKey] = item.ID;

				// retrieve the virtual path so we can figure out if this item is routed through an area
				var vpd = helper.RouteCollection.GetVirtualPath(helper.ViewContext.RequestContext, values);
				if (vpd == null)
					throw new InvalidOperationException("Unable to render " + item + " (" + values.ToQueryString() + " did not match any route)");

				values["area"] = vpd.DataTokens["area"];
				return values;
			}

			static int mvcVersion = new System.Reflection.AssemblyName(typeof(WebFormView).Assembly.FullName).Version.Major;
			private ViewContext CreateViewContext(RouteData rd, HtmlTextWriter writer)
			{
				var cc = new ControllerContext(new RequestContext(new HttpContextWrapper(HttpContext.Current), rd), new TempController());
#if NET4
				// TODO: avoid activator below
#endif
				var wfv = mvcVersion >= 3 
					? (WebFormView)Activator.CreateInstance(typeof(WebFormView), cc, this.Page.Request.AppRelativeCurrentExecutionFilePath)
					: (WebFormView)Activator.CreateInstance(typeof(WebFormView), this.Page.Request.AppRelativeCurrentExecutionFilePath);
				var vc = new ViewContext(cc, wfv, new ViewDataDictionary(), new TempDataDictionary(), writer);
				return vc;
			}

			private RouteData CreateRouteData()
			{
				ContentItem page = item;
				var ic = Page as IItemContainer;
				if (ic != null)
					page = ic.CurrentItem;

				var rd = new RouteData();
				rd.ApplyCurrentItem("webform", "Index", page, item);
				return rd;
			}

			class TempController : ControllerBase
			{
				protected override void ExecuteCore()
				{
					throw new NotImplementedException();
				}
			}
		}
	}
}
