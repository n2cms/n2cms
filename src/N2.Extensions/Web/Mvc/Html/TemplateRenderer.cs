using System;
using System.Collections;
using System.IO;
using System.Security;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Web.Mvc;
using N2.Engine;
using N2.Web.UI;

namespace N2.Web.Mvc.Html
{
	public interface ITemplateRenderer
	{
		string RenderTemplate(ContentItem item, IItemContainer container);
	}

	public class TemplateRenderer : ITemplateRenderer
	{
		private readonly IControllerMapper _controllerMapper;
		private readonly IEngine _engine;

		public TemplateRenderer(IControllerMapper controllerMapper, IEngine engine)
		{
			_controllerMapper = controllerMapper;
			_engine = engine;
		}

		public string RenderTemplate(ContentItem item, IItemContainer container)
		{
			var routeData = new RouteData();

			routeData.Values[ContentRoute.ContentItemKey] = item;
			routeData.Values[ContentRoute.ContentEngineKey] = _engine;
			routeData.Values[ContentRoute.ControllerKey] = _controllerMapper.GetControllerName(item.GetType());
			routeData.Values[ContentRoute.ActionKey] = "index";

			var writer = new StringWriter();

			var viewDataContainer = container as IViewDataContainer ?? new DummyViewDataContainer(container);

			using (var scope = new HttpContextScope(writer))
			{
				// execute the action
				var helper = new System.Web.Mvc.HtmlHelper(new ViewContext
				                                           	{
				                                           		HttpContext = new HttpContextWrapper(scope.CurrentContext),
																ViewData = viewDataContainer.ViewData,
				                                           	},
				                                           viewDataContainer);

				helper.RenderRoute(routeData.Values);
			}

			return writer.ToString();
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