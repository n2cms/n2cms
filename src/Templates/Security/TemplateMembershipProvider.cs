using System;
using System.Collections.Generic;
using System.Web.Security;

namespace N2.Templates.Security
{
	public class TemplateMembershipProvider : MembershipProvider
	{
		protected ItemBridge Bridge
		{
			get { return Context.Instance.Resolve<ItemBridge>(); }
		}

		private string applicationName = "N2.Templates.Membership";

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
			IList<ContentItem> users = Bridge.Finder
				.Where.Detail("Email").Eq(emailToMatch)
				.And.Type.Eq(typeof(Items.User))
				.Filters(new Collections.ParentFilter(Bridge.GetUserContainer()))
				.Select();
			totalRecords = users.Count;
			Collections.CountFilter.Filter(users, pageIndex * pageSize, pageSize);

			MembershipUserCollection muc = new MembershipUserCollection();
			foreach (Items.User u in users)
				muc.Add(u.GetMembershipUser(Name));
			return muc;
		}

		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			MembershipUserCollection muc = new MembershipUserCollection();
			Items.User u = Bridge.GetUser(usernameToMatch);
			if (u == null)
			{
				totalRecords = 0;
			}
			else
			{
				totalRecords = 1;
				if (pageIndex == 0 && pageSize > 0)
				{
					muc.Add(u.GetMembershipUser(Name));
				}
			}
			return muc;
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			return Bridge.GetUserContainer().GetMembershipUsers(Name, pageIndex * pageSize, pageSize, out totalRecords);
		}

		public override int GetNumberOfUsersOnline()
		{
			int online = 0;
			foreach (Items.User u in Bridge.GetUserContainer().Children)
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
			IList<ContentItem> users = Bridge.Finder
				.Where.Detail("ProviderUserKey").Eq(providerUserKey)
				.And.Type.Eq(typeof(Items.User))
				.Filters(new Collections.ParentFilter(Bridge.GetUserContainer()))
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
			IList<ContentItem> users = Bridge.Finder
				.Where.Detail("Email").Eq(email)
				.And.Type.Eq(typeof(Items.User))
				.Filters(new Collections.ParentFilter(Bridge.GetUserContainer()))
				.Select();
			foreach (Items.User u in users)
				return u.Name;
			return null;
		}

		public override int MaxInvalidPasswordAttempts
		{
			get { return 3; }
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
			return true;
		}

		public override void UpdateUser(MembershipUser user)
		{
			Items.User u = Bridge.GetUser(user.UserName);
			if (u != null)
				u.UpdateFromMembershipUser(user);
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
