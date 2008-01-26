using System.Web.UI.WebControls;
using N2.Edit;

namespace N2.Parts.Web.UI.WebControls
{
	public class DropPoint : Panel
	{
		private readonly string zoneName;
		private readonly ContentItem item;
		private readonly CreationPosition position;
		private readonly string backImageUrl;

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

		protected override void OnInit(System.EventArgs e)
		{
			base.OnInit(e);

			if(position == CreationPosition.Before)
			{
				Page.ClientScript.RegisterArrayDeclaration(
					"dropPoints",
					string.Format("{{dropKey:'{0}',zone:'{1}',before:{2}}}",
					              ClientID, zoneName, item.ID));
			}
			else
			{
				Page.ClientScript.RegisterArrayDeclaration(
					"dropPoints",
					string.Format("{{dropKey:'{0}',zone:'{1}',below:{2}}}",
					              ClientID, zoneName, item.ID));
			}
		}
	}
}
