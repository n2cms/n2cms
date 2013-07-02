using N2.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Targeting.Detectors
{
	[Detector]
	public class MembershipDetector : DetectorBase
	{
		private ISecurityManager security;
		public MembershipDetector(ISecurityManager security)
		{
			this.security = security;
		}

		public override void AppendFlags(TargetingContext context)
		{
			if (!context.HttpContext.User.Identity.IsAuthenticated)
				context.Flags.Add(Security.AuthorizedRole.AnonymousUser.Identity.Name);
			else 
			{
				context.Flags.Add("Authenticated");
				if (security.IsAdmin(context.HttpContext.User))
					context.Flags.Add("Administrator");
				else if (security.IsEditor(context.HttpContext.User))
					context.Flags.Add("Editor");
			}
		}
	}
}
