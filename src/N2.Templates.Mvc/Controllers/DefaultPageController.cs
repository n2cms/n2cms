using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Web;
using N2.Templates.Mvc.Items;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(AbstractPage))]
	public class DefaultPageController : TemplatesControllerBase<AbstractPage>
	{
	}
}
