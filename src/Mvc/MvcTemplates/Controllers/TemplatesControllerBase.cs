using System;
using N2.Web.Mvc;
using N2.Web.UI;
using System.Web.Mvc;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Routing;

namespace N2.Templates.Mvc.Controllers
{
    /// <summary>
    /// Base class with default handling of index action and other useful helpers.
    /// </summary>
    /// <typeparam name="T">The type of content this controller handles.</typeparam>
    public abstract class TemplatesControllerBase<T> : ContentController<T> where T : ContentItem
    {
        public TemplatesControllerBase()
        {
            TempDataProvider = new SessionAndPerRequestTempDataProvider();
        }

        //// Actions ////

        /// <summary>By default the templates index method renders a view located in ~/Views/SharedPages/{ContentType}.aspx or partial view located in ¨~/Views/SharedPages/{ContentType}.ascx.</summary>
        /// <returns></returns>
        public override System.Web.Mvc.ActionResult Index()
        {
            if(CurrentItem.IsPage)
                return View(string.Format("PageTemplates/{0}", CurrentItem.GetContentType().Name), CurrentItem);
            else
                return PartialView(string.Format("PartTemplates/{0}", CurrentItem.GetContentType().Name), CurrentItem);
        }

        //// View overloads ////

        /// <summary>
        /// Overrides/hides the most common method of rendering a View and calls View or PartialView depending on the type of the model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected new ViewResultBase View(object model)
        {
            ContentItem item = ExtractFromModel(model) ?? CurrentItem;

            if (item != null && !item.IsPage)
                return PartialView(model);

            return base.View(model);
        }

        /// <summary>
        /// Overrides/hides the most common method of rendering a View and calls View or PartialView depending on the type of the model
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected new ViewResultBase View(string viewName, object model)
        {
            ContentItem item = ExtractFromModel(model) ?? CurrentItem;

            if (item != null && !item.IsPage)
                return PartialView(viewName, model);

            return base.View(viewName, model);
        }

        protected override ViewResult View(IView view, object model)
        {
            CheckForPageRender(model);

            return base.View(view, model ?? CurrentItem);
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            CheckForPageRender(model);

            return base.View(viewName, masterName, model ?? CurrentItem);
        }

        /// <summary>Renders a script that loads the given action asynchronously from the client.</summary>
        /// <param name="actionName">The name of the action to call.</param>
        /// <returns>A content result with a load action script.</returns>
        protected virtual ActionResult AsyncView(string actionName)
        {
            return AsyncView(actionName, null);
        }

        /// <summary>Renders a script that loads the given action asynchronously from the client.</summary>
        /// <param name="actionName">The name of the action to call.</param>
        /// <param name="routeParams">Additional parameters for the async load request.</param>
        /// <returns>A content result with a load action script.</returns>
        protected virtual ActionResult AsyncView(string actionName, object routeParams)
        {
            var values = new RouteValueDictionary(routeParams);
            values[ContentRoute.ActionKey] = actionName;

            return AsyncView(values);
        }

        /// <summary>Renders a script that loads the given action asynchronously from the client.</summary>
        /// <param name="routeParams">The route parameters to use for building the url.</param>
        /// <returns>A content result with a load action script.</returns>
        protected virtual ActionResult AsyncView(RouteValueDictionary routeParams)
        {
            if (!routeParams.ContainsKey(ContentRoute.ContentPartKey))
                routeParams[ContentRoute.ContentPartKey] = CurrentItem.ID;
            
            string id = "part_" + routeParams[ContentRoute.ContentPartKey];
            N2.Web.Url url = CurrentPage.Url;
            url = url.UpdateQuery(routeParams);

            return Content(string.Format(@"<div id='{0}' class='async loading'></div><script type='text/javascript'>//<![CDATA[
jQuery('#{0}').load('{1}');//]]></script>", id, url));
        }

        //// Helpers ////

        private void CheckForPageRender(object model)
        {
            ContentItem item = ExtractFromModel(model) ?? CurrentItem;

            if (item != null && !item.IsPage)
                throw new InvalidOperationException(@"Rendering of Parts using View(..) is no longer supported. Use PartialView(..) to render this item.
            
- Item of type " + item.GetContentType() + @"
- Controller is " + GetType() + @"
- Action is " + ViewData[ContentRoute.ActionKey]);
        }

        private static ContentItem ExtractFromModel(object model)
        {
            var item = model as ContentItem;

            if (item != null)
                return item;

            var itemContainer = model as IItemContainer;

            return itemContainer != null ? itemContainer.CurrentItem : null;
        }

        #region Nested type: SessionAndPerRequestTempDataProvider

        /// <summary>
        /// Overrides the default behaviour in the SessionStateTempDataProvider to make TempData available for the entire request, not just for the first controller it sees
        /// </summary>
        private sealed class SessionAndPerRequestTempDataProvider : ITempDataProvider
        {
            private const string TempDataSessionStateKey = "__ControllerTempData";

            #region ITempDataProvider Members

            public IDictionary<string, object> LoadTempData(ControllerContext controllerContext)
            {
                HttpContextBase httpContext = controllerContext.HttpContext;

                if (httpContext.Session == null)
                {
                    throw new InvalidOperationException("Session state appears to be disabled.");
                }

                var tempDataDictionary = (httpContext.Session[TempDataSessionStateKey]
                                          ?? httpContext.Items[TempDataSessionStateKey])
                                         as Dictionary<string, object>;

                if (tempDataDictionary == null)
                {
                    return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                }

                // If we got it from Session, remove it so that no other request gets it
                httpContext.Session.Remove(TempDataSessionStateKey);
                // Cache the data in the HttpContext Items to keep available for the rest of the request
                httpContext.Items[TempDataSessionStateKey] = tempDataDictionary;

                return tempDataDictionary;
            }

            public void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values)
            {
                HttpContextBase httpContext = controllerContext.HttpContext;

                if (httpContext.Session == null)
                {
                    throw new InvalidOperationException("Session state appears to be disabled.");
                }

                httpContext.Session[TempDataSessionStateKey] = values;
            }

            #endregion
        }

        #endregion
    }
}
