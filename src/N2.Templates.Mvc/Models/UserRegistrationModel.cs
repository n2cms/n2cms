using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using MvcContrib.FluentHtml;
using MvcContrib.FluentHtml.Behaviors;
using N2.Templates.Mvc.Items.Items;
using N2.Web.Mvc;
using N2.Web.UI;

namespace N2.Templates.Mvc.Models
{
	public class UserRegistrationModel : IItemContainer<UserRegistration>, IViewModelContainer<UserRegistrationModel>, IDataErrorInfo
	{
		public UserRegistrationModel()
		{
		}

		public UserRegistrationModel(UserRegistration userRegistration)
		{
			CurrentItem = userRegistration;
		}

		public string UserName { get; set; }

		public string Password { get; set; }

		public string ConfirmPassword { get; set; }

		public string Email { get; set; }

		#region IItemContainer<UserRegistration> Members

		/// <summary>Gets the item associated with the item container.</summary>
		ContentItem IItemContainer.CurrentItem
		{
			get { return CurrentItem; }
		}

		public UserRegistration CurrentItem { get; set; }

		#endregion

		#region IViewModelContainer<UserRegistrationModel> Members

		public UserRegistrationModel ViewModel
		{
			get { return this; }
		}

		public IEnumerable<IBehaviorMarker> Behaviors
		{
			get { return new IBehaviorMarker[0]; }
		}

		public string HtmlNamePrefix { get; set; }

		public ViewDataDictionary ViewData
		{
			get { return new ViewDataDictionary(this); }
			set { }
		}

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
					if (String.IsNullOrEmpty(UserName))
						return "User Name cannot be empty";
					break;
				case "password":
					if (String.IsNullOrEmpty(Password))
						return "Password cannot be empty";
					break;
				case "email":
					if (String.IsNullOrEmpty(Email))
						return "Email cannot be empty";
					break;
				case "confirmpassword":
					if (ConfirmPassword != Password)
						return "Passwords do not match";
					break;
			}
			return String.Empty;
		}

        #region IViewModelContainer<UserRegistrationModel> Members

        public HtmlHelper Html
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}