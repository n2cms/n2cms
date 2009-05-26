using System;
using N2.Definitions;
using N2.Persistence.Finder;
using N2.Persistence;
using N2.Web;
using N2.Web.UI;
using N2.Configuration;
using System.Collections.Specialized;

namespace N2.Security
{
	/// <summary>
	/// Provides access to users and roles stored as nodes in the item 
	/// hierarchy.
	/// </summary>
	public class ItemBridge
	{
		readonly private IDefinitionManager definitions;
		readonly private IItemFinder finder;
		readonly private IPersister persister;
	    private readonly IHost host;
		private string userContainerName = "TemplateUsers";
		private string[] defaultRoles = new string[] { "Everyone", "Members", "Editors", "Administrators", "Writers" };
		string[] editorUsernames = new string[] {"admin"};
		string[] administratorUsernames = new string[] { "admin" };

		public ItemBridge(IDefinitionManager definitions, IItemFinder finder, IPersister persister, IHost host, EditSection config)
			: this(definitions, finder, persister, host)
		{
			editorUsernames = ToArray(config.Editors.Users);
			administratorUsernames = ToArray(config.Administrators.Users);
		}

		public ItemBridge(IDefinitionManager definitions, IItemFinder finder, IPersister persister, IHost host)
		{
			this.definitions = definitions;
			this.finder = finder;
			this.persister = persister;
		    this.host = host;
		}

		public IItemFinder Finder
		{
			get { return finder; }
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

			Items.User u = definitions.CreateInstance<Items.User>(GetUserContainer(true));
			u.Title = username;
			u.Name = username;
			u.Password = password;
			u.Email = email;
			u.PasswordQuestion = passwordQuestion;
			u.PasswordAnswer = passwordAnswer;
			u.IsApproved = isApproved;
			
			persister.Save(u);
			
			return u;
		}

		public virtual Items.User GetUser(string username)
		{
			Items.UserList users = GetUserContainer(false);
			if(users == null)
				return null;
			return users.GetChild(username) as Items.User;
		}

		public virtual Items.UserList GetUserContainer(bool create)
		{
			ContentItem parent = persister.Get(UserContainerParentID);
			Items.UserList m = parent.GetChild(UserContainerName) as Items.UserList;
			if (m == null && create)
			{
				m = CreateUserContainer(parent);
			}
			return m;
		}

		protected Items.UserList CreateUserContainer(ContentItem parent)
		{
			Items.UserList m = Context.Definitions.CreateInstance<Items.UserList>(parent);
			m.Title = "Users";
			m.Name = UserContainerName;
			foreach (string role in DefaultRoles)
			{
				m.AddRole(role);
			}

			persister.Save(m);
			return m;
		}

		public virtual void Delete(ContentItem item)
		{
			persister.Delete(item);
		}

		public virtual void Save(ContentItem item)
		{
			persister.Save(item);
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
