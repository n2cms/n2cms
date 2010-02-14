using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using N2.Engine;
using N2.Security;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Base class for content controllers that provides easy access to the content item in scope.
	/// </summary>
	/// <typeparam name="T">The type of content item the controller handles.</typeparam>
	public abstract class ContentController<T> : Controller
		where T : ContentItem
	{
		#region Accessors
		ContentItem currentPart;
		T currentItem;
		ContentItem currentPage;
		IEngine engine;
		

		public virtual IEngine Engine
		{
			get
			{
				if (engine == null)
				{
					engine = ControllerContext.RouteData.DataTokens[ContentRoute.ContentEngineKey] as IEngine
					          ?? Context.Current;
				}
				return engine;
			}
			set { engine = value; }
		}

		/// <summary>The content item associated with the requested path.</summary>
		public virtual T CurrentItem
		{
			get
			{
				if (currentItem == null)
					currentItem = ControllerContext.RequestContext.CurrentItem<T>()
						?? GetCurrentItemById(ContentRoute.ContentItemKey);

				return currentItem;
			}
			set { currentItem = value; }
		}

		public ContentItem CurrentPage
		{
			get
			{
				if(currentPage == null)
					currentPage = ControllerContext.RequestContext.CurrentPage<ContentItem>()
						?? CurrentItem.ClosestPage()
						?? Engine.UrlParser.CurrentPage;

				return currentPage;
			}
		}

		public ContentItem CurrentPart
		{
			get { return currentPart ?? (currentPart = ControllerContext.RequestContext.CurrentPart<ContentItem>()); }
		}
		#endregion



		/// <summary>Defaults to the current item's TemplateUrl and pass the item itself as view data.</summary>
		/// <returns>A reference to the item's template.</returns>
		public virtual ActionResult Index()
		{
			return View(CurrentItem);
		}



		private T GetCurrentItemById(string key)
		{
			int itemId;
			if (Int32.TryParse(ControllerContext.RouteData.Values[key] as string, out itemId)
				|| Int32.TryParse(Request[ContentRoute.ContentItemKey], out itemId))
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

			var mapper = Engine.Resolve<IControllerMapper>();
			var webContext = Engine.Resolve<IWebContext>();

			return new ViewPageResult(thePage, mapper, webContext, ActionInvoker);
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
	}
}