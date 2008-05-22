using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Security
{
	/// <summary>
	/// This class is a base class for attributes used to restrict permissions.
	/// </summary>
	public abstract class AuthorizedRolesAttribute : Attribute
	{
		public AuthorizedRolesAttribute()
		{
		}

		public AuthorizedRolesAttribute(params string[] authorizedRoles)
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
