namespace N2.Templates.Security
{
	public class ItemBridge
	{
		readonly Definitions.IDefinitionManager definitions;
		Persistence.Finder.IItemFinder finder;
		readonly Persistence.IPersister persister;
		private string userContainerName = "TemplateUsers";
		private int userContainerParentID = 1;

		public ItemBridge(Definitions.IDefinitionManager definitions, Persistence.Finder.IItemFinder finder, Persistence.IPersister persister, N2.Web.Site site)
		{
			this.definitions = definitions;
			this.finder = finder;
			this.persister = persister;
			this.userContainerParentID = site.RootItemID;
		}

		public Persistence.Finder.IItemFinder Finder
		{
			get { return finder; }
			set { finder = value; }
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

		public virtual Items.User CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey)
		{
			Items.User u = definitions.CreateInstance<Items.User>(GetUserContainer());
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
			return GetUserContainer().GetChild(username) as Items.User;
		}

		public virtual Items.UserList GetUserContainer()
		{
			ContentItem parent = persister.Get(UserContainerParentID);
			Items.UserList m = parent.GetChild(UserContainerName) as Items.UserList;
			if (m == null)
			{
				m = Context.Definitions.CreateInstance<Items.UserList>(parent);
				m.Title = "Users";
				m.Name = UserContainerName;
				
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
