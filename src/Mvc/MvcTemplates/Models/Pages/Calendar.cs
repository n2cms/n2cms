using System;
using System.Collections.Generic;
using N2.Collections;
using N2.Integrity;
using N2.Web.Mvc;
using N2.Definitions;
using System.Linq;

namespace N2.Templates.Mvc.Models.Pages
{
    [PageDefinition("Calendar", 
        Description = "A list of recent events.",
        SortOrder = 120,
        IconClass = "fa fa-calendar")]
    [RestrictParents(typeof (IStructuralPage))]
    [SortChildren(SortBy.Expression, SortExpression = "EventDate")]
    public class Calendar : ContentPageBase
    {
        public virtual IEnumerable<Event> GetEvents()
        {
			return Children.WhereAccessible().OfType<Event>();
        }

        public virtual IList<Event> GetEvents(DateTime day)
        {
			return GetEvents().Where(e => e.EventDate.HasValue && e.EventDate.Value == day.Date).ToList();
        }
    }
}
