using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using N2.Security;

namespace N2.Configuration
{
    public class PermissionElement : ConfigurationElement
    {
        [ConfigurationProperty("dynamic", DefaultValue = true)]
        public bool Dynamic
        {
            get { return (bool)base["dynamic"]; }
            set { base["dynamic"] = value; }
        }

        [ConfigurationProperty("users"), TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        public StringCollection Users
        {
            get { return (CommaDelimitedStringCollection)base["users"]; }
            set { base["users"] = value; }
        }

        [ConfigurationProperty("roles"), TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        public StringCollection Roles 
        {
            get { return (CommaDelimitedStringCollection)base["roles"]; }
            set { base["roles"] = value; }
        }

        public PermissionMap ToPermissionMap(Permission permission, string[] defaultRoles, string[] defaultUsers)
        {
            PermissionMap map = Dynamic ? new DynamicPermissionMap() : new PermissionMap();
            map.Permissions = permission;
            map.Roles = ToArray(Roles, defaultRoles);
            map.Users = ToArray(Users, defaultUsers);
            map.IsAltered = Roles != null || Users != null;
            return map;
        }

        private string[] ToArray(StringCollection list, string[] defaultValue)
        {
            if(list == null)
                return defaultValue;

            string[] array = new string[list.Count];
            list.CopyTo(array, 0);
            return array;
        }
    }
}
