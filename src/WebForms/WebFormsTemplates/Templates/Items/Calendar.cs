using System;
using System.Collections.Generic;
using N2.Collections;
using N2.Integrity;
using N2.Web;
using N2.Definitions;
using System.Linq;

namespace N2.Templates.Items
{
    [PageDefinition("Calendar", 
        Description = "A list of recent events.",
        SortOrder = 120,
        IconUrl = "~/Templates/UI/Img/calendar.png")]
    [RestrictParents(typeof(IStructuralPage))]
    [ConventionTemplate("CalendarList")]
    [SortChildren(SortBy.Expression, SortExpression = "EventDate")]
    public class Calendar : AbstractContentPage
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
