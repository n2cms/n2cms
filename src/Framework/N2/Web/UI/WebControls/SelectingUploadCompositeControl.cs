using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace N2.Web.UI.WebControls
{
    internal class SelectingUploadCompositeControl : Control, INamingContainer
    {
        public HtmlGenericControl SelectorContainer { get; set; }
        public HtmlGenericControl UploadContainer { get; set; }
        public FileSelector SelectorControl { get; set; }
        public Label UploadLabel { get; set; }
        public FileUpload UploadControl { get; set; }

        public SelectingUploadCompositeControl()
        {
            SelectorControl = new FileSelector();
            SelectorControl.ID = "Selector";
            UploadControl = new FileUpload();
            UploadControl.ID = "Uploader";
            UploadLabel = new Label();
            UploadLabel.AssociatedControlID = UploadControl.ID;
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            SelectorContainer = new HtmlGenericControl("span");
            SelectorContainer.Attributes["class"] = "uploadableContainer selector";
            Controls.Add(SelectorContainer);

            SelectorContainer.Controls.Add(SelectorControl);

            UploadContainer = new HtmlGenericControl("span");
            UploadContainer.Attributes["class"] = "uploadableContainer uploader";
            Controls.Add(UploadContainer);

            UploadContainer.Controls.Add(UploadLabel);
            UploadContainer.Controls.Add(UploadControl);
        }

        public void Select(string url)
        {
            EnsureChildControls();
            SelectorControl.Url = url;

            if (string.IsNullOrEmpty(url))
                SelectorContainer.Attributes["class"] = "uploadableContainer selector";
            else
                UploadContainer.Attributes["class"] = "uploadableContainer uploader";
        }
    }
}
