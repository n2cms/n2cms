using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Addons.UITests.UI
{
    public partial class EditableUc2 : System.Web.UI.UserControl, N2.Edit.IContentBinder
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        #region IBinder<ContentItem> Members

        public bool UpdateObject(ContentItem value)
        {
            string current = value["DetailName"] as string;
            if (current != txtDate.Text)
            {
                value["DetailName"] = txtDate.Text;
                return true;
            }
            return false;
        }

        public void UpdateInterface(ContentItem value)
        {
            txtDate.Text = value["DetailName"] as string;
        }

        #endregion
    }
}
