using System.Web;
using System.Web.UI;
using N2.Definitions;
using N2.Web.UI.WebControls;
using System;
using N2.Engine;

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
				ControlPanel.RegisterArrayValue(Page, "dragItems", array);
			}
			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
            IEngine e = N2.Context.Current;

			if (ControlPanel.GetState() == ControlPanelState.DragDrop)
			{
				writer.Write("<div id='");
				writer.Write(ClientID);
				writer.Write("' class='titleBar ");
				writer.Write(Definition.Discriminator);
                writer.Write("'>");

                WriteCommand(writer, "Edit part", "command edit", Url.Parse(e.EditManager.GetEditExistingItemUrl(CurrentItem)).AppendQuery("returnUrl", Page.Request.RawUrl));
                WriteCommand(writer, "Delete part", "command delete", Url.Parse(e.EditManager.GetDeleteUrl(CurrentItem)).AppendQuery("returnUrl", Page.Request.RawUrl));
                WriteTitle(writer);
				
                writer.Write("</div>");
			}
		}

        private void WriteCommand(HtmlTextWriter writer, string title, string @class, string url)
        {
            writer.Write("<a title='" + title + "' class='" + @class + "' href='");
            writer.Write(url);
            writer.Write("'></a>");
        }

        private void WriteTitle(HtmlTextWriter writer)
        {
            writer.Write("<span class='title' style='background-image:url(");
            writer.Write(Url.ToAbsolute(Definition.IconUrl));
            writer.Write(");'>");
            writer.Write(Definition.Title);
            writer.Write("</span>");
        }
	}
}
