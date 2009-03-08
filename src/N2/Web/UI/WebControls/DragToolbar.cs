using System.Web;
using System.Web.UI;
using N2.Definitions;
using N2.Web.UI.WebControls;
using System;

namespace N2.Web.UI.WebControls
{
	public class DraggableToolbar : Control
	{
		private ContentItem currentItem;
		private ItemDefinition definition;
		private bool bindButtons = false;

		public DraggableToolbar()
		{
			ID = "toolbar";
			bindButtons = true;
		}

		public DraggableToolbar(ContentItem item, ItemDefinition definition)
		{
            if (definition == null) throw new ArgumentNullException("definition");

			this.currentItem = item;
			this.definition = definition;
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

		protected override void OnPreRender(EventArgs e)
		{
			if (bindButtons && ControlPanel.GetState() == ControlPanelState.DragDrop)
			{
				if (string.IsNullOrEmpty(ID))
					ID = "t" + CurrentItem.ID;
			    string array = string.Format("{{dragKey:'{0}',item:{1}}}", ClientID, CurrentItem.ID);
			    Page.ClientScript.RegisterArrayDeclaration("dragItems", array);
			}
			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (ControlPanel.GetState() == ControlPanelState.DragDrop)
			{
				writer.Write("<div id='");
				writer.Write(ClientID);
				writer.Write("' class='titleBar ");
				writer.Write(Definition.Discriminator);
				writer.Write("' style='background-image:url(");
                writer.Write(Url.ToAbsolute(Definition.IconUrl));
                writer.Write(");'>");
			    writer.Write("<img src='");
                writer.Write(Url.ToAbsolute("~/Edit/img/ico/png/delete.png"));
                writer.Write("' class='delete' alt='delete'");
				if(bindButtons)
				{
				    writer.Write(" onclick=\"n2ddcp.del('");
                    writer.Write(ClientID);
                    writer.Write("');\"");
				}
				writer.Write("/>");
			    writer.Write("<img src='");
                writer.Write(Url.ToAbsolute("~/Edit/img/ico/png/pencil.png"));
                writer.Write("' class='edit' alt='edit'");
				if (bindButtons)
				{
				    writer.Write(" onclick=\"n2ddcp.edit('");
                    writer.Write(ClientID);
                    writer.Write("');\"");
				}
				writer.Write("/>");
				writer.Write(Definition.Title);
				writer.Write("</div>");
			}
		}
	}
}
