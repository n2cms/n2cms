using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace N2.Management.Api
{
    public class ProfileUser
    {
        public ProfileUser()
        {
            Settings = new Dictionary<string, object>();
        }

        public string Name { get; set; }

        public string Email { get; set; }

        public IDictionary<string, object> Settings { get; set; }
    }

    public interface IProfileRepository
    {
        ProfileUser Get(string username);
        void Save(ProfileUser user);
    }

    public static class ProfileRepositoryExtensions
    {
        public static ProfileUser Get(this IProfileRepository repository, IPrincipal user)
        {
            string username = GetUserName(user);
            if (string.IsNullOrEmpty(username))
                return new ProfileUser { Name = "Anonymous" };
            return repository.Get(username);
        }

        public static ProfileUser GetOrCreate(this IProfileRepository repository, IPrincipal user)
        {
            string username = GetUserName(user);
            if (string.IsNullOrEmpty(username))
                return new ProfileUser { Name = "Anonymous" };
            
            return GetOrCreate(repository, username);
        }

		public static object GetProfileSetting(this IProfileRepository repository, IPrincipal user, string settingKey)
		{
			string username = GetUserName(user);
			if (string.IsNullOrEmpty(username))
				return null;

			var profile = GetOrCreate(repository, username);
			return profile.Settings.TryGet("LastDismissed");
		}

        public static ProfileUser GetOrCreate(this IProfileRepository repository, string username)
        {
            var user = repository.Get(username);
            if (user != null)
                return user;

            return new ProfileUser { Name = username };
        }

        private static string GetUserName(IPrincipal user)
        {
            return user != null ? user.Identity != null ? user.Identity.Name : null : null;
        }
    }

    [Service(typeof(IProfileRepository))]
    public class InMemoryProfileRepository : IProfileRepository
    {
        Dictionary<string, ProfileUser> users = new Dictionary<string, ProfileUser>();

        public ProfileUser Get(string username)
        {
            if (users.ContainsKey(username))
                return users[username];
            else
                return null;
        }

        public void Save(ProfileUser user)
        {
            users[user.Name] = user;
        }
    }
}
