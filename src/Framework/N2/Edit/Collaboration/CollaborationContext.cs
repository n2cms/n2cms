using N2.Management.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace N2.Edit.Collaboration
{
	public class CollaborationContext
	{
		public ContentItem SelectedItem { get; set; }
		public DateTime LastDismissed { get; set; }
		public IPrincipal User { get; set; }

		public CollaborationContext ParseLastDismissed(string lastDismissed)
		{
			LastDismissed = Parse(lastDismissed);
			return this;
		}

		private static DateTime Parse(string lastDismissed)
		{
			DateTime date;
			if (DateTime.TryParse(lastDismissed, CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out date))
				return date;
			return DateTime.MinValue;
		}

		public static CollaborationContext Create(IProfileRepository profiles, ContentItem item, System.Web.HttpContextBase context)
		{
			return Create(Parse(context.Request["lastDismissed"] ?? profiles.GetProfileSetting(context.User, "LastDismissed") as string), item, context.User);
		}

		public static CollaborationContext Create(DateTime lastDismissed, ContentItem item, IPrincipal user)
		{
			return new CollaborationContext { SelectedItem = item, User = user, LastDismissed = lastDismissed };
		}
	}
}
