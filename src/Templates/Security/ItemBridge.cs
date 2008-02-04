using N2.Definitions;
using N2.Persistence.Finder;
using N2.Persistence;
using N2.Web;

namespace N2.Templates.Security
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
		private string userContainerName = "TemplateUsers";
		private int userContainerParentID = 1;
		private string[] defaultRoles = new string[] { "Everyone", "Editors", "Administrators" };

		public ItemBridge(IDefinitionManager definitions, IItemFinder finder, IPersister persister, Site site)
		{
			this.definitions = definitions;
			this.finder = finder;
			this.persister = persister;
			this.userContainerParentID = site.RootItemID;
		}

		public IItemFinder Finder
		{
			get { return finder; }
		}
		
		protected int UserContainerParentID
		{
			get { return userContainerParentID; }
			set { userContainerParentID = value; }
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
			Items.User u = definitions.CreateInstance<Items.User>(GetUserContainer(true));
			u.Title = username;
			u.Name = username;
			u.Password = password;
			u.Email = email;
			u.PasswordQuestion = passwordQuestion;
			u.PasswordAnswer = passwordAnswer;
			u.IsApproved = isApproved;
			u.ProviderUserKey = providerUserKey;
			
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
				m = Context.Definitions.CreateInstance<Items.UserList>(parent);
				m.Title = "Users";
				m.Name = UserContainerName;
				foreach (string role in DefaultRoles)
				{
					m.AddRole(role);
				}
				
				persister.Save(m);
			}
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
	}
}
