using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using N2.Engine;

namespace N2.Web.Mvc
{
	public abstract class ContentController<T> : System.Web.Mvc.Controller
		where T: ContentItem
	{
		protected virtual IEngine Engine
		{
			get { return ControllerContext.RouteData.Values["ContentEngine"] as IEngine; }
		}
		protected virtual T CurrentItem
		{
			get { return ControllerContext.RouteData.Values["ContentItem"] as T; }
		}

		public abstract ActionResult Index();
	}
}
