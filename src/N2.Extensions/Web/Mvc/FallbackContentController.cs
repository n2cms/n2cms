using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace N2.Web.Mvc
{
	[Controls(typeof(ContentItem))]
	public class FallbackContentController : ContentController<ContentItem>
	{
		public override ActionResult Index()
		{
			ContentItem item = CurrentItem;
			
			return View(item.TemplateUrl, item);
		}
	}
}
