using System.Web;
using System.Web.UI;
using N2.Definitions;
using N2.Web.UI.WebControls;
using System;
using N2.Engine;
using System.IO;
using N2.Edit;

namespace N2.Web.UI.WebControls
{
	public class DraggableToolbar : Control
	{
		private ContentItem currentItem;
		private ItemDefinition definition;

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

        //protected override void OnPreRender(EventArgs e)
        //{
        //    if (bindButtons && ControlPanel.GetState(Page.User, Page.Request.QueryString) == ControlPanelState.DragDrop)
        //    {
        //        if (string.IsNullOrEmpty(ID))
        //            ID = "t" + CurrentItem.ID;
        //        string array = string.Format("{{dragKey:'{0}',item:{1}}}", ClientID, CurrentItem.ID);
        //        ControlPanel.RegisterArrayValue(Page, "dragItems", array);
        //    }
        //    base.OnPreRender(e);
        //}

		protected override void Render(HtmlTextWriter writer)
		{
            IEngine e = N2.Context.Current;

            if (ControlPanel.GetState(Page.User, Page.Request.QueryString) == ControlPanelState.DragDrop)
			{
                WriteTitleBar(writer, e.EditManager, Definition, CurrentItem);
			}
		}

        public static void WriteTitleBar(TextWriter writer, IEditManager editManager, ItemDefinition definition, ContentItem item)
        {
            writer.Write("<div class='titleBar ");
            writer.Write(definition.Discriminator);
            writer.Write("'>");

            var returnUrl = Url.Parse(editManager.GetPreviewUrl(item)).AppendQuery("edit", "drag");
            WriteCommand(writer, "Edit part", "command edit", Url.Parse(editManager.GetEditExistingItemUrl(item)).AppendQuery("returnUrl", returnUrl).Encode());
            WriteCommand(writer, "Delete part", "command delete", Url.Parse(editManager.GetDeleteUrl(item)).AppendQuery("returnUrl", returnUrl).Encode());
            WriteTitle(writer, definition);

            writer.Write("</div>");
        }

        protected static void WriteCommand(TextWriter writer, string title, string @class, string url)
        {
            writer.Write("<a title='" + title + "' class='" + @class + "' href='");
            writer.Write(url);
            writer.Write("'></a>");
        }

        protected static void WriteTitle(TextWriter writer, ItemDefinition definition)
        {
            writer.Write("<span class='title' style='background-image:url(");
            writer.Write(Url.ToAbsolute(definition.IconUrl));
            writer.Write(");'>");
            writer.Write(definition.Title);
            writer.Write("</span>");
        }
	}
}
