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

namespace N2.Templates.Mvc.Services
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
			var cw = new ControllerWrapper(item, renderer);
			container.Controls.Add(cw);
			return cw;
		}

		class ControllerWrapper : Control, IViewDataContainer
		{
			ITemplateRenderer renderer;
			ContentItem item;

			public ControllerWrapper(ContentItem item, ITemplateRenderer renderer)
			{
				ViewData = new ViewDataDictionary(item);
				this.renderer = renderer;
				this.item = item;
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
				Html = new HtmlHelper<ContentItem>(vc, this);

				renderer.RenderTemplate(item, Html);
			}

			private ViewContext CreateViewContext(RouteData rd, HtmlTextWriter writer)
			{
				var vc = new ViewContext(
					new ControllerContext(
						new RequestContext(
							new HttpContextWrapper(HttpContext.Current),
							rd),
						new TempController()),
					new WebFormView(this.Page.Request.AppRelativeCurrentExecutionFilePath),
					new ViewDataDictionary(),
					new TempDataDictionary(),
					writer);
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
