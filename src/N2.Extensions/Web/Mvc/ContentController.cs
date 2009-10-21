using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using N2.Engine;
using N2.Security;
using N2.Web.UI;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Base class for content controllers that provides easy access to the content item in scope.
	/// </summary>
	/// <typeparam name="T">The type of content item the controller handles.</typeparam>
	public abstract class ContentController<T> : Controller
		where T : ContentItem
	{
		private T _currentItem;
		private IEngine _engine;

		protected ContentController()
		{
			TempDataProvider = new SessionAndPerRequestTempDataProvider();
		}

		public virtual IEngine Engine
		{
			get
			{
				if (_engine == null)
				{
					_engine = ControllerContext.RouteData.Values[ContentRoute.ContentEngineKey] as IEngine
					          ?? Context.Current;
				}
				return _engine;
			}
			set { _engine = value; }
		}

		/// <summary>The content item associated with the requested path.</summary>
		public virtual T CurrentItem
		{
			get
			{
				if (_currentItem == null)
				{
					_currentItem = ControllerContext.RouteData.Values[ContentRoute.ContentItemKey] as T
					               ?? GetCurrentItemById();
				}
				return _currentItem;
			}
			set { _currentItem = value; }
		}

		public ContentItem CurrentPage
		{
			get
			{
				ContentItem page = CurrentItem;
				while (page != null && !page.IsPage)
					page = page.Parent;

				return page;
			}
		}

		private T GetCurrentItemById()
		{
			int itemId;
			if (Int32.TryParse(ControllerContext.RouteData.Values[ContentRoute.ContentItemIdKey] as string, out itemId)
				|| Int32.TryParse(Request[ContentRoute.ContentItemIdKey], out itemId))
				return Engine.Persister.Get(itemId) as T;

			return null;
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			CheckItemSecurity(filterContext);

			MergeModelStateFromEarlierStep(filterContext.HttpContext);

			base.OnActionExecuting(filterContext);
		}

		private void MergeModelStateFromEarlierStep(HttpContextBase context)
		{
			const string modelStateForMergingKey = "N2_ModelStateForMerging";

			ModelStateDictionary modelState = ModelState;
			if(context.Items.Contains(modelStateForMergingKey))
			{
				modelState = context.Items[modelStateForMergingKey] as ModelStateDictionary;

				if(modelState != null)
					ModelState.Merge(modelState);
			}

			context.Items[modelStateForMergingKey] = modelState;
		}

		private void CheckItemSecurity(ActionExecutingContext filterContext)
		{
			if (CurrentItem != null)
			{
				var securityManager = Engine.Resolve<ISecurityManager>();

				if (!securityManager.IsAuthorized(CurrentItem, User))
					filterContext.Result = new HttpUnauthorizedResult();
			}
		}

		/// <summary>Defaults to the current item's TemplateUrl and pass the item itself as view data.</summary>
		/// <returns>A reference to the item's template.</returns>
		public virtual ActionResult Index()
		{
			return View(CurrentItem);
		}

		protected virtual ActionResult RedirectToParentPage()
		{
			return Redirect(CurrentPage.Url);
		}

		/// <summary>
		/// Returns a <see cref="ViewPageResult"/> which calls the default action for the controller that handles the current page.
		/// </summary>
		/// <returns></returns>
		protected internal virtual ViewPageResult ViewParentPage()
		{
			if (CurrentItem != null && CurrentItem.IsPage)
			{
				throw new InvalidOperationException(
					"The current page is already being rendered. ViewParentPage should only be used from content items to render their parent page.");
			}

			return ViewPage(CurrentPage);
		}

		/// <summary>
		/// Returns a <see cref="ViewPageResult"/> which calls the default action for the controller that handles the current page.
		/// </summary>
		/// <returns></returns>
		protected internal virtual ViewPageResult ViewPage(ContentItem thePage)
		{
			if (thePage == null)
				throw new ArgumentNullException("thePage");

			if (!thePage.IsPage)
				throw new InvalidOperationException("Item " + thePage.GetType().Name + " is not a page type and cannot be rendered on its own.");

			if(thePage == CurrentItem)
				throw new InvalidOperationException("The page passed into ViewPage was the current page. This would cause an infinite loop.");

			return new ViewPageResult(thePage, Engine.Resolve<IControllerMapper>(), Engine.Resolve<IWebContext>(),
									  ActionInvoker);
		}

		/// <summary>
		/// Overrides/hides the most common method of rendering a View and calls View or PartialView depending on the type of the model
		/// </summary>
		/// <param name="viewName"></param>
		/// <returns></returns>
		protected new ViewResultBase View(string viewName)
		{
			return View(viewName, CurrentItem);
		}

		/// <summary>
		/// Overrides/hides the most common method of rendering a View and calls View or PartialView depending on the type of the model
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		protected new ViewResultBase View(object model)
		{
			ContentItem item = ItemExtractor.ExtractFromModel(model) ?? CurrentItem;

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
			ContentItem item = ItemExtractor.ExtractFromModel(model) ?? CurrentItem;

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

		private void CheckForPageRender(object model)
		{
			ContentItem item = ItemExtractor.ExtractFromModel(model) ?? CurrentItem;

			if (item != null && !item.IsPage)
				throw new InvalidOperationException(@"Rendering of Parts using View(..) is no longer supported. Use PartialView(..) to render this item.
			
- Item of type " + item.GetType() + @"
- Controller is " + GetType() + @"
- Action is " + ViewData[ContentRoute.ActionKey]);
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