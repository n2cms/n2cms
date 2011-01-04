using System;
using System.Linq;
using System.Web.Mvc;
using N2.Templates.Mvc.Models.Parts;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(CalendarTeaser))]
	public class CalendarTeaserController : ContentController<CalendarTeaser>
	{
		public override ActionResult Index()
		{
			var hits = CurrentItem.Container.GetEvents().Where(e => e.EventDate > DateTime.Today);

			return PartialView(new CalendarTeaserModel(CurrentItem, hits.Take(5).ToList()));
		}
	}
}