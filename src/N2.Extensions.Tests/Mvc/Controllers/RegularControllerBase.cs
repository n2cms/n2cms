using System;
using N2.Extensions.Tests.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Extensions.Tests.Mvc.Controllers
{
	[Controls(typeof(RegularPage))]
	public class RegularControllerBase : ContentController<RegularPage>
	{
		public override System.Web.Mvc.ActionResult Index()
		{
			throw new NotImplementedException();
		}
	}
}