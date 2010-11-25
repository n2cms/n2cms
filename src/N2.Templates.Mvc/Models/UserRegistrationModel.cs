using System;
using System.ComponentModel;
using N2.Templates.Mvc.Models.Parts;
using N2.Web.Mvc;
using N2.Web.UI;

namespace N2.Templates.Mvc.Models
{
	public class UserRegistrationModel : IItemContainer<UserRegistration>, IDataErrorInfo
	{
		public UserRegistrationModel()
		{
		}

		public UserRegistrationModel(UserRegistration userRegistration)
		{
			CurrentItem = userRegistration;
		}

		public string RegisterUserName { get; set; }

		public string RegisterPassword { get; set; }

		public string RegisterConfirmPassword { get; set; }

		public string RegisterEmail { get; set; }

		#region IItemContainer<UserRegistration> Members

		/// <summary>Gets the item associated with the item container.</summary>
		ContentItem IItemContainer.CurrentItem
		{
			get { return CurrentItem; }
		}

		public UserRegistration CurrentItem { get; set; }

		#endregion

		#region IDataErrorInfo Members

		public string this[string columnName]
		{
			get { return Validate(columnName); }
		}

		public string Error
		{
			get { return String.Empty; }
		}

		#endregion

		private string Validate(string propertyName)
		{
			switch (propertyName.ToLower())
			{
				case "username":
					if (String.IsNullOrEmpty(RegisterUserName))
						return "User Name cannot be empty";
					break;
				case "password":
					if (String.IsNullOrEmpty(RegisterPassword))
						return "Password cannot be empty";
					break;
				case "email":
					if (String.IsNullOrEmpty(RegisterEmail))
						return "Email cannot be empty";
					break;
				case "confirmpassword":
					if (RegisterConfirmPassword != RegisterPassword)
						return "Passwords do not match";
					break;
			}
			return String.Empty;
		}
        }
}