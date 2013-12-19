using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using N2.Configuration;
using N2.Engine;
using N2.Persistence;
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
        private readonly Logger<ItemBridge> logger;
        private string userContainerName = "TemplateUsers";
        private string[] defaultRoles = { "Everyone", "Members", "Writers", "Editors", "Administrators" };
        readonly string[] editorUsernames = {"admin"};
        readonly string[] administratorUsernames = { "admin" };
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
            if (!typeof(User).IsAssignableFrom(configuredUserType)) throw new ArgumentException("Configured membership user type '" + config.Membership.UserType + "' doesn't derive from '" + typeof(User).AssemblyQualifiedName + "'");
            this.userType = configuredUserType;
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

            Save(u);
            
            return u;
        }

        public virtual Items.User GetUser(string username)
        {
            try
            {
                IList<Items.User> users = GetUsers(username, 0, 1);
                if (users.Count == 0)
                    return null;
                return users[0];
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

            Save(m);
            return m;
        }

        public virtual void Delete(ContentItem item)
        {
            using (security.Disable())
            {
                security.ScopeEnabled = false;
                persister.Delete(item);

                if (UserDeleted != null)
                    UserDeleted(this, new ItemEventArgs(item));
            }
        }

        public virtual void Save(ContentItem item)
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
