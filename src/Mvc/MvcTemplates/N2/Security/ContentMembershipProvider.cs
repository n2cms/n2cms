using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Security;

namespace N2.Security
{
	/// <summary>
	/// Implements the default ASP.NET membership provider. Stores users as 
	/// nodes in the N2 item hierarchy.
	/// </summary>
	/// <remarks>
	///  Password is stored as plain text by default. Hashed and Encrypted passwords are supported as well.
	///  See MSDN description how to configure provider properties
	///   http://msdn.microsoft.com/en-us/library/6d4936ht.aspx
	///   Password reset <see cref="ResetPassword"/> method generates a random password,
	///   generated password length is defined by custom <see cref="generatedPasswordLength"/> 
	///   configuration property. Generated legth is greather or equal to <see cref="minRequiredPasswordLength"/>
	///   configuration property.
	///  See also an intro to machineKey configuration
	///   http://stackoverflow.com/questions/957068/asp-net-machinekey-validationkey-and-decryptionkey-key-lengths
	/// </remarks>
	public class ContentMembershipProvider : MembershipProvider
	{
		ItemBridge bridge;

		// Note: see MSDN for additional info
		// http://msdn.microsoft.com/en-us/library/9x1zytyd.aspx
		private bool enablePasswordReset = true;  // default value is true (MSDN)
		private bool enablePasswordRetrieval = false; // default value is false (MSDN)
		int maxInvalidPasswordAttempts = 4; // note: tracking is not implemented by the provider yet
		private int minRequiredNonAlphanumericCharacters = 0;
		private int minRequiredPasswordLength = 5;
		private int generatedPasswordLength = 6;
		private int passwordAttemptWindow = 20;
		private MembershipPasswordFormat passwordFormat = MembershipPasswordFormat.Clear;
		private string passwordStrengthRegularExpression = ".*";
		private bool requiresQuestionAndAnswer = false; // default value is true (MSDN), Breaking change: N2 version 2.2 and older default value was false.
		private bool requiresUniqueEmail = true; // default value is true (MSDN), Breaking change: N2 version 2.2 and older default value was false. Note: still unused by the provider.

		public ContentMembershipProvider()
		{
		}

		public ContentMembershipProvider(ItemBridge bridge)
			: this()
		{
			this.bridge = bridge;
		}

		protected virtual ItemBridge Bridge
		{
			get { return bridge ?? (bridge = Context.Current.Resolve<ItemBridge>()); }
		}

		#region provider properties

		/// <summary> Initialize provider from system configuration </summary>
		/// <param name="name"> The friendly name of the provider </param>
		/// <param name="config"> A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider </param>
		public override void Initialize(string name, NameValueCollection config)
		{
			if (config == null)
				throw new ArgumentNullException("config required");

			if (string.IsNullOrEmpty(name))
				name = "ContentMembershipProvider";

			if (string.IsNullOrEmpty(config["description"]))
			{
				config.Remove("description");
				config.Add("description", "N2 Content Membership provider");
			}

			bool bvalue;
			int ivalue;

			if (bool.TryParse(config["enablePasswordReset"], out bvalue))
				enablePasswordReset = bvalue;

			if (bool.TryParse(config["enablePasswordRetrieval"], out bvalue))
				enablePasswordRetrieval = bvalue;

			if (int.TryParse(config["maxInvalidPasswordAttempts"], out ivalue))
				maxInvalidPasswordAttempts = ivalue;

			if (int.TryParse(config["minRequiredNonAlphanumericCharacters"], out ivalue))
				minRequiredNonAlphanumericCharacters = ivalue;

			if (int.TryParse(config["minRequiredPasswordLength"], out ivalue))
				minRequiredPasswordLength = ivalue;

			if (int.TryParse(config["generatedPasswordLength"], out ivalue))
				generatedPasswordLength = ivalue;
			generatedPasswordLength = Math.Max(generatedPasswordLength, minRequiredPasswordLength);

			if (int.TryParse(config["passwordAttemptWindow"], out ivalue))
				passwordAttemptWindow = ivalue;

			try
			{
				passwordFormat = (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), config["passwordFormat"]);
			}
			catch (Exception)
			{
			}

			if (!string.IsNullOrEmpty(config["description"]))
				passwordStrengthRegularExpression = config["description"];

			if (bool.TryParse(config["requiresQuestionAndAnswer"], out bvalue))
				requiresQuestionAndAnswer = bvalue;

			if (bool.TryParse(config["requiresUniqueEmail"], out bvalue))
				requiresUniqueEmail = bvalue;

			if (!string.IsNullOrEmpty(config["applicationName"]))
				ApplicationName = config["applicationName"];
			if (string.IsNullOrEmpty(ApplicationName))
				ApplicationName = "N2.Security";

			// Initialize the abstract base class.
			base.Initialize(name, config);
		}

		public override bool EnablePasswordReset { get { return enablePasswordReset; } }

		public override bool EnablePasswordRetrieval { get { return enablePasswordRetrieval; } }

		public override int MaxInvalidPasswordAttempts { get { return maxInvalidPasswordAttempts; } }

		public override int MinRequiredNonAlphanumericCharacters { get { return minRequiredNonAlphanumericCharacters; } }

		public override int MinRequiredPasswordLength { get { return minRequiredPasswordLength; } }

		public virtual int GeneratedPasswordLength { get { return generatedPasswordLength; } }

		public override int PasswordAttemptWindow { get { return passwordAttemptWindow; } }

		public override MembershipPasswordFormat PasswordFormat { get { return passwordFormat; } }

		public override string PasswordStrengthRegularExpression { get { return passwordStrengthRegularExpression; } }

		public override bool RequiresQuestionAndAnswer { get { return requiresQuestionAndAnswer; } }

		public override bool RequiresUniqueEmail { get { return requiresUniqueEmail; } }
		
		public override string ApplicationName { get; set; }

		#endregion

