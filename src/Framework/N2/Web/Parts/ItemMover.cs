using System.Collections.Generic;
using System.Collections.Specialized;
using N2.Edit;
using N2.Engine;
using N2.Persistence;
using N2.Integrity;
using N2.Edit.Versioning;

namespace N2.Web.Parts
{
	[Service(typeof(IAjaxService))]
	public class ItemMover : PartsAjaxService
    {
        private readonly Navigator navigator;
        private readonly IPersister persister;
		private IIntegrityManager integrity;
		private IVersionManager versions;
		private ContentVersionRepository versionRepository;

        public ItemMover(IPersister persister, Navigator navigator, IIntegrityManager integrity, IVersionManager versions, ContentVersionRepository versionRepository)
        {
            this.persister = persister;
            this.navigator = navigator;
			this.integrity = integrity;
			this.versions = versions;
			this.versionRepository = versionRepository;
        }

        public override string Name
        {
            get { return "move"; }
        }

        public override NameValueCollection HandleRequest(NameValueCollection request)
        {
            MoveItem(request);
            return new NameValueCollection();
        }

        private void MoveItem(NameValueCollection request)
        {
            ContentItem item = navigator.Navigate(request["item"]);
			item = versionRepository.ParseVersion(request[PathData.VersionQueryKey], request["versionKey"], item)
				?? item;

			var page = Find.ClosestPage(item);
			if (!page.VersionOf.HasValue)
			{
				page = versions.AddVersion(page, asPreviousVersion: false);
				item = page.FindPartVersion(item);
			}

            ContentItem parent;

            item.ZoneName = request["zone"];
            string before = request["before"];
			string beforeVersionKey = request["beforeVersionKey"];
			string below = request["below"];
			string belowVersionKey = request["belowVersionKey"];

            if (!string.IsNullOrEmpty(before))
            {
                ContentItem beforeItem = navigator.Navigate(before);
				beforeItem = page.FindPartVersion(beforeItem);
				parent = MoveBefore(item, beforeItem);
            }
			else if (!string.IsNullOrEmpty(beforeVersionKey))
			{
				var beforeItem = page.FindDescendantByVersionKey(beforeVersionKey);
				parent = MoveBefore(item, beforeItem);
			}
			else if (!string.IsNullOrEmpty(belowVersionKey))
			{
				parent = page.FindDescendantByVersionKey(belowVersionKey);
				ValidateLocation(item, parent);
				Utility.Insert(item, parent, parent.Children.Count);
			}
			else
			{
				parent = navigator.Navigate(below);
				parent = page.FindPartVersion(parent);
				ValidateLocation(item, parent);
				Utility.Insert(item, parent, parent.Children.Count);
			}

			Utility.UpdateSortOrder(parent.Children);
			versionRepository.Save(page);
        }

		private ContentItem MoveBefore(ContentItem item, ContentItem beforeItem)
		{
			ContentItem parent;
			parent = beforeItem.Parent;
			int newIndex = parent.Children.IndexOf(beforeItem);
			ValidateLocation(item, parent);
			Utility.Insert(item, parent, newIndex);
			return parent;
		}

		private void ValidateLocation(ContentItem item, ContentItem parent)
		{
			var ex = integrity.GetMoveException(item, parent);
			if (ex != null)
				throw ex;
		}
    }
}