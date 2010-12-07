using System;
using System.Collections.Generic;
using System.Web.Security;

namespace N2.Security
{
	/// <summary>
	/// Implements the default ASP.NET membership provider. Stores users as 
	/// nodes in the N2 item hierarchy.
	/// </summary>
	public class ContentMembershipProvider : MembershipProvider
	{
		ItemBridge bridge;

		public ContentMembershipProvider() {}
		public ContentMembershipProvider(ItemBridge bridge)
		{
			this.bridge = bridge;
		}

		protected virtual ItemBridge Bridge
		{
			get { return bridge ?? (bridge = Context.Current.Resolve<ItemBridge>()); }
		}

		private string applicationName = "N2.Security";

		public override string ApplicationName
		{
			get { return applicationName; }
			set { applicationName = value; }
		}

		public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			Items.User u = Bridge.GetUser(username);
			if (u == null || u.Password != oldPassword)
				return false;
			u.Password = newPassword;
			Bridge.Save(u);
			return true;
		}

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			Items.User u = Bridge.GetUser(username);
			if (u == null || u.Password != password)
				return false;
			u.PasswordQuestion = newPasswordQuestion;
			u.PasswordAnswer = newPasswordAnswer;
			Bridge.Save(u);
			return true;
		}

		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{
			Items.User u = Bridge.GetUser(username);
			if (u != null)
			{
				status = MembershipCreateStatus.DuplicateUserName;
				return null;
			}
			if (string.IsNullOrEmpty(username))
			{
				status = MembershipCreateStatus.InvalidUserName;
				return null;
			}
			if (string.IsNullOrEmpty(password))
			{
				status = MembershipCreateStatus.InvalidPassword;
				return null;
			}
			status = MembershipCreateStatus.Success;

			u = Bridge.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey);

			MembershipUser m = u.GetMembershipUser(this.Name);
			return m;
		}

		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			Items.User u = Bridge.GetUser(username);
			if (u == null)
				return false;
			Bridge.Delete(u);
			return true;
		}

		public override bool EnablePasswordReset
		{
			get { return false; }
		}

		public override bool EnablePasswordRetrieval
		{
			get { return false; }
		}

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			MembershipUserCollection muc = new MembershipUserCollection();
			Items.UserList userContainer = Bridge.GetUserContainer(false);
			if (userContainer == null)
			{
				totalRecords = 0;
				return muc;
			}
			IList<ContentItem> users = Bridge.Finder
				.Where.Detail("Email").Eq(emailToMatch)
				.And.Type.Eq(typeof(Items.User))
				.And.Parent.Eq(userContainer)
				.Select();
			totalRecords = users.Count;
			Collections.CountFilter.Filter(users, pageIndex * pageSize, pageSize);

			foreach (Items.User u in users)
				muc.Add(u.GetMembershipUser(Name));
			return muc;
		}

		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			IList<Items.User> users = Bridge.GetUsers(usernameToMatch, pageIndex*pageSize, pageSize);
            totalRecords = users.Count;

            MembershipUserCollection muc = new MembershipUserCollection();
            foreach (var user in users)
                muc.Add(user.GetMembershipUser(Name));

			return muc;
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			Items.UserList users = Bridge.GetUserContainer(false);
			if (users == null)
			{
				totalRecords = 0;
				return new MembershipUserCollection();
			}
			return users.GetMembershipUsers(Name, pageIndex * pageSize, pageSize, out totalRecords);
		}

		public override int GetNumberOfUsersOnline()
		{
			Items.UserList users = Bridge.GetUserContainer(false);
			if (users == null)
				return 0;
			int online = 0;
			foreach (Items.User u in users.Children)
				if (u.LastActivityDate < DateTime.Now.AddMinutes(-20))
					online++;
			return online;
		}

		public override string GetPassword(string username, string answer)
		{
			Items.User u = Bridge.GetUser(username);
			if (u != null && u.PasswordAnswer.Equals(answer, StringComparison.InvariantCultureIgnoreCase))
				return u.Password;
			return null;
		}

		public override MembershipUser GetUser(string username, bool userIsOnline)
		{
			Items.User u = Bridge.GetUser(username);
			if (u != null)
			{
				if (userIsOnline)
					u.LastActivityDate = DateTime.Now;
				return u.GetMembershipUser(this.Name);
			}
			return null;
		}

		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			Items.UserList userContainer = Bridge.GetUserContainer(false);
			if (userContainer == null)
				return null;
			
			int _userId = 0;
			
			if(providerUserKey is int) {
				_userId = (int)providerUserKey;
			} else if(providerUserKey is Guid) {
				/// http://forums.asp.net/t/1266765.aspx
				Guid guid = (Guid)providerUserKey;
				//extract an integer from the beginning of the Guid
				byte[] _bytes = guid.ToByteArray();
				_userId = ((int)_bytes[0]) | ((int)_bytes[1] << 8) | ((int)_bytes[2] << 16) | ((int)_bytes[3] << 24);
			}
			
			IList<ContentItem> users = Bridge.Finder
				.Where.ID.Eq(_userId)
				.And.Type.Eq(typeof(Items.User))
				.And.Parent.Eq(userContainer)
				.Select();
			
			foreach (Items.User u in users)
			{
				if(userIsOnline)
					u.LastLoginDate = DateTime.Now;
				return u.GetMembershipUser(Name);
			}
			return null;
		}

		public override string GetUserNameByEmail(string email)
		{
			Items.UserList userContainer = Bridge.GetUserContainer(false);
			if (userContainer == null)
				return null;
			IList<ContentItem> users = Bridge.Finder
				.Where.Detail("Email").Eq(email)
				.And.Type.Eq(typeof(Items.User))
				.And.Parent.Eq(userContainer)
				.Select();
			foreach (Items.User u in users)
				return u.Name;
			return null;
		}

		public override int MaxInvalidPasswordAttempts
		{
			get { return 4; }
		}

		public override int MinRequiredNonAlphanumericCharacters
		{
			get { return 0; }
		}

		public override int MinRequiredPasswordLength
		{
			get { return 5; }
		}

		public override int PasswordAttemptWindow
		{
			get { return 20; }
		}

		public override MembershipPasswordFormat PasswordFormat
		{
			get { return MembershipPasswordFormat.Clear; }
		}

		public override string PasswordStrengthRegularExpression
		{
			get { return ".*"; }
		}

		public override bool RequiresQuestionAndAnswer
		{
			get { return false; }
		}

		public override bool RequiresUniqueEmail
		{
			get { return false; }
		}

		public override string ResetPassword(string username, string answer)
		{
			Items.User u = Bridge.GetUser(username);
			if (u != null)
			{
				u.IsLockedOut = false;
				Bridge.Save(u);
				return u.Password;
			}
			return null;
		}

		public override bool UnlockUser(string userName)
		{
			Items.User u = Bridge.GetUser(userName);
			if (u == null)
				return false;
			u.IsLockedOut = false;
			Bridge.Save(u);
			return true;
		}

		public override void UpdateUser(MembershipUser user)
		{
			if (user == null) throw new ArgumentNullException("user");

			Items.User u = Bridge.GetUser(user.UserName);
			if (u != null)
			{
				u.UpdateFromMembershipUser(user);
				Bridge.Save(u);
			}
			else
				throw new N2Exception("User '" + user.UserName + "' not found.");
		}

		public override bool ValidateUser(string username, string password)
		{
			Items.User u = Bridge.GetUser(username);
			if (u != null && u.Password == password)
				return true;
			return false;
		}
	}
}
