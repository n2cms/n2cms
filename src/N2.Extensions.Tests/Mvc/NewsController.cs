using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using N2.Web.Mvc;

namespace N2.Extensions.Tests.Mvc
{
	[Controls(typeof(NewsItem))]
	public class NewsController : ContentController<NewsItem>
	{
		public override ActionResult Index()
		{
			throw new NotImplementedException();
		}
	}
}
