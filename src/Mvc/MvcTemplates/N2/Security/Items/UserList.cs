using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI.WebControls;
using N2.Collections;
using N2.Definitions;
using N2.Details;
using System.Security.Principal;

namespace N2.Security.Items
{
    [PartDefinition("User List", 
		SortOrder = 2000, 
		IconUrl= "{ManagementUrl}/Resources/icons/group.png")]
	[WithEditableTitle("Title", 10)]
	[ItemAuthorizedRoles(Roles = new string[0])]
	[Throwable(AllowInTrash.No)]
	public class UserList : ContentItem, ISystemNode
	{
		[EditableText("Roles", 100, TextMode=TextBoxMode.MultiLine)]
		[DetailAuthorizedRoles("admin", "Administrators")]
		public virtual string Roles
		{
			get { return (string) (GetDetail("Roles") ?? "Everyone"); }
			set { SetDetail("Roles", value, "Everyone"); }
		}

		public virtual string[] GetRoleNames()
		{
			return Roles.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		}

		/// <summary>Adds a role if not already present.</summary>
		/// <param name="roleName">The role to add.</param>
		public virtual void AddRole(string roleName)
		{
			if(Array.IndexOf<string>(GetRoleNames(), roleName) < 0)
				Roles += Environment.NewLine + roleName;
		}

		/// <summary>Removes a role if existing.</summary>
		/// <param name="roleName">The role to remove.</param>
		public virtual void RemoveRole(string roleName)
		{
			List<string> roles = new List<string>();
			foreach (string existingRole in GetRoleNames())
			{
				if (!existingRole.Equals(roleName))
				{
					roles.Add(existingRole);
				}
			}
			Roles = string.Join(Environment.NewLine, roles.ToArray());
		}

		public virtual MembershipUserCollection GetMembershipUsers(string providerName)
		{
			MembershipUserCollection muc = new MembershipUserCollection();
			foreach (User u in Children)
			{
				muc.Add(u.GetMembershipUser(providerName));
			}
			return muc;
		}

		public virtual MembershipUserCollection GetMembershipUsers(string providerName, int startIndex, int maxResults,
		                                                           out int totalRecords)
		{
			totalRecords = 0;
			CountFilter cf = new CountFilter(startIndex, maxResults);
			MembershipUserCollection muc = new MembershipUserCollection();
			foreach (User u in Children)
			{
				if (cf.Match(u))
					muc.Add(u.GetMembershipUser(providerName));
				totalRecords++;
			}
			return muc;
		}

		public override bool IsAuthorized(IPrincipal user)
		{
			return base.IsAuthorized(user) && Context.SecurityManager.IsAdmin(user);
		}
	}
}