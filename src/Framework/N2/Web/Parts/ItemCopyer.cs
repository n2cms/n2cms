using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using N2.Persistence;
using N2.Web;
using N2.Edit;
using N2.Engine;

namespace N2.Web.Parts
{
	[Service(typeof(IAjaxService))]
	public class ItemCopyer : PartsAjaxService
	{
		readonly Navigator navigator;
        readonly IPersister persister;

        public ItemCopyer(IPersister persister, Navigator navigator)
		{
            this.persister = persister;
			this.navigator = navigator;
		}

		public override string Name
		{
			get { return "copy"; }
		}

		public override NameValueCollection HandleRequest(NameValueCollection request)
		{
			CopyItem(request);
			return new NameValueCollection();
		}

		private void CopyItem(NameValueCollection request)
		{
            ContentItem item = navigator.Navigate(request["item"]);
			ContentItem parent;

			item = item.Clone(true);
            item.Name = null;
			item.ZoneName = request["zone"];

			string before = request["before"];
			string below = request["below"];

            if (!string.IsNullOrEmpty(before))
			{
                ContentItem beforeItem = navigator.Navigate(before);
				parent = beforeItem.Parent;
				int newIndex = parent.Children.IndexOf(beforeItem);
				Utility.Insert(item, parent, newIndex);
			}
            else
            {
                parent = navigator.Navigate(below);
                Utility.Insert(item, parent, parent.Children.Count);
            }

            persister.Save(item);

			IEnumerable<ContentItem> changedItems = Utility.UpdateSortOrder(parent.Children);
			foreach (ContentItem changedItem in changedItems)
				persister.Save(changedItem);
		}
	}
}