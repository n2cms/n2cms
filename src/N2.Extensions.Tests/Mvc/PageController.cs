using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.Mvc;

namespace N2.Extensions.Tests.Mvc
{
	[Controls(typeof(PageItem))]
	public class PageController : ContentController<PageItem>
	{
		public override System.Web.Mvc.ActionResult Index()
		{
			throw new NotImplementedException();
		}
	}
}
