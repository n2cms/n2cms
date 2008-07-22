using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Edit
{
    public partial class AvailableZones : System.Web.UI.UserControl
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

			return N2.Context.Current.EditManager.GetSelectNewItemUrl(CurrentItem, a.ZoneName);
        }

        protected string GetEditDataItemText(object dataItem)
        {
            ContentItem item = (ContentItem)dataItem;
            return string.Format("<img src='{0}'>{1}", N2.Web.Url.ToAbsolute(item.IconUrl), string.IsNullOrEmpty(item.Title) ? "(untitled)" : item.Title);
        }
        protected string GetEditDataItemUrl(object dataItem)
        {
            return N2.Context.Current.EditManager.GetEditExistingItemUrl((ContentItem)dataItem);
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
    }
}