using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using N2.Engine;
using N2.Security;
using N2.Engine.Globalization;
using System.Threading;
using System.Globalization;

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

		/// <summary>
		/// Used to resolve services.
		/// </summary>
		public virtual IEngine Engine
		{
			get
			{
				return engine
					?? (engine = RouteData.GetEngine());
			}
			set { engine = value; }
		}

		/// <summary>The content item associated with the requested path.</summary>
		public virtual T CurrentItem
		{
			get
			{
				return currentItem
					?? (currentItem = ControllerContext.RequestContext.CurrentItem<T>());
			}
			set { currentItem = value; }
		}

		/// <summary>The page beeing served by this request, or the page a part has been added to.</summary>
		public ContentItem CurrentPage
		{
			get
			{
				return currentPage
				  ?? (currentPage = RouteData.CurrentPage()
					  ?? CurrentItem.ClosestPage()
					  ?? Engine.UrlParser.CurrentPage);
			}
			set { currentPage = value; }
		}

		/// <summary>The currently displayed part or null if a page is beeing executed.</summary>
		public ContentItem CurrentPart
		{
			get
			{
				return currentPart
					?? (currentPart = ControllerContext.RequestContext.CurrentPart<ContentItem>());
			}
			set { currentPart = value; }
		}
		#endregion



		/// <summary>Defaults to the current item's TemplateUrl and pass the item itself as view data.</summary>
		/// <returns>A reference to the item's template.</returns>
		public virtual ActionResult Index()
		{
			return View(CurrentItem);
		}




		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			SetCulture(filterContext);
			ValidateAuthorization(filterContext);

			MergeModelStateFromEarlierStep(filterContext.HttpContext);

			base.OnActionExecuting(filterContext);
		}

		protected virtual void SetCulture(ActionExecutingContext filterContext)
		{
			if (ControllerContext.IsChildAction || CurrentItem == null)
				return;

			var gateway = Engine.Resolve<ILanguageGateway>();
			if (!gateway.Enabled)
				return;

			ILanguage language = gateway.GetLanguage(CurrentItem);
			if (language == null || string.IsNullOrEmpty(language.LanguageCode))
				return;

			var culture = CultureInfo.GetCultureInfoByIetfLanguageTag(language.LanguageCode);
			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;
		}

		/// <summary>Validates that the user is allowed to read the rendered item.</summary>
		/// <param name="filterContext"></param>
		protected virtual void ValidateAuthorization(ActionExecutingContext filterContext)
		{
			if (CurrentItem != null)
			{
				var securityManager = Engine.Resolve<ISecurityManager>();

				if (!securityManager.IsAuthorized(CurrentItem, User))
					filterContext.Result = new HttpUnauthorizedResult();
			}
		}

		protected override void OnResultExecuting(ResultExecutingContext filterContext)
		{
			ProcessOutputCache(filterContext);

			base.OnResultExecuting(filterContext);
		}

		/// <summary>Applies output cache as configured in the n2/host/outputCache config section.</summary>
		/// <param name="filterContext"></param>
		protected virtual void ProcessOutputCache(ResultExecutingContext filterContext)
		{
			new ContentOutputCacheAttribute().OnResultExecuting(filterContext);
		}

		private void MergeModelStateFromEarlierStep(HttpContextBase context)
		{
			const string modelStateForMergingKey = "N2_ModelStateForMerging";

			ModelStateDictionary modelState = ModelState;
			if (context.Items.Contains(modelStateForMergingKey))
			{
				modelState = context.Items[modelStateForMergingKey] as ModelStateDictionary;

				if (modelState != null)
					ModelState.Merge(modelState);
			}

			context.Items[modelStateForMergingKey] = modelState;
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
				throw new InvalidOperationException("Item " + thePage.GetContentType().Name + " is not a page type and cannot be rendered on its own.");

			if (thePage == CurrentItem)
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
