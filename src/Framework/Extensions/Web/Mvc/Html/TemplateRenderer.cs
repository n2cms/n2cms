using System;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using N2.Engine;

namespace N2.Web.Mvc.Html
{
	public interface ITemplateRenderer
	{
        void RenderTemplate(ContentItem item, HtmlHelper helper);
	}

	[Service(typeof(ITemplateRenderer))]
	public class TemplateRenderer : ITemplateRenderer
	{
		private readonly IControllerMapper controllerMapper;

		public TemplateRenderer(IControllerMapper controllerMapper)
		{
			this.controllerMapper = controllerMapper;
		}

		private static string ItemOverrideKey { get { return "Override." + ContentRoute.ContentPartKey; } }

		public void RenderTemplate(ContentItem item, HtmlHelper helper)
		{
			Type itemType = item.GetContentType();
			string controllerName = controllerMapper.GetControllerName(itemType);
			if(string.IsNullOrEmpty(controllerName))
			{
				Trace.TraceWarning("Found no controller for type " + itemType);
				return;
			}

			RouteValueDictionary values = GetRouteValues(helper, item, controllerName);

			try
			{
				helper.ViewContext.RouteData.DataTokens[ItemOverrideKey] = item;
				helper.RenderAction("Index", values);
			}
			finally
			{
				if (helper.ViewContext.RouteData.DataTokens.ContainsKey(ItemOverrideKey))
					helper.ViewContext.RouteData.DataTokens.Remove(ItemOverrideKey);
			}
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
	}
}