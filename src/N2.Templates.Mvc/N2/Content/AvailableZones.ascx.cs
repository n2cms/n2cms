using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Web;

namespace N2.Edit
{
    public partial class AvailableZones : EditUserControl
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
			this.Visible = this.CurrentItem != null && this.CurrentItem.ID>0 && this.rptZones.Items.Count > 0;
        }

        public object DataSource
        {
            get { return this.rptZones.DataSource; }
            set { this.rptZones.DataSource = value; }
        }

        private N2.ContentItem currentItem;
        public N2.ContentItem CurrentItem
        {
            get { return currentItem; }
            set { currentItem = value; }
        }
		protected string GetNewDataItemText(object dataItem)
		{
			return string.Format((string)GetLocalResourceObject("hlNew.TextFormat"),
				DataBinder.Eval(dataItem, "Title"));
		}
	
        protected string GetNewDataItemUrl(object dataItem)
        {
            N2.Integrity.AvailableZoneAttribute a = (N2.Integrity.AvailableZoneAttribute)dataItem;

            Url newUrl = Engine.ManagementPaths.GetSelectNewItemUrl(CurrentItem, a.ZoneName);
            return newUrl.AppendQuery("returnUrl", Request.RawUrl);
		}

		protected int GetEditDataItemID(object dataItem)
		{
			ContentItem item = (ContentItem)dataItem;
			return item.ID;
		}

		protected string GetEditDataItemText(object dataItem)
		{
			ContentItem item = (ContentItem)dataItem;
			return string.Format("<img src='{0}'>{1}", N2.Web.Url.ToAbsolute(item.IconUrl), string.IsNullOrEmpty(item.Title) ? "(untitled)" : item.Title);
        }

        protected string GetEditDataItemUrl(object dataItem)
        {
            Url editUrl = Engine.ManagementPaths.GetEditExistingItemUrl((ContentItem)dataItem);
            return editUrl.AppendQuery("returnUrl", Request.RawUrl);
        }

        protected string GetDeleteDataItemUrl(object dataItem)
        {
            Url deleteUrl = Engine.ManagementPaths.GetDeleteUrl((ContentItem)dataItem);
            return deleteUrl.AppendQuery("returnUrl", Request.RawUrl);
        }

        protected IList<ContentItem> GetItemsInZone(object dataItem)
        {
            N2.Integrity.AvailableZoneAttribute a = (N2.Integrity.AvailableZoneAttribute)dataItem;
            return CurrentItem.GetChildren(a.ZoneName);
        }

		protected string GetZoneString(string key)
		{
			return Utility.GetGlobalResourceString("Zones", key);
		}

		protected bool CanMoveItemUp(object dataItem)
		{
			var item = (ContentItem)dataItem;

			return CurrentItem.GetChildren(item.ZoneName).IndexOf(item) > 0;
		}

		protected string MoveItemUpClass(object dataItem)
		{
			return CanMoveItemUp(dataItem) ? "" : "disabled";
		}

		protected bool CanMoveItemDown(object dataItem)
		{
			var item = (ContentItem)dataItem;

			var siblings = CurrentItem.GetChildren(item.ZoneName);

			return siblings.IndexOf(item) < siblings.Count - 1;
		}

		protected string MoveItemDownClass(object dataItem)
		{
			return CanMoveItemDown(dataItem) ? "" : "disabled";
		}

		protected void MoveItemUp(object sender, EventArgs e)
		{
			var image = (ImageButton)sender;

			var id = Int32.Parse(image.CommandArgument);

			var item = Engine.Persister.Get(id);
			var siblings = CurrentItem.GetChildren(item.ZoneName);
			var itemIndex = siblings.IndexOf(item);

			if(itemIndex == 0)
				return;

			ContentItem previousItem = siblings[itemIndex - 1];

			Engine.Resolve<ITreeSorter>().MoveTo(item, NodePosition.Before, previousItem);
			Response.Redirect(Request.Url.PathAndQuery);
		}

		protected void MoveItemDown(object sender, EventArgs e)
		{
			var image = (ImageButton)sender;

			var id = Int32.Parse(image.CommandArgument);

			var item = Engine.Persister.Get(id);
			var siblings = CurrentItem.GetChildren(item.ZoneName);
			var itemIndex = siblings.IndexOf(item);

			if (itemIndex >= siblings.Count - 1)
				return;

			ContentItem nextItem = siblings[itemIndex + 1];

			Engine.Resolve<ITreeSorter>().MoveTo(item, NodePosition.After, nextItem);
			Response.Redirect(Request.Url.PathAndQuery);
		}
    }
}
