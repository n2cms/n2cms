using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Security;
using System.Linq;
using N2.Persistence;
using System.Configuration;
using N2.Configuration;

namespace N2.Security
{
    public class ContentRoleProvider : RoleProvider
    {
        private string applicationName = "N2.Templates.Roles";
        private ItemBridge bridge;
        private N2.Engine.StructureBoundDictionaryCache<string, CachedRoles> cache;
        
        public ContentRoleProvider()
        {
        }

        public ContentRoleProvider(ItemBridge bridge, Engine.StructureBoundDictionaryCache<string, CachedRoles> cache)
            : this()
        {
            Set(bridge);
            this.cache = cache;
        }

        protected virtual Engine.IEngine Engine
        {
            get { return Context.Current; }
        }

        protected virtual ItemBridge Bridge
        {
            get { return bridge ?? Set(Engine.Resolve<ItemBridge>()); }
        }

        private ItemBridge Set(ItemBridge bridge)
        {
            this.bridge = bridge;
            bridge.UserSaved += (s, ea) => Cache.Expire();
            return bridge;
        }

        public class CachedRoles
        {
            public string[] Roles { get; set; }
        }

        protected Engine.StructureBoundDictionaryCache<string, CachedRoles> Cache
        {
            get { return cache ?? (cache = Engine.Resolve<Engine.StructureBoundDictionaryCache<string, CachedRoles>>()); }
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
            IList<ContentItem> users = Bridge.Repository.Find(Parameter.Equal("Name", usernameToMatch) & Parameter.Equal("Roles", roleName).Detail()).ToList();
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
            return Cache.GetValue(username, (un) =>
                {
                    Items.User u = Bridge.GetUser(username);
                    if (u != null)
                        return new CachedRoles { Roles = u.GetRoles() ?? new string[0] };

                    if (username == "admin")
                    {
                        return new CachedRoles { Roles = new string[] { "Administrators" } };
                    }
                    return new CachedRoles { Roles = new string[0] };
                }).Roles;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            IList<ContentItem> users = Bridge.Repository.Find(Parameter.Equal("Roles", roleName).Detail()).ToList();
            return ToArray(users);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return GetRolesForUser(username)
                .Any(r => r.Equals(roleName, StringComparison.OrdinalIgnoreCase));
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
