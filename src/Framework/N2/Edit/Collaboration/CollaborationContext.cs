using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace N2.Edit.Collaboration
{
	public class CollaborationContext
	{
		public ContentItem SelectedItem { get; set; }
		public DateTime LastDismissed { get; set; }

		public CollaborationContext ParseLastDismissed(string lastDismissed)
		{
			DateTime date;
			if (DateTime.TryParse(lastDismissed, CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out date))
				LastDismissed = date;
			return this;
		}
	}
}
