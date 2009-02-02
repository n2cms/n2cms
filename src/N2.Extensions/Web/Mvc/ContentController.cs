using System.Web.Mvc;
using N2.Engine;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Base class for contet controllers that provides easy access to the content item in scope.
	/// </summary>
	/// <typeparam name="T">The type of content item the controller handles.</typeparam>
	public abstract class ContentController<T> : Controller
		where T: ContentItem
	{
		protected virtual IEngine Engine
		{
			get { return ControllerContext.RouteData.Values[ContentRoute.ContentEngineKey] as IEngine; }
		}

		/// <summary>The content item associated with the requested path.</summary>
		protected virtual T CurrentItem
		{
			get
			{
				T item = ControllerContext.RouteData.Values[ContentRoute.ContentItemKey] as T;
				return item;
			}
		}

		/// <summary>Defaults to the current item's TemplateUrl and pass the item itself as view data.</summary>
		/// <returns>A reference to the item's template.</returns>
		public virtual ActionResult Index()
		{
			return View(CurrentItem.TemplateUrl, CurrentItem);
		}
	}
}
