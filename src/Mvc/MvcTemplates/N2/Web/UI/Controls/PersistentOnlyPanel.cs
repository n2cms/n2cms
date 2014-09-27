using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using N2.Web.UI;

namespace N2.Edit.Web.UI.Controls
{
    public class PersistentOnlyPanel : PlaceHolder
    {
        CustomValidator cv = new CustomValidator
        {
            CssClass = "info",
            Text = "This is not supported for this type of item.",
            Display = ValidatorDisplay.Dynamic
        };
		private bool isInvalid;

        public string Text
        {
            get { return cv.Text; }
            set { cv.Text = value; }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            cv.Page = Page;
        }

		protected override void OnPreRender(EventArgs e)
		{
			var item = new SelectionUtility(this, Page.GetEngine()).SelectedItem;
			isInvalid = item == null || item.ID == 0;

			base.OnPreRender(e);
		}

        protected override void Render(HtmlTextWriter writer)
        {
            if (isInvalid)
            {
                cv.IsValid = false;
                cv.RenderControl(writer);
            }
            else
                base.Render(writer);
        }
    }
}
