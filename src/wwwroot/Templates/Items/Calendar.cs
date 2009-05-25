using System;
using System.Collections.Generic;
using N2.Collections;
using N2.Integrity;
using N2.Web;

namespace N2.Templates.Items
{
    [Definition("Calendar", 
		Description = "A list of recent events.",
		SortOrder = 120,
		IconUrl = "~/Templates/UI/Img/calendar.png")]
    [RestrictParents(typeof(IStructuralPage))]
	[ConventionTemplate("CalendarList")]
    public class Calendar : AbstractContentPage
    {
        public virtual IEnumerable<Event> GetEvents()
        {
            foreach (Event child in GetChildren(new TypeFilter(typeof(Event)), new AccessFilter()))
                yield return child;
		}

		public virtual IList<Event> GetEvents(DateTime day)
		{
			return GetChildren(new TypeFilter(typeof (Event)), new AccessFilter(), new DelegateFilter(c => ((Event) c).EventDate.HasValue && ((Event) c).EventDate.Value.Date == day.Date))
				.Cast<Event>();
		}
    }
}