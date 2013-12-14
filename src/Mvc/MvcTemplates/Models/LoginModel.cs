using System;
using N2.Templates.Mvc.Models.Parts;
using N2.Web.Mvc;
using N2.Web.UI;

namespace N2.Templates.Mvc.Models
{
    public class LoginModel : IItemContainer<LoginItem>
    {
        public bool LoggedIn { get; set; }

        public LoginModel(LoginItem item)
        {
            CurrentItem = item;
        }

        /// <summary>Gets the item associated with the item container.</summary>
        ContentItem IItemContainer.CurrentItem
        {
            get { return CurrentItem; }
        }

        public LoginItem CurrentItem { get; private set; }
    }
}
