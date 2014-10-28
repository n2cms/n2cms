using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using N2.Collections;
using N2.Edit.Web;
using N2.Integrity;
using N2.Security;
using N2.Web;

// ReSharper disable RedundantNameQualifier
namespace N2.Edit
{
    public partial class AvailableZones : EditUserControl
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Visible = CurrentItem != null && rptZones.Items.Count > 0;
        }

        public object DataSource
        {
            get { return rptZones.DataSource; }
            set { rptZones.DataSource = value; }
        }

        private N2.ContentItem currentItem;
        public N2.ContentItem CurrentItem
        {
            get { return currentItem; }
            set { currentItem = value; }
        }
    
        protected string GetNewDataItemUrl(object dataItem)
        {
            N2.Integrity.AvailableZoneAttribute a = (N2.Integrity.AvailableZoneAttribute)dataItem;

            Url newUrl = Engine.ManagementPaths.GetSelectNewItemUrl(CurrentItem, a.ZoneName);
            return newUrl.AppendQuery("returnUrl", Request.RawUrl);
        }

        protected int GetEditDataItemID(object dataItem)
        {
	        return ((ContentItem) dataItem).ID;
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

        protected string GetEditDataItemClass(object dataItem)
        {
            return GetClassName((ContentItem)dataItem);
        }

        private string GetClassName(ContentItem item)
        {
            var className = new StringBuilder();
	        var currentTime = N2.Utility.CurrentTime();
            if (!item.Published.HasValue || item.Published > currentTime)
                className.Append("unpublished ");
            else if (item.Published > currentTime.AddDays(-1))
                className.Append("day ");
            else if (item.Published > currentTime.AddDays(-7))
                className.Append("week ");
            else if (item.Published > currentTime.AddMonths(-1))
                className.Append("month ");

            if (item.Expires.HasValue && item.Expires <= currentTime)
                className.Append("expired ");

            if (!item.Visible)
                className.Append("invisible ");

            if (item.AlteredPermissions != Permission.None && item.AuthorizedRoles != null &&
                item.AuthorizedRoles.Count > 0)
                className.Append("locked ");

            return className.ToString();
        }

        protected string GetDeleteDataItemUrl(object dataItem)
        {
            Url deleteUrl = Engine.ManagementPaths.GetDeleteUrl((ContentItem)dataItem);
            return deleteUrl.AppendQuery("returnUrl", Request.RawUrl);
        }

        protected IList<ContentItem> GetItemsInZone(object dataItem)
        {
            var a = (N2.Integrity.AvailableZoneAttribute)dataItem;
            return CurrentItem.Children.FindParts(a.ZoneName)
                .Where(p => Engine.SecurityManager.IsAuthorized(p, Page.User))
                .ToList();
        }

        protected string GetZoneString(string key)
        {
            return Utility.GetGlobalResourceString("Zones", key);
        }

        protected bool CanMoveItemUp(object dataItem)
        {
            var item = (ContentItem)dataItem;

            return CurrentItem.GetChildPartsUnfiltered(item.ZoneName).WhereAccessible().IndexOf(item) > 0;
        }

        protected string MoveItemUpClass(object dataItem)
        {
            return CanMoveItemUp(dataItem) ? string.Empty : "disabled";
        }

        protected bool CanMoveItemDown(object dataItem)
        {
            var item = (ContentItem)dataItem;

			var siblings = CurrentItem.GetChildPartsUnfiltered(item.ZoneName).WhereAccessible();

            return siblings.IndexOf(item) < siblings.Count - 1;
        }

        protected string MoveItemDownClass(object dataItem)
        {
            return CanMoveItemDown(dataItem) ? string.Empty : "disabled";
        }

        protected void MoveItemUp(object sender, EventArgs e)
        {
            var image = (ImageButton)sender;

            var id = Int32.Parse(image.CommandArgument);

            var item = Engine.Persister.Get(id);
			var siblings = CurrentItem.GetChildPartsUnfiltered(item.ZoneName).WhereAccessible();
            var itemIndex = siblings.IndexOf(item);

            if (itemIndex == 0)
                return;

            var previousItem = siblings[itemIndex - 1];
            Engine.Resolve<ITreeSorter>().MoveTo(item, NodePosition.Before, previousItem);
            Response.Redirect(Request.Url.PathAndQuery);
        }

        protected void MoveItemDown(object sender, EventArgs e)
        {
            var image = (ImageButton)sender;

            var id = Int32.Parse(image.CommandArgument);

            var item = Engine.Persister.Get(id);
			var siblings = CurrentItem.GetChildPartsUnfiltered(item.ZoneName).WhereAccessible();
            var itemIndex = siblings.IndexOf(item);

            if (itemIndex >= siblings.Count - 1)
                return;

            ContentItem nextItem = siblings[itemIndex + 1];

            Engine.Resolve<ITreeSorter>().MoveTo(item, NodePosition.After, nextItem);
            Response.Redirect(Request.Url.PathAndQuery);
        }

        public void LoadZonesOf(Definitions.ItemDefinition definition, ContentItem contentItem)
        {
	        DataSource = definition.AvailableZones.Union(contentItem.Children.FindZoneNames().Where(zn => !string.IsNullOrEmpty(zn)).Select(zn => new AvailableZoneAttribute(zn, zn)));
            DataBind();
        }
    }
}
// ReSharper restore RedundantNameQualifier