		/// <summary> Password value to be persiste. <seealso cref="PasswordFormat"/></summary>
		/// <param name="password"> Plain text password value </param>
		/// <remarks> 
		///   Default implementation:
		///   Unmodified password value returned for Clear password format,
		///   Hashed(SHA1) and Encrypted passwords using machineKey configuration settings.
		/// </remarks>
		protected virtual string ToStoredPassword(string password)  // JH
		{
			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Clear:
					return password;
				case MembershipPasswordFormat.Hashed:
					return FormsAuthentication.HashPasswordForStoringInConfigFile(password, System.Web.Configuration.FormsAuthPasswordFormat.SHA1.ToString());
				default: // MembershipPasswordFormat.Encrypted
					return Convert.ToBase64String(base.EncryptPassword(System.Text.Encoding.UTF8.GetBytes(password)));
			}
		}

		protected virtual string FromStoredPassword(string storedPassword)
		{
			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Clear:
					return storedPassword;
				case MembershipPasswordFormat.Hashed:
					throw new NotSupportedException("Hashed password cannot be restored");
				default: // MembershipPasswordFormat.Encrypted
					return System.Text.Encoding.UTF8.GetString(base.DecryptPassword(Convert.FromBase64String(storedPassword)));
			}
		}

		/// <summary> Generate new random password <seealso cref="GeneratedPasswordLength"/></summary>
		/// <remarks> See MSDN ResetPassword: 
		///           The random password created by the ResetPassword method is not guaranteed 
		///           to pass the regular expression in the <see cref="PasswordStrengthRegularExpression"/> property. 
		///           However, the random password will meet the criteria established 
		///           by the <see cref="MinRequiredPasswordLength"/> 
		///           and <see cref="MinRequiredNonAlphanumericCharacters"/> properties.
		/// </remarks>
		protected virtual string GenerateRandomPassword() // JH
		{
			return Membership.GeneratePassword(GeneratedPasswordLength, MinRequiredNonAlphanumericCharacters);
		}

		public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			N2.Security.Items.User u = Bridge.GetUser(username);
			if (u == null || u.Password != ToStoredPassword(oldPassword)) // JH
				return false;
			u.Password = ToStoredPassword(newPassword); // JH
			Bridge.Save(u);
			return true;
		}

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			N2.Security.Items.User u = Bridge.GetUser(username);
			if (u == null || u.Password != ToStoredPassword(password)) // JH
				return false;
			u.PasswordQuestion = newPasswordQuestion;
			u.PasswordAnswer = newPasswordAnswer;
			Bridge.Save(u);
			return true;
		}

		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{
			N2.Security.Items.User u = Bridge.GetUser(username);
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

			// TODO: RequiresUniqueEmail validation

			var args = new ValidatePasswordEventArgs(username, password, true);
			OnValidatingPassword(args);
			if (args.Cancel)
				throw new MembershipCreateUserException("Create user cancelled", args.FailureInformation);

			status = MembershipCreateStatus.Success;
			u = Bridge.CreateUser(username, ToStoredPassword(password), // JH
								  email, passwordQuestion, passwordAnswer, isApproved, providerUserKey);

			MembershipUser m = u.GetMembershipUser(this.Name);
			return m;
		}

		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			N2.Security.Items.User u = Bridge.GetUser(username);
			if (u == null)
				return false;
			Bridge.Delete(u);
			return true;
		}

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			MembershipUserCollection muc = new MembershipUserCollection();
			N2.Security.Items.UserList userContainer = Bridge.GetUserContainer(false);
			if (userContainer == null)
			{
				totalRecords = 0;
				return muc;
			}
			IList<ContentItem> users = Bridge.Finder
			  .Where.Detail("Email").Eq(emailToMatch)
			  .And.Type.Eq(typeof(N2.Security.Items.User))
			  .And.Parent.Eq(userContainer)
			  .Select();
			totalRecords = users.Count;
			N2.Collections.CountFilter.Filter(users, pageIndex * pageSize, pageSize);

			foreach (N2.Security.Items.User u in users)
				muc.Add(u.GetMembershipUser(Name));
			return muc;
		}

		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			IList<N2.Security.Items.User> users = Bridge.GetUsers(usernameToMatch, pageIndex * pageSize, pageSize);
			totalRecords = users.Count;

			MembershipUserCollection muc = new MembershipUserCollection();
			foreach (var user in users)
				muc.Add(user.GetMembershipUser(Name));

			return muc;
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			N2.Security.Items.UserList users = Bridge.GetUserContainer(false);
			if (users == null)
			{
				totalRecords = 0;
				return new MembershipUserCollection();
			}
			return users.GetMembershipUsers(Name, pageIndex * pageSize, pageSize, out totalRecords);
		}

		public override int GetNumberOfUsersOnline()
		{
			N2.Security.Items.UserList users = Bridge.GetUserContainer(false);
			if (users == null)
				return 0;
			int online = 0;
			int userIsOnlineTimeWindow = (Membership.UserIsOnlineTimeWindow > 0 ? Membership.UserIsOnlineTimeWindow : 20);
			foreach (N2.Security.Items.User u in users.Children)
				if (u.LastActivityDate < DateTime.Now.AddMinutes(-userIsOnlineTimeWindow)) // JH
					online++;
			return online;
		}

		public override string GetPassword(string username, string answer)
		{
			if (!EnablePasswordRetrieval)
				throw new NotSupportedException("Password retrieval is not supported");
			N2.Security.Items.User u = Bridge.GetUser(username);
			if (u != null)
			{
				if (RequiresQuestionAndAnswer && !string.Equals(u.PasswordAnswer, answer, StringComparison.InvariantCultureIgnoreCase)) // JH
					throw new MembershipPasswordException("Invalid password answer");
				// Note: may throw exception for hashed passwords!
				return FromStoredPassword(u.Password); // JH
			}
			return null;
		}

		public override MembershipUser GetUser(string username, bool userIsOnline)
		{
			N2.Security.Items.User u = Bridge.GetUser(username);
			if (u != null)
			{
				if (userIsOnline)
				{
					u.LastActivityDate = DateTime.Now;
					Bridge.Save(u); // JH
				}
				return u.GetMembershipUser(this.Name);
			}
			return null;
		}

		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			N2.Security.Items.UserList userContainer = Bridge.GetUserContainer(false);
			if (userContainer == null)
				return null;

			int _userId = 0;

			if (providerUserKey is int)
			{
				_userId = (int)providerUserKey;
			}
			else if (providerUserKey is Guid)
			{
				/// http://forums.asp.net/t/1266765.aspx
				Guid guid = (Guid)providerUserKey;
				//extract an integer from the beginning of the Guid
				byte[] _bytes = guid.ToByteArray();
				_userId = ((int)_bytes[0]) | ((int)_bytes[1] << 8) | ((int)_bytes[2] << 16) | ((int)_bytes[3] << 24);
			}

			IList<ContentItem> users = Bridge.Finder
			  .Where.ID.Eq(_userId)
			  .And.Type.Eq(typeof(N2.Security.Items.User))
			  .And.Parent.Eq(userContainer)
			  .Select();

			foreach (N2.Security.Items.User u in users)
			{
				if (userIsOnline)
				{
					u.LastActivityDate = DateTime.Now; // JH
					Bridge.Save(u);
				}
				return u.GetMembershipUser(Name);
			}
			return null;
		}

		public override string GetUserNameByEmail(string email)
		{
			N2.Security.Items.UserList userContainer = Bridge.GetUserContainer(false);
			if (userContainer == null)
				return null;
			IList<ContentItem> users = Bridge.Finder
			  .Where.Detail("Email").Eq(email)
			  .And.Type.Eq(typeof(N2.Security.Items.User))
			  .And.Parent.Eq(userContainer)
			  .Select();
			foreach (N2.Security.Items.User u in users)
				return u.Name;
			return null;
		}

		public override string ResetPassword(string username, string answer)
		{
			if (!EnablePasswordReset)
				throw new NotSupportedException("Password reset is not supported");
			N2.Security.Items.User u = Bridge.GetUser(username);
			if (u != null)
			{
				// JH
				/* original implementation does not provide service as expected for ResetPassword.
				 * MSDN: Resets a user's password to a new, automatically generated password.
						u.IsLockedOut = false;
						Bridge.Save(u);
						return u.Password; 
				 */
				if (RequiresQuestionAndAnswer && !string.Equals(u.PasswordAnswer, answer))
					throw new MembershipPasswordException("Incorrect password answer");
				string newPassword = GenerateRandomPassword();
				u.Password = ToStoredPassword(newPassword);
				u.IsLockedOut = false;
				Bridge.Save(u);
				return newPassword;
			}
			return null;
		}

		public override bool UnlockUser(string userName)
		{
			N2.Security.Items.User u = Bridge.GetUser(userName);
			if (u == null)
				return false;
			u.IsLockedOut = false;
			Bridge.Save(u);
			return true;
		}

		public override void UpdateUser(MembershipUser user)
		{
			if (user == null) throw new ArgumentNullException("user");

			N2.Security.Items.User u = Bridge.GetUser(user.UserName);
			if (u != null)
			{
				u.UpdateFromMembershipUser(user); // JH: note that password remains unaffected
				Bridge.Save(u);
			}
			else
				throw new N2Exception("User '" + user.UserName + "' not found.");
		}

		public override bool ValidateUser(string username, string password)
		{
			N2.Security.Items.User u = Bridge.GetUser(username);
			if (u != null && u.Password == ToStoredPassword(password)) // JH
			{
				u.LastLoginDate = DateTime.Now; // JH
				Bridge.Save(u);
				return true;
			}
			return false;
		}
	}
}
