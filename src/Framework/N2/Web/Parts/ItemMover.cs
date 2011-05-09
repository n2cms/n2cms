using System.Collections.Generic;
using System.Collections.Specialized;
using N2.Edit;
using N2.Engine;
using N2.Persistence;

namespace N2.Web.Parts
{
	[Service(typeof(IAjaxService))]
	public class ItemMover : PartsAjaxService
    {
        private readonly Navigator navigator;
        private readonly IPersister persister;

        public ItemMover(IPersister persister, Navigator navigator)
        {
            this.persister = persister;
            this.navigator = navigator;
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
            ContentItem parent;

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

            IEnumerable<ContentItem> changedItems = Utility.UpdateSortOrder(parent.Children);
            foreach (ContentItem changedItem in changedItems)
                persister.Save(changedItem);
            persister.Save(item);
        }
    }
}