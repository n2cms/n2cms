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

			ControllerBase controller = BuildController(context);

			_actionInvoker.InvokeAction(controller.ControllerContext, "Index");
		}

		private void SetupN2ForNewPageRequest()
		{
			_webContext.CurrentPage = _thePage;
		}

		private ControllerBase BuildController(ControllerContext context)
		{
			var routeData = context.RouteData;
			routeData.Values[ContentRoute.ContentItemKey] = _thePage;
			routeData.Values[ContentRoute.ContentItemIdKey] = _thePage.ID;
			routeData.Values["action"] = "Index";

			var requestContext = new RequestContext(context.HttpContext, routeData);

			var controller = (ControllerBase)ControllerBuilder.Current.GetControllerFactory()
			                                 	.CreateController(requestContext, _controllerMapper.GetControllerName(_thePage.GetType()));

			controller.ControllerContext = new ControllerContext(requestContext, controller);
			controller.ViewData.ModelState.Merge(context.Controller.ViewData.ModelState);

			return controller;
		}
	}
}