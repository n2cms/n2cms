using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Security;
using N2.Collections;

namespace N2.Tests.Fakes
{
	public class FakeSecurityManager : ISecurityManager
	{
		#region ISecurityManager Members

		public bool IsEditor(System.Security.Principal.IPrincipal principal)
		{
			return principal != null && principal.Identity.Name == "Editor" || principal.IsInRole("Editors");
		}

		public bool IsAdmin(System.Security.Principal.IPrincipal principal)
		{
			return principal != null && principal.Identity.Name == "Admin" || principal.IsInRole("Administrators");
		}

		public bool IsAuthorized(ContentItem item, System.Security.Principal.IPrincipal principal)
		{
			if (principal == null)
				return item["Unaccessible"] == null;
			return item.Name == principal.Identity.Name;
		}

		public bool IsPublished(ContentItem item)
		{
			return PublishedFilter.IsPublished(item);
		}

		public bool Enabled { get; set; }

		public bool ScopeEnabled { get; set; }

		public bool IsAuthorized(System.Security.Principal.IPrincipal user, IEnumerable<string> roles)
		{
			foreach(string role in roles)
				if(user.IsInRole(role))
					return true;
			return false;
		}

		#endregion
	}
}
