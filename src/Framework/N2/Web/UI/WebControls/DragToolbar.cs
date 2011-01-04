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

		public DraggableToolbar()
		{
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

		[Obsolete("Don't use", true)]
		public ItemDefinition Definition
		{
			get { return definition ?? (definition = N2.Context.Definitions.GetDefinition(CurrentItem.GetContentType())); }
		}

		protected override void Render(HtmlTextWriter writer)
		{
			IEngine e = N2.Context.Current;

			if (ControlPanel.GetState(Page.User, Page.Request.QueryString) == ControlPanelState.DragDrop)
			{
				e.Resolve<PartUtilities>().WriteTitleBar(writer, CurrentItem, Page.Request.RawUrl);
			}
		}
	}
}