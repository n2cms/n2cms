using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Collections;

namespace N2.Security
{
	public class ContentRoleProvider : RoleProvider
	{
		protected ItemBridge Bridge
		{
			get { return Context.Current.Resolve<ItemBridge>(); }
		}

		public override void AddUsersToRoles(string[] usernames, string[] roleNames)
		{
			foreach (string username in usernames)
			{
				Items.User u = Bridge.GetUser(username);
				foreach (string role in roleNames)
				{
					if (!u.Roles.Contains(role))
					{
						u.Roles.Add(role);
						Bridge.Save(u);
					}
				}
			}
		}

		private string applicationName = "N2.Templates.Roles";

		public override string ApplicationName
		{
			get { return applicationName; }
			set { applicationName = value; }
		}

		public override void CreateRole(string roleName)
		{
			Items.UserList ul = Bridge.GetUserContainer(true);
			ul.AddRole(roleName);
			Bridge.Save(ul);
		}

		public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
		{
			if(throwOnPopulatedRole && GetUsersInRole(roleName).Length > 0)
				throw new N2Exception("Role {0} cannot be deleted since it has users attached to it.", roleName);
			
			Items.UserList ul = Bridge.GetUserContainer(true);
			ul.RemoveRole(roleName);
			Bridge.Save(ul);
			return true;
		}

		public override string[] FindUsersInRole(string roleName, string usernameToMatch)
		{
			IList<ContentItem> users = Bridge.Finder
				.Where.Name.Eq(usernameToMatch)
				.And.Detail("Roles").Eq(roleName)
				.Select();
			return ToArray(users);
		}

		public override string[] GetAllRoles()
		{
			Items.UserList users = Bridge.GetUserContainer(false);
			if (users == null)
				return Bridge.DefaultRoles;
			return users.GetRoleNames();
		}

		public override string[] GetRolesForUser(string username)
		{
			Items.User u = Bridge.GetUser(username);
			if (u != null)
				return u.GetRoles();
			return new string[0];
		}

		public override string[] GetUsersInRole(string roleName)
		{
			IList<ContentItem> users = Bridge.Finder
				.Where.Detail("Roles").Like(roleName)
				.Select();
			return ToArray(users);
		}

		public override bool IsUserInRole(string username, string roleName)
		{
			Items.User u = Bridge.GetUser(username);
			foreach(string userRole in u.Roles)
			{
				if (userRole.Equals(roleName))
				{
					return true;
				}
			}
			return false;
		}

		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
		{
			foreach (string username in usernames)
			{
				Items.User u = Bridge.GetUser(username);
				foreach (string role in roleNames)
				{
					if (u.Roles.Contains(role))
					{
						u.Roles.Remove(role);
						Bridge.Save(u);
					}
				}
			}
		}

		public override bool RoleExists(string roleName)
		{
			return 0 < Array.IndexOf<string>(GetAllRoles(), roleName);
		}

		private static string[] ToArray(IList<ContentItem> items)
		{
			string[] roles = new string[items.Count];
			for (int i = 0; i < roles.Length; i++)
			{
				roles[i] = items[i].Name;
			}
			return roles;
		}

		private static string[] ToArray(IList items)
		{
			string[] roles = new string[items.Count];
			for (int i = 0; i < roles.Length; i++)
			{
				roles[i] = ((ContentItem)items[i]).Name;
			}
			return roles;
		}
	}
}
