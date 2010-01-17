using System;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
	public abstract class TemplatesControllerBase<T> : ContentController<T> where T : ContentItem
	{
		public override System.Web.Mvc.ActionResult Index()
		{
			if(CurrentItem.IsPage)
				return View(string.Format("~/Views/SharedPages/{0}.aspx", CurrentItem.GetType().Name), CurrentItem);
			else
				return PartialView(string.Format("~/Views/SharedParts/{0}.ascx", CurrentItem.GetType().Name), CurrentItem);
		}
	}
}