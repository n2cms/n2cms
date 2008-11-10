using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.Mvc;

namespace N2.Extensions.Tests.Mvc
{
	[Controls(typeof(BaseItem))]
	public class BaseController : ContentController<BaseItem>
	{
		public override System.Web.Mvc.ActionResult Index()
		{
			throw new NotImplementedException();
		}
	}
}
