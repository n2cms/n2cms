using N2.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace N2.Security
{
    public class EditablePasswordAttribute : EditableTextAttribute
    {
        public EditablePasswordAttribute(string title, int sortOrder)
            : base (title, sortOrder)
        {
            this.TextMode = TextBoxMode.Password;
        }

        public override void UpdateEditor(ContentItem item, System.Web.UI.Control editor)
        {
        }

        public override bool UpdateItem(ContentItem item, System.Web.UI.Control editor)
        {
            var password = ((TextBox)editor).Text;
            if (string.IsNullOrEmpty(password))
                return false;

            var membershpiProvider = Membership.Provider as ContentMembershipProvider;
            if (membershpiProvider != null)
            {
                password = membershpiProvider.ToStoredPassword(password);
            }

            item[Name] = password;
            return true;
        }
    }
}
