using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace N2.Web.Mvc
{
	[Obsolete("The fallback is removed to avoid interfering with non-mvc content items.")]
	public class FallbackContentController : ContentController<ContentItem>
	{
		public override ActionResult Index()
		{
			ContentItem item = CurrentItem;
			
			return View(item.TemplateUrl, item);
		}
	}
}
