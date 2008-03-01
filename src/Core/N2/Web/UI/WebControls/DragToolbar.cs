using System.Web;
using System.Web.UI;
using N2.Definitions;
using N2.Web.UI.WebControls;

[assembly: WebResource("N2.Resources.delete.png", "image/png")]
[assembly: WebResource("N2.Resources.page_edit.png", "image/png")]

namespace N2.Web.UI.WebControls
{
	public class DraggableToolbar : Control
	{
		private ContentItem currentItem;
		private ItemDefinition definition;
		private readonly string gripperImageUrl;
		private bool bindButtons = false;

		public DraggableToolbar()
		{
			bindButtons = true;
		}

		public DraggableToolbar(ContentItem item, ItemDefinition definition, string gripperImageUrl)
		{
			this.currentItem = item;
			this.definition = definition;
			this.gripperImageUrl = gripperImageUrl;
		}

		public ContentItem CurrentItem
		{
			get { return currentItem ?? (currentItem = Find.CurrentPage); }
			set { currentItem = value; }
		}

		public ItemDefinition Definition
		{
			get { return definition ?? (definition = N2.Context.Definitions.GetDefinition(CurrentItem.GetType())); }
		}

		protected override void OnPreRender(System.EventArgs e)
		{
			if (bindButtons && ControlPanel.GetState() == ControlPanelState.DragDrop)
			{
				if (string.IsNullOrEmpty(ID))
					ID = "t" + CurrentItem.ID;
				Page.ClientScript.RegisterArrayDeclaration(
					"dragItems",
					string.Format("{{dragKey:'{0}',item:{1}}}", ClientID, CurrentItem.ID));
			}
			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (ControlPanel.GetState() == ControlPanelState.DragDrop)
			{
				writer.Write("<div class='titleBar'");
				if (!string.IsNullOrEmpty(gripperImageUrl))
				{
					writer.Write(" style='background-image:url({0});'", gripperImageUrl);
				}
				writer.Write(">");
				writer.Write("<img src='{0}' class='delete' alt='delete'", GetDeleteIcon());
				if(bindButtons)
				{
					writer.Write(" onclick=\"n2ddcp.del('{0}');\"", ClientID);
				}
				writer.Write("/>");
				writer.Write("<img src='{0}' class='edit' alt='edit'", GetEditIcon());
				if (bindButtons)
				{
					writer.Write(" onclick=\"n2ddcp.edit('{0}');\"", ClientID);
				}
				writer.Write("/>");
				writer.Write(Definition.Title);
				writer.Write("</div>");
			}
		}

		private string GetEditIcon()
		{
			return Page.ClientScript.GetWebResourceUrl(typeof (DraggableToolbar), "N2.Resources.page_edit.png");
		}

		private string GetDeleteIcon()
		{
			return Page.ClientScript.GetWebResourceUrl(typeof (DraggableToolbar), "N2.Resources.delete.png");
		}
	}
}
