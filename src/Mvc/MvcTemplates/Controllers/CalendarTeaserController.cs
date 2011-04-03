using System;
using System.Linq;
using System.Web.Mvc;
using N2.Templates.Mvc.Models.Parts;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;
using N2.Templates.Mvc.Models.Pages;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(CalendarTeaser))]
	public class CalendarTeaserController : ContentController<CalendarTeaser>
	{
		public override ActionResult Index()
		{
			var container = CurrentItem.Container;
			var hits = container != null
				? container.GetEvents().Where(e => e.EventDate > DateTime.Today)
				: new Event[0];

			return PartialView(new CalendarTeaserModel(CurrentItem, hits.Take(5).ToList()));
		}
	}
}