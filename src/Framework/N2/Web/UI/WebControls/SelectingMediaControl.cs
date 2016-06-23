using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Web.UI.WebControls
{
    internal class SelectingMediaControl : Control
    {
		public HtmlGenericControl SelectorContainer { get; set; }
        public MediaSelector SelectorControl { get; set; }

        public SelectingMediaControl()
        {
			SelectorContainer = new HtmlGenericControl("div");
			SelectorControl = new MediaSelector();
		}

		public SelectingMediaControl(string name)
		{
			this.ID = name;
			SelectorContainer = new HtmlGenericControl("div");
			SelectorControl = new MediaSelector(name);
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			EnsureChildControls();
		}

		protected override void CreateChildControls()
        {
			base.CreateChildControls();

            SelectorContainer.Attributes["class"] = "uploadableContainer selector";
            Controls.Add(SelectorContainer);

            SelectorContainer.Controls.Add(SelectorControl);
        }

        public void Select(string url)
        {
            EnsureChildControls();
            SelectorControl.Url = url;

            if (string.IsNullOrEmpty(url))
                SelectorContainer.Attributes["class"] = "uploadableContainer selector";
        }
    }
}
