using System;
using System.Collections.Specialized;
using N2.Persistence;
using N2.Web;
using N2.Edit;
using N2.Engine;

namespace N2.Web.Parts
{
	[Service]
	public class ItemDeleter : PartsAjaxService
	{
		readonly IPersister persister;
        readonly Navigator navigator;

		public ItemDeleter(IPersister persister, Navigator navigator, AjaxRequestDispatcher dispatcher)
			: base(dispatcher)
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
            ContentItem item = navigator.Navigate(request["item"]);
			if (item == null)
				throw new N2Exception("Couln't find any item with the id: " + request[PathData.ItemQueryKey]);
			persister.Delete(item);
			return new NameValueCollection();
		}
	}
}
