using System.Collections.Generic;
using N2.Collections;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Calendar.Items
{
	[Definition("Calendar", "Calendar", "A list of recent events.", "", 120)]
	[RestrictParents(typeof(IStructuralPage))]
	public class Calendar : AbstractContentPage
	{
		public override string IconUrl
		{
			get { return "~/Calendar/UI/Img/calendar.png"; }
		}

		public override string TemplateUrl
		{
			get { return "~/Calendar/UI/Container.aspx"; }
		}

		public virtual IEnumerable<Event> GetEvents()
		{
			foreach (Event child in GetChildren(new TypeFilter(typeof(Event)), new AccessFilter()))
				yield return child;
		}
	}
}
