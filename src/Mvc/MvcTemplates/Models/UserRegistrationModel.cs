using System;
using System.ComponentModel;
using N2.Templates.Mvc.Models.Parts;
using N2.Web.Mvc;
using N2.Web.UI;
using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage = "User Name is required")]
        public string RegisterUserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string RegisterPassword { get; set; }
        
        [Required(ErrorMessage = "Confirm Password is required")]
        public string RegisterConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email is required"), RegularExpression("[^@]+@[^.]+(\\.[^.]+)+", ErrorMessage = "Invalid email")]
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
            if (propertyName.ToLower() == "confirmpassword")
            {
                if (RegisterConfirmPassword != RegisterPassword)
                    return "Passwords do not match";
            }
            return String.Empty;
        }
        }
}
