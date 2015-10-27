using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Web.UI.WebControls
{
    internal class SelectingMediaControl : Control, INamingContainer
    {
        public HtmlGenericControl SelectorContainer { get; set; }
        public HtmlGenericControl UploadContainer { get; set; }
        public MediaSelector SelectorControl { get; set; }

        public SelectingMediaControl()
        {
            SelectorControl = new MediaSelector();
            SelectorControl.ID = "Selector";
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
