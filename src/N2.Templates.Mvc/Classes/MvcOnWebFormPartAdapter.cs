using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Web.Parts;
using N2.Engine;
using N2.Web.Mvc.Html;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using N2.Web.Mvc;
using N2.Web.UI;

namespace N2.Templates.Mvc.Classes
{
	[Adapts(typeof(Models.Parts.AbstractItem))]
	public class MvcOnWebFormPartAdapter : PartsAdapter
	{
		ITemplateRenderer renderer;
		public MvcOnWebFormPartAdapter(ITemplateRenderer renderer)
		{
			this.renderer = renderer;
		}

		public override Control AddTo(ContentItem item, Control container)
		{
			// fake an mvc context and render an mvc part
			var rd = CreateRouteData(item, container);
			var vc = CreateViewContext(container, rd);			
			var html = renderer.RenderTemplate(item, vc);

			var lc = new LiteralControl(html);
			container.Controls.Add(lc);
			return lc;
		}

		private static ViewContext CreateViewContext(Control container, RouteData rd)
		{
			var vc = new ViewContext(
				new ControllerContext(
					new RequestContext(
						new HttpContextWrapper(HttpContext.Current),
						rd),
					new TempController()),
				new WebFormView(container.Page.Request.AppRelativeCurrentExecutionFilePath),
				new ViewDataDictionary(),
				new TempDataDictionary(),
				container.Page.Response.Output);
			return vc;
		}

		private static RouteData CreateRouteData(ContentItem item, Control container)
		{
			ContentItem page = item;
			var ic = container.Page as IItemContainer;
			if (ic != null)
				page = ic.CurrentItem;

			var rd = new RouteData();
			rd.ApplyCurrentItem("webform", "index", item, page, item);
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
