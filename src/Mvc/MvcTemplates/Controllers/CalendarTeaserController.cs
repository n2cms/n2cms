using System;
using System.Linq;
using System.Web.Mvc;
using N2.Templates.Mvc.Models.Parts;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;
using N2.Templates.Mvc.Models.Pages;
using N2.Linq;
using N2.Persistence;

namespace N2.Templates.Mvc.Controllers
{
    [Controls(typeof(CalendarTeaser))]
    public class CalendarTeaserController : ContentController<CalendarTeaser>
    {
        private IContentItemRepository repository;
        public CalendarTeaserController(IContentItemRepository repository)
        {
            this.repository = repository;
        }

        public override ActionResult Index()
        {
            var parameters = Parameter.TypeEqual(typeof(Event).Name)
                & Parameter.GreaterOrEqual("EventDate", N2.Utility.CurrentTime());

            if (CurrentItem.Container != null)
                parameters.Add(Parameter.BelowOrSelf(CurrentItem.Container));

            var hits = repository.Find(parameters.OrderBy("EventDate").Take(5))
                .OfType<Event>().ToList();

            return PartialView(new CalendarTeaserModel(CurrentItem, hits));
        }
    }
}
