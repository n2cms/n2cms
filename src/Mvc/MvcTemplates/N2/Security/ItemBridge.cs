using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using N2.Configuration;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Security.Items;
using N2.Web;
using System.Linq;

namespace N2.Security
{
	/// <summary>
	/// Provides access to users and roles stored as nodes in the item 
	/// hierarchy.
	/// </summary>
	[Service]
	public class ItemBridge
	{
		readonly private ContentActivator activator;
		readonly private IPersister persister;
		readonly private ISecurityManager security;
	    private readonly IHost host;
		private readonly Engine.Logger<ItemBridge> logger;
		private string userContainerName = "TemplateUsers";
		private string[] defaultRoles = new string[] { "Everyone", "Members", "Writers", "Editors", "Administrators" };
		string[] editorUsernames = new string[] {"admin"};
		string[] administratorUsernames = new string[] { "admin" };
		Type userType = typeof(User);

		public event EventHandler<ItemEventArgs> UserDeleted;
		public event EventHandler<ItemEventArgs> UserSaved;

		public ItemBridge(ContentActivator activator, IPersister persister, ISecurityManager security, IHost host, EditSection config)
		{
			this.security = security;
			this.activator = activator;
			this.persister = persister;
			this.host = host;
			this.editorUsernames = ToArray(config.Editors.Users);
			this.administratorUsernames = ToArray(config.Administrators.Users);

			Type configuredUserType = Type.GetType(config.Membership.UserType);
			if (configuredUserType == null) throw new ArgumentException("Couldn't create configured membership user type: " + config.Membership.UserType);
            SetUserType(configuredUserType);
		}

        /// <summary>
        /// Define N2 User type (application wide)
        /// </summary>
        /// <remarks>
        /// User type shall be assignable to <see cref="User"/> basic user type.
        /// User type defaults to <see cref="User"/> basic user type or type defined by N2 <see cref="MembershipElement.UserType"/> configuration parameter.
        /// User type may be explicitly set at start of application, e.g. by custom account system what overrides configured type.
        /// Limitations: All user records shall be exactly of a specified user type, any records of other user types are ignored by Bridge.
        ///              Data migration shall be planned when introducing new user type.
        /// </remarks>
        public void SetUserType(Type userType)
        {
            if (!typeof(User).IsAssignableFrom(userType))
                throw new ArgumentException("Configured membership user type '" + userType.AssemblyQualifiedName + "' doesn't derive from '" + typeof(User).AssemblyQualifiedName + "'");
            this.userType = userType;
        }

        public IContentItemRepository Repository
		{
			get { return persister.Repository; }
		}
		
		protected int UserContainerParentID
		{
			get { return host.CurrentSite.RootItemID; }
		}

		public string UserContainerName
		{
			get { return userContainerName; }
			set { userContainerName = value; }
		}

		public string[] DefaultRoles
		{
			get { return defaultRoles; }
			set { defaultRoles = value; }
		}

        #region User

        public virtual Items.User CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey)
		{
			if(IsEditorOrAdmin(username))
				throw new ArgumentException("Invalid username.", "username");

			User u = (User)activator.CreateInstance(userType, GetUserContainer(true));
			u.Title = username;
			u.Name = username;
			u.Password = password;
			u.Email = email;
			u.PasswordQuestion = passwordQuestion;
			u.PasswordAnswer = passwordAnswer;
			u.IsApproved = isApproved;

			SaveUser(u);
			
			return u;
		}

		public virtual Items.User GetUser(string username)
		{
			try
			{
                /*
				IList<Items.User> users = GetUsers(username, 0, 1);
				if (users.Count == 0)
					return null;
				return users[0];
                */

                Items.UserList users = GetUserContainer(false);
                if (users == null)
                    return null;

                return Repository.Find(Parameter.Equal("Parent", users) & Parameter.TypeEqual(userType.Name) & Parameter.Equal("Name", username))
                                 .Cast<User>().FirstOrDefault();         
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				return null;
			}
        }

		public virtual IList<Items.User> GetUsers(string username, int firstResult, int maxResults)
		{
			Items.UserList users = GetUserContainer(false);
			if (users == null)
				return new List<Items.User>();

            return Repository.Find((Parameter.Equal("Parent", users) & Parameter.TypeEqual(userType.Name) & Parameter.Like("Name", username)).Skip(firstResult).Take(maxResults))
				.OfType<User>().ToList();
		}

        public virtual IEnumerable<Items.User> GetUsers(int firstResult, int maxResults)
        {
            Items.UserList users = GetUserContainer(false);
            if (users == null)
                return new List<Items.User>();

            return users.Children.FindRange(firstResult, maxResults).OfType<Items.User>();
            // return Repository.Find((Parameter.Equal("Parent", users) & Parameter.TypeEqual(userType.Name)).Skip(firstResult).Take(maxResults))
            //    .OfType<User>();
        }

