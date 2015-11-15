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
			SelectorContainer = new HtmlGenericControl("div");
			SelectorControl = new MediaSelector();
            UploadContainer = new HtmlGenericControl("span");
		}

		protected override void CreateChildControls()
        {
			SelectorControl.ID = ID + "_selector";

			base.CreateChildControls();

            SelectorContainer.Attributes["class"] = "uploadableContainer selector";
            Controls.Add(SelectorContainer);

            SelectorContainer.Controls.Add(SelectorControl);

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
