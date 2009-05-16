using System.Web.UI.WebControls;
using N2.Edit;

namespace N2.Web.UI.WebControls
{
	public class DropPoint : Panel
	{
		private readonly string zoneName;
		private readonly ContentItem item;
		private readonly CreationPosition position;
        private string text;
		
		public DropPoint(string zoneName, ContentItem item, CreationPosition position, string backImageUrl)
		{
			this.zoneName = zoneName;
			this.item = item;
			this.position = position;
			BackImageUrl = backImageUrl;

			CssClass = "dropPoint " + zoneName;
			if (position == CreationPosition.Below)
				ID = "dp_" + item.ID + "_" + zoneName;
			else
				ID = "dp_" + item.ID;
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

		protected override void OnInit(System.EventArgs e)
		{
			base.OnInit(e);

			if(position == CreationPosition.Before)
			{
				ControlPanel.RegisterArrayValue(Page, "dropPoints",
					string.Format("{{dropKey:'{0}',zone:'{1}',before:{2}}}", ClientID, zoneName, item.ID));
			}
			else
			{
				ControlPanel.RegisterArrayValue(Page, "dropPoints",
					string.Format("{{dropKey:'{0}',zone:'{1}',below:{2}}}", ClientID, zoneName, item.ID));
			}
		}

        protected override void CreateChildControls()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                Label l = new Label();
                l.Text = Text;
                l.CssClass = "dpLabel";
                this.Controls.AddAt(0, l);
            }
            base.CreateChildControls();
        }
	}
}
