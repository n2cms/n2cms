using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace N2.Web.Mvc
{
	/// <summary>
	/// A custom <see cref="ActionResult"/> that invokes the MVC pipeline again for the given page.
	/// </summary>
	public class ViewPageResult : ActionResult
	{
		private readonly ContentItem _thePage;
		private readonly IControllerMapper _controllerMapper;
		private readonly IWebContext _webContext;
		private readonly IActionInvoker _actionInvoker;

		public ViewPageResult(ContentItem thePage, IControllerMapper controllerMapper, IWebContext webContext, IActionInvoker actionInvoker)
		{
			_thePage = thePage;
			_controllerMapper = controllerMapper;
			_webContext = webContext;
			_actionInvoker = actionInvoker;
		}

		public ContentItem Page
		{
			get { return _thePage; }
		}

		/// <summary>
		/// Enables processing of the result of an action method by a custom type that inherits from <see cref="T:System.Web.Mvc.ActionResult"/>.
		/// </summary>
		/// <param name="context"/>
		public override void ExecuteResult(ControllerContext context)
		{
			SetupN2ForNewPageRequest();

			context = BuildPageControllerContext(context);

			_actionInvoker.InvokeAction(context, "Index");
		}

		private void SetupN2ForNewPageRequest()
		{
			_webContext.CurrentPage = _thePage;
		}

		private ControllerContext BuildPageControllerContext(ControllerContext context)
		{
			string controllerName = _controllerMapper.GetControllerName(_thePage.GetContentType());
			
			var routeData = context.RouteData;
			routeData.ApplyCurrentItem(controllerName, "Index", _thePage, null);
			if (context.RouteData.DataTokens.ContainsKey(ContentRoute.ContentPartKey))
			{
				routeData.ApplyContentItem(ContentRoute.ContentPartKey, context.RouteData.DataTokens[ContentRoute.ContentPartKey] as ContentItem);
			}

			var requestContext = new RequestContext(context.HttpContext, routeData);

			var controller = (ControllerBase)ControllerBuilder.Current.GetControllerFactory()
			                                 	.CreateController(requestContext, controllerName);

			controller.ControllerContext = new ControllerContext(requestContext, controller);
			controller.ViewData.ModelState.Merge(context.Controller.ViewData.ModelState);

			return controller.ControllerContext;
		}
	}
}