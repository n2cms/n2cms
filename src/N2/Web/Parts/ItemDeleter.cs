using System;
using System.Collections.Specialized;
using N2.Persistence;
using N2.Web;

namespace N2.Web.Parts
{
	public class ItemDeleter : PartsAjaxService
	{
		private readonly IPersister persister;

		public ItemDeleter(IPersister persister, AjaxRequestDispatcher dispatcher)
			: base(dispatcher)
		{
			this.persister = persister;
		}

		public override string Name
		{
			get { return "delete"; }
		}

		public override NameValueCollection HandleRequest(NameValueCollection request)
		{
			ContentItem item = persister.Get(int.Parse(request["item"]));
			if (item == null)
				throw new N2Exception("Couln't find any item with the id: " + request["item"]);
			persister.Delete(item);
			return new NameValueCollection();
		}
	}
}
