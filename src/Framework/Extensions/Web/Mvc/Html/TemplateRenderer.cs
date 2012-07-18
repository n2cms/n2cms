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


		public void RenderTemplate(ContentItem item, HtmlHelper helper)
		{
			RouteValueDictionary values = GetRouteValues(helper, item);

			if (values == null)
				return;

			var currentPath = helper.ViewContext.RouteData.CurrentPath();
			try
			{
				var newPath = currentPath.Clone(currentPath.CurrentPage, item);
				helper.ViewContext.RouteData.ApplyCurrentPath(newPath);
				helper.RenderAction("Index", values);
			}
			finally
			{
				helper.ViewContext.RouteData.ApplyCurrentPath(currentPath);
			}
		}

		private RouteValueDictionary GetRouteValues(HtmlHelper helper, ContentItem item)
		{
			Type itemType = item.GetContentType();
			string controllerName = controllerMapper.GetControllerName(itemType);
			if (string.IsNullOrEmpty(controllerName))
			{
				Engine.Logger.WarnFormat("Found no controller for type {0}", itemType);
				return null;
			}

			var values = new RouteValueDictionary();
			values[ContentRoute.ActionKey] = "Index";
			values[ContentRoute.ControllerKey] = controllerName;
			values[ContentRoute.ContentItemKey] = item;

			// retrieve the virtual path so we can figure out if this item is routed through an area
			var vpd = helper.RouteCollection.GetVirtualPath(helper.ViewContext.RequestContext, values);
			if (vpd == null)
				throw new InvalidOperationException("Unable to render " + item + " (" + controllerName + " did not match any route)");

			values["area"] = vpd.DataTokens["area"];
			return values;
		}
	}
}