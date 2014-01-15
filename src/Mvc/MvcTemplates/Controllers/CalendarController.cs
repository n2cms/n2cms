using System;
using System.Linq;
using System.Web.Mvc;
using N2.Templates.Mvc.Models.Pages;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
    [Controls(typeof(Calendar))]
    public class CalendarController : ContentController<Calendar>
    {
        [NonAction]
        public override ActionResult Index()
        {
            return Index(null);
        }

        public ActionResult Index(DateTime? date)
        {
            var hits = CurrentItem.GetEvents().Where(e => e.EventDate > DateTime.Today);
            
            if (date != null)
                hits = CurrentItem.GetEvents(date.Value);

            return View(new CalendarModel(CurrentItem, hits.ToList()));
        }
    }
}
