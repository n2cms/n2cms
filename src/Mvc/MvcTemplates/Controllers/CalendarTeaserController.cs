using System;
using System.Linq;
using System.Web.Mvc;
using N2.Templates.Mvc.Models.Parts;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;
using N2.Templates.Mvc.Models.Pages;
using N2.Linq;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(CalendarTeaser))]
	public class CalendarTeaserController : ContentController<CalendarTeaser>
	{
		public override ActionResult Index()
		{
            var query = Content.Search.Find.Where.Type.Eq(typeof(Event))
                .And.Property("EventDate").Ge(DateTime.Now);

            if (CurrentItem.Container != null)
                query = query.And.IsDescendantOrSelf(CurrentItem.Container);

            var hits = query.OrderBy.Property("EventDate").Asc
                .MaxResults(5).Select<Event>();

			return PartialView(new CalendarTeaserModel(CurrentItem, hits));
		}
	}
}