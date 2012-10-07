using System.Collections.Generic;
using System.Collections.Specialized;
using N2.Edit;
using N2.Engine;
using N2.Persistence;
using N2.Edit.Versioning;
using System;

namespace N2.Web.Parts
{
	[Service(typeof(IAjaxService))]
	public class ItemCopyer : PartsAjaxService
	{
		readonly Navigator navigator;
        readonly IPersister persister;
		readonly Integrity.IIntegrityManager integrity;
		readonly IVersionManager versions;
		readonly ContentVersionRepository versionRepository;

		public ItemCopyer(IPersister persister, Navigator navigator, Integrity.IIntegrityManager integrity, IVersionManager versions, ContentVersionRepository versionRepository)
		{
            this.persister = persister;
			this.navigator = navigator;
			this.integrity = integrity;
			this.versions = versions;
			this.versionRepository = versionRepository;
		}

		public override string Name
		{
			get { return "copy"; }
		}

		public override NameValueCollection HandleRequest(NameValueCollection request)
		{

			var response = new NameValueCollection();
			response["redirect"] = CopyItem(request);
			return response;
		}

		private string CopyItem(NameValueCollection request)
		{
			var path = PartsExtensions.EnsureDraft(versions, versionRepository, navigator, request);
			ContentItem item = path.CurrentItem;
			ContentItem page = path.CurrentPage;
			
			item = item.Clone(true);
            item.Name = null;
			item.ZoneName = request["zone"];
			item.SetVersionKey(Guid.NewGuid().ToString());

			var beforeItem = PartsExtensions.GetBeforeItem(navigator, request, page);
			ContentItem parent;
			if (beforeItem != null)
			{
				parent = beforeItem.Parent;
				int newIndex = parent.Children.IndexOf(beforeItem);
				ValidateLocation(item, parent);
				Utility.Insert(item, parent, newIndex);
			}
			else
			{
				parent = PartsExtensions.GetBelowItem(navigator, request, page);
				ValidateLocation(item, parent);
				Utility.Insert(item, parent, parent.Children.Count);
			}

			Utility.UpdateSortOrder(parent.Children);
			versionRepository.Save(page);

			return page.Url.ToUrl().SetQueryParameter("edit", "drag");

			//string before = request["before"];
			//string below = request["below"];

			//if (!string.IsNullOrEmpty(before))
			//{
			//    ContentItem beforeItem = navigator.Navigate(before);
			//    parent = beforeItem.Parent;
			//    int newIndex = parent.Children.IndexOf(beforeItem);
			//    Utility.Insert(item, parent, newIndex);
			//}
			//else
			//{
			//    parent = navigator.Navigate(below);
			//    Utility.Insert(item, parent, parent.Children.Count);
			//}

			//persister.Save(item);

			//IEnumerable<ContentItem> changedItems = Utility.UpdateSortOrder(parent.Children);
			//foreach (ContentItem changedItem in changedItems)
			//    persister.Save(changedItem);
		}

		private void ValidateLocation(ContentItem item, ContentItem parent)
		{
			var ex = integrity.GetCopyException(item, parent);
			if (ex != null)
				throw ex;
		}
	}
}