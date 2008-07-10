using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Profile;
using System.Configuration;
using N2.Security.Items;
using System.Diagnostics;

namespace N2.Security
{
    public class ContentProfileProvider : ProfileProvider
    {
        protected ItemBridge Bridge
        {
            get { return Context.Current.Resolve<ItemBridge>(); }
        }

        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            Trace.TraceWarning("ContentProfileProvider.DeleteInactiveProfiles Not Implemented");
            return 0;
        }

        public override int DeleteProfiles(string[] usernames)
        {
            int count = 0;
            foreach (string username in usernames)
            {
                ContentItem user = Bridge.GetUser(username);
                if (user != null)
                {
                    Bridge.Delete(user);
                    count++;
                }
            }
            return count;
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            string[] usernames = new string[profiles.Count];
            int i = 0;
            foreach(ProfileInfo profile in profiles)
            {
                usernames[i] = profile.UserName;
            }
            return DeleteProfiles(usernames);
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            Trace.TraceWarning("ContentProfileProvider.FindInactiveProfilesByUserName Not Implemented");
            return new ProfileInfoCollection();
        }

        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            ProfileInfoCollection profiles = new ProfileInfoCollection();
            User u = Bridge.GetUser(usernameToMatch);
            if (u != null)
            {
                totalRecords = 1;
                if(pageIndex == 0 && pageSize > 0)
                    profiles.Add(CreateProfile(u));
            }
            totalRecords = 0;
            return profiles;
        }

        private static ProfileInfo CreateProfile(User u)
        {
            return new ProfileInfo(u.Name, false, u.LastActivityDate, u.Updated, u.Details.Count);
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            Trace.TraceWarning("ContentProfileProvider.GetAllInactiveProfiles Not Implemented");
            return new ProfileInfoCollection();
        }

        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            ProfileInfoCollection profiles = new ProfileInfoCollection();
            UserList users = Bridge.GetUserContainer(false);
            if (users != null)
            {
                totalRecords = users.Children.Count;
                foreach(User u in users.GetChildren(new Collections.CountFilter(pageIndex * pageSize, pageSize)))
                    profiles.Add(CreateProfile(u));
            }
            totalRecords = 0;
            return profiles;
        }

        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            Trace.TraceWarning("ContentProfileProvider.GetNumberOfInactiveProfiles Not Implemented");
            return 0;
        }

        private string applicationName = "N2.Templates.Profiles";

        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        private User GetUser(SettingsContext context)
        {
            string username = (string)context["UserName"];
            if (!string.IsNullOrEmpty(username))
            {
                return Bridge.GetUser(username);
            }
            return null;
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection requestedProperties)
        {
            SettingsPropertyValueCollection properties = new SettingsPropertyValueCollection();
            if (requestedProperties.Count > 0)
            {
                User u = GetUser(context);
                if (u != null)
                {
                    foreach (SettingsProperty requestedProperty in requestedProperties)
                    {
                        SettingsPropertyValue property = new SettingsPropertyValue(requestedProperty);
                        property.PropertyValue = u[requestedProperty.Name];
                        properties.Add(property);
                    }
                }
            }
            return properties;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propertiesToChange)
        {
            if (propertiesToChange.Count > 0)
            {
                User u = GetUser(context);
                if (u != null)
                {
                    foreach (SettingsPropertyValue property in propertiesToChange)
                    {
                        u[property.Name] = property.PropertyValue;
                    }
                    Bridge.Save(u);
                }
            }
        }
    }
}