        public virtual int GetUsersCount()
        {
            Items.UserList users = GetUserContainer(false);
            if (users == null)
                return 0;
            return users.Children.OfType<User>().Count(); 
        }

        /// <summary> Exist one or more users in specified role? </summary>
        public virtual bool HasUsersInRole(string roleName)
        {
            foreach (var userName in GetUsersInRole(roleName, 1))
                return true;
            return false;
        }

        /// <summary> Returns users (UserNames) in specified role </summary>
        public virtual string[] GetUsersInRole(string roleName, int maxResults)
        {
            Items.UserList users = GetUserContainer(false);
            if (users == null)
                return new string[] {};

            return Repository.Find((Parameter.Equal("Parent", users) & Parameter.TypeEqual(userType.Name) & Parameter.Equal("Roles",roleName).SetDetail(true)).Take(maxResults))
              .OfType<User>().Select(u => u.Name).ToArray();
        }

        public virtual void SaveUser(Items.User user) 
        { 
            SaveItem(user); 
        }

        public virtual void DeleteUser(Items.User user) 
        { 
            DeleteItem(user); 
        }

        #endregion

        #region UserList

        public virtual Items.UserList GetUserContainer(bool create)
		{
			var parents = Repository.Find(Parameter.Equal("Parent.ID", UserContainerParentID) & Parameter.Equal("Name", UserContainerName));
			foreach (var container in parents)
			{
				return (Items.UserList)container;
			}
			
			if (!create)
				return null;

			ContentItem parent = persister.Get(UserContainerParentID);
			return CreateUserContainer(parent);
		}

		protected Items.UserList CreateUserContainer(ContentItem parent)
		{
			Items.UserList m = activator.CreateInstance<Items.UserList>(parent);
			m.Title = "Users";
			m.Name = UserContainerName;
			foreach (string role in DefaultRoles)
			{
				m.AddRole(role);
			}

            SaveUserContainer(m);
			return m;
		}

        public virtual void SaveUserContainer(Items.UserList userList) { SaveItem(userList); }

        #endregion

        #region UserLoginInfo

        public virtual UserLogin FindLogin(string loginProvider, string providerKey)
        {
            return FindUserLogin(null, loginProvider, providerKey);
        }

        public virtual UserLogin FindUserLogin(Items.User user, string loginProvider, string providerKey)
        {
            return Repository.Find(UserLogin.QueryLoginInfoParameter(loginProvider, providerKey, user))
                         .OfType<UserLogin>()
                         .FirstOrDefault();
        }

        public virtual IEnumerable<UserLogin> FindUserLogins(Items.User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return user.GetChildren().OfType<UserLogin>();
        }

        public virtual bool DeleteUserLogin(Items.User user, string loginProvider, string providerKey) 
        {
            if (user == null)
                throw new ArgumentNullException("user");
            var loginItem = FindUserLogin(user, loginProvider, providerKey);
            if (loginItem == null)
                return false;
            DeleteItem(loginItem);
            return true;
        }

        public virtual bool AddUserLogin(Items.User user, string loginProvider, string providerKey)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (FindUserLogin(user, loginProvider, providerKey) != null)
                return false;
            var loginItem = activator.CreateInstance<UserLogin>(user);
            loginItem.LoginProvider = loginProvider;
            loginItem.ProviderKey = providerKey;
            SaveItem(loginItem);
            return true;
        }

        #endregion

        [Obsolete("Use specialized methods instead, e.g. DeleteUser")]
		public virtual void Delete(ContentItem item) { DeleteItem(item); }

        protected virtual void DeleteItem(ContentItem item)
		{
			using (security.Disable())
			{
				security.ScopeEnabled = false;
				persister.Delete(item);

				if (UserDeleted != null)
					UserDeleted(this, new ItemEventArgs(item));
			}
		}

        [Obsolete("Use specialized methods instead, e.g. SaveUser, SaveUserList")]
        public virtual void Save(ContentItem item) { SaveItem(item); }

        protected virtual void SaveItem(ContentItem item)
		{
			using (security.Disable())
			{
				security.ScopeEnabled = false;
				using (var tx = persister.Repository.BeginTransaction())
				{
					persister.Repository.SaveOrUpdate(item);
					tx.Commit();
				}
				if (UserSaved != null)
					UserSaved(this, new ItemEventArgs(item));
			}
		}

		string[] ToArray(StringCollection users)
		{
			if(users == null) return new string[0];
			
			string[] userArray = new string[users.Count];
			for (int i = 0; i < users.Count; i++)
			{
				userArray[i] = users[0];
			}
			return userArray;
		}

		private bool IsEditorOrAdmin(string username)
		{
			if (Array.Exists(editorUsernames, un => un.Equals(username, StringComparison.InvariantCultureIgnoreCase)))
				return true;
			if (Array.Exists(administratorUsernames, un => un.Equals(username, StringComparison.InvariantCultureIgnoreCase)))
				return true;
			return false;
		}

    }
}
