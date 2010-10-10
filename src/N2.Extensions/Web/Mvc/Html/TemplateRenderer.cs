using System;
using System.Collections;
using System.IO;
using System.Security;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using N2.Engine;
using N2.Web.UI;

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
			RouteValueDictionary values = new RouteValueDictionary();
			values[ContentRoute.ControllerKey] = controllerMapper.GetControllerName(item.GetContentType());
			values[ContentRoute.ActionKey] = "index";
			values[ContentRoute.ContentItemKey] = item.ID;

			// retrieve the virtual path so we can figure out if this item is routed through an area
			var vpd = helper.RouteCollection.GetVirtualPath(helper.ViewContext.RequestContext, values);
			if (vpd == null)
				throw new InvalidOperationException("Unable to render " + item + " (" + values.ToQueryString() + " did not match any route)");
			string area = vpd.DataTokens["area"] as string;
			if (!string.IsNullOrEmpty(area))
				values["area"] = vpd.DataTokens["area"];

			helper.RenderAction("index", values);
		}

		private RouteValueDictionary CopyValues(RouteValueDictionary from, RouteValueDictionary to)
		{
			foreach (var kvp in from)
				to[kvp.Key] = kvp.Value;
			return to;
		}
 

 


        class SimpleViewDataContainer : IViewDataContainer
        {
            #region IViewDataContainer Members

            public ViewDataDictionary ViewData { get; set; }
            #endregion
        }

		/// <summary>Replaces the current HttpContext with one that writes to the specified writer</summary>
		/// <seealso cref="http://mikehadlow.blogspot.com/2008/06/mvc-framework-capturing-output-of-view_05.html"/>
		private class HttpContextScope : IDisposable
		{
			private readonly HttpContext _oldContext;

			public HttpContextScope(TextWriter writer)
			{
				// replace the current context with a new context that writes to a string writer
				_oldContext = HttpContext.Current;

				var response = new HttpResponse(writer);
				CurrentContext = new HttpContext(_oldContext.Request, response) { User = _oldContext.User };
				foreach (DictionaryEntry contextItem in _oldContext.Items)
				{
					CurrentContext.Items[contextItem.Key] = contextItem.Value;
				}

				try
				{
					HttpContext.Current = CurrentContext;
				}
				catch(SecurityException)
				{
				}
			}

			public HttpContext CurrentContext { get; private set; }

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			/// <filterpriority>2</filterpriority>
			public void Dispose()
			{
				ReplaceExistingContext();
			}

			private void ReplaceExistingContext()
			{
				try
				{
					// Copy any new/replaced cookies to the old context
					foreach(var cookieName in CurrentContext.Response.Cookies.AllKeys)
					{
						var cookie = CurrentContext.Response.Cookies[cookieName];

						if(cookie != null)
							_oldContext.Response.Cookies.Add(cookie);
					}

					HttpContext.Current = _oldContext;
				}
				catch (SecurityException)
				{
				}
			}
		}

		#region Nested type: DummyViewDataContainer

		private class DummyViewDataContainer : IViewDataContainer
		{
			public DummyViewDataContainer(IItemContainer container)
			{
				if(container is ViewMasterPage)
					ViewData = ((ViewMasterPage) container).ViewData;
				else
					ViewData = new ViewDataDictionary(container.CurrentItem);
			}

			#region IViewDataContainer Members

			public ViewDataDictionary ViewData { get; set; }

			#endregion
		}

		#endregion
	}
}