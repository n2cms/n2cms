using System.Web.Mvc;
using System.Web.Routing;

namespace N2.Web.Mvc
{
    /// <summary>
    /// A custom <see cref="ActionResult"/> that invokes the MVC pipeline again for the given page.
    /// </summary>
    public class ViewPageResult : ActionResult
    {
        private readonly ContentItem thePage;
        private readonly IControllerMapper controllerMapper;
        private readonly IWebContext webContext;
        private readonly IActionInvoker actionInvoker;
        private readonly IControllerFactory controllerFactory;

        public ViewPageResult(ContentItem thePage, IControllerMapper controllerMapper, IWebContext webContext, IActionInvoker actionInvoker)
        {
            this.controllerFactory = ControllerBuilder.Current.GetControllerFactory();
            this.thePage = thePage;
            this.controllerMapper = controllerMapper;
            this.webContext = webContext;
            this.actionInvoker = actionInvoker;
        }

        public ContentItem Page
        {
            get { return thePage; }
        }

        /// <summary>
        /// Enables processing of the result of an action method by a custom type that inherits from <see cref="T:System.Web.Mvc.ActionResult"/>.
        /// </summary>
        /// <param name="context"/>
        public override void ExecuteResult(ControllerContext context)
        {
            try
            {
                context = BuildPageControllerContext(context);
                actionInvoker.InvokeAction(context, "Index");
            }
            finally
            {
                if (context.Controller != null)
                    controllerFactory.ReleaseController(context.Controller);
            }
        }

        private ControllerContext BuildPageControllerContext(ControllerContext context)
        {
            string controllerName = controllerMapper.GetControllerName(thePage.GetContentType());
            RouteExtensions.ApplyCurrentPath(context.RouteData, controllerName, "Index", new PathData(thePage));

            var requestContext = new RequestContext(context.HttpContext, context.RouteData);

            var controller = (ControllerBase)controllerFactory.CreateController(requestContext, controllerName);

            controller.ControllerContext = new ControllerContext(requestContext, controller);
            controller.ViewData.ModelState.Merge(context.Controller.ViewData.ModelState);

            return controller.ControllerContext;
        }
    }
}
