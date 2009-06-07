using N2.Templates.Mvc.Items.Pages;
using N2.Templates.Mvc.Services;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(RssFeed))]
	public class RssFeedController : ContentController<RssFeed>
	{
		public override System.Web.Mvc.ActionResult Index()
		{
			if (Request["hungry"] == "yes")
			{
				Response.ContentType = "text/xml";
				Engine.Resolve<RssWriter>().Write(Response.Output, CurrentItem);
				Response.End();
			}
			return base.Index();
		}
	}
}