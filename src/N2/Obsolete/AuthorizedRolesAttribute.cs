using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Security
{
	/// <summary>
	/// This class is a base class for attributes used to restrict permissions.
	/// </summary>
	[Obsolete("No longer in use.", true)]
	public abstract class AuthorizedRolesAttribute2 : Attribute
	{
		public AuthorizedRolesAttribute2()
		{
		}

		public AuthorizedRolesAttribute2(params string[] authorizedRoles)
		{
			this.roles = authorizedRoles;
		}

		private string[] roles;

		public string[] Roles
		{
			get { return roles; }
			set { roles = value; }
		}
	}
}
