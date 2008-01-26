using System;
using System.Web.Security;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Security.Items
{
	[Definition("User", "User")]
	[RestrictParents(typeof (UserList))]
	[WithEditableName("Username", 10)]
	[WithEditableTitle("First and last name", 100)]
	public class User : AbstractItem
	{
		[EditableTextBox("Password", 20)]
		public virtual string Password
		{
			get { return (string) (GetDetail("Password") ?? string.Empty); }
			set { SetDetail("Password", value, string.Empty); }
		}

		[EditableTextBox("Email", 110)]
		public virtual string Email
		{
			get { return (string) (GetDetail("Email") ?? string.Empty); }
			set { SetDetail("Email", value, string.Empty); }
		}

		[EditableTextBox("PasswordQuestion", 120)]
		public virtual string PasswordQuestion
		{
			get { return (string) (GetDetail("PasswordQuestion") ?? string.Empty); }
			set { SetDetail("PasswordQuestion", value, string.Empty); }
		}

		[EditableTextBox("PasswordAnswer", 130)]
		public virtual string PasswordAnswer
		{
			get { return (string) (GetDetail("PasswordAnswer") ?? string.Empty); }
			set { SetDetail("PasswordAnswer", value, string.Empty); }
		}

		[EditableCheckBox("IsOnline", 140)]
		public virtual bool IsOnline
		{
			get { return (bool) (GetDetail("IsOnline") ?? false); }
			set { SetDetail("IsOnline", value, false); }
		}

		[EditableCheckBox("IsApproved", 140)]
		public virtual bool IsApproved
		{
			get { return (bool) (GetDetail("IsApproved") ?? false); }
			set { SetDetail("IsApproved", value, false); }
		}

		[EditableCheckBox("IsLockedOut", 140)]
		public virtual bool IsLockedOut
		{
			get { return (bool) (GetDetail("IsLockedOut") ?? false); }
			set { SetDetail("IsLockedOut", value, false); }
		}

		public virtual object ProviderUserKey
		{
			get { return GetDetail("ProviderUserKey"); }
			set { SetDetail("ProviderUserKey", value); }
		}


		[EditableTextBox("Comment", 100)]
		public virtual string Comment
		{
			get { return (string) (GetDetail("Comment") ?? string.Empty); }
			set { SetDetail("Comment", value, string.Empty); }
		}

		[EditableTextBox("LastLoginDate", 100)]
		public virtual DateTime LastLoginDate
		{
			get { return (DateTime) (GetDetail("LastLoginDate") ?? Published.Value); }
			set { SetDetail("LastLoginDate", value, Published.Value); }
		}

		[EditableTextBox("LastActivityDate", 100)]
		public virtual DateTime LastActivityDate
		{
			get { return (DateTime) (GetDetail("LastActivityDate") ?? Published.Value); }
			set { SetDetail("LastActivityDate", value, Published.Value); }
		}

		[EditableTextBox("LastPasswordChangedDate", 100)]
		public virtual DateTime LastPasswordChangedDate
		{
			get { return (DateTime) (GetDetail("LastPasswordChangedDate") ?? Published.Value); }
			set { SetDetail("LastPasswordChangedDate", value, Published.Value); }
		}

		[EditableTextBox("LastLockoutDate", 100)]
		public virtual DateTime? LastLockoutDate
		{
			get { return (DateTime?) GetDetail("LastLockoutDate"); }
			set { SetDetail("LastLockoutDate", value < new DateTime(2000, 1, 1) ? null : value); }
		}

		public virtual DetailCollection Roles
		{
			get { return GetDetailCollection("Roles", true); }
		}

		public override string IconUrl
		{
			get { return "~/Edit/Img/Ico/Png/user.png"; }
		}

		public virtual MembershipUser GetMembershipUser(string providerName)
		{
			return
				new MembershipUser(providerName, Name, ProviderUserKey, Email, PasswordQuestion, Comment, IsApproved, IsLockedOut,
				                   Created, LastLoginDate, LastActivityDate, LastPasswordChangedDate,
				                   (LastLockoutDate ?? DateTime.MinValue));
		}

		public virtual void UpdateFromMembershipUser(MembershipUser mu)
		{
			Comment = mu.Comment;
			Created = mu.CreationDate;
			Email = mu.Email;
			IsApproved = mu.IsApproved;
			IsLockedOut = mu.IsLockedOut;
			IsOnline = mu.IsOnline;
			LastActivityDate = mu.LastActivityDate;
			LastLockoutDate = mu.LastLockoutDate;
			LastLoginDate = mu.LastLoginDate;
			LastPasswordChangedDate = mu.LastPasswordChangedDate;
			PasswordQuestion = mu.PasswordQuestion;
			ProviderUserKey = mu.ProviderUserKey;
			Name = mu.UserName;
		}

		public virtual string[] GetRoles()
		{
			string[] roles = new string[Roles.Count];
			for (int i = 0; i < roles.Length; i++)
			{
				roles[i] = Roles[i] as string;
			}
			return roles;
		}
	}
}