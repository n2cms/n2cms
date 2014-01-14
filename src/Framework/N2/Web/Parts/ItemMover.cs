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
        readonly IIntegrityManager integrity;
        readonly IVersionManager versions;
        readonly ContentVersionRepository versionRepository;

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
            var response = new NameValueCollection();
            response["redirect"] = MoveItem(request);
            return response;
        }

        private string MoveItem(NameValueCollection request)
        {
            var path = PartsExtensions.EnsureDraft(versions, versionRepository, navigator, request);
            ContentItem item = path.CurrentItem;
            ContentItem page = path.CurrentPage;
            
            item.ZoneName = request["zone"];

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
        }

        private void ValidateLocation(ContentItem item, ContentItem parent)
        {
            var ex = integrity.GetMoveException(item, parent);
            if (ex != null)
                throw ex;
        }
    }
}
