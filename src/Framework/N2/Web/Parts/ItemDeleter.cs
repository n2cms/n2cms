using System.Collections.Specialized;
using N2.Edit;
using N2.Engine;
using N2.Persistence;

namespace N2.Web.Parts
{
    [Service(typeof(IAjaxService))]
    public class ItemDeleter : PartsAjaxService
    {
        readonly IPersister persister;
        readonly Navigator navigator;

        public ItemDeleter(IPersister persister, Navigator navigator)
        {
            this.persister = persister;
            this.navigator = navigator;
        }

        public override string Name
        {
            get { return "delete"; }
        }

        public override NameValueCollection HandleRequest(NameValueCollection request)
        {
            ContentItem item = navigator.Navigate(request[PathData.ItemQueryKey]);
            if (item == null)
                throw new N2Exception("Couln't find any item with the id: " + request[PathData.ItemQueryKey]);
            persister.Delete(item);
            return new NameValueCollection();
        }
    }
}
