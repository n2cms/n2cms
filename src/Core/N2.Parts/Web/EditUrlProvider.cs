using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using N2.Edit;
using N2.Persistence;
using N2.Web;

namespace N2.Parts.Web
{
	public class EditUrlProvider : PartsAjaxService
	{
		private readonly IPersister persister;
		private readonly IEditManager editManager;

		public EditUrlProvider(IPersister persister, IEditManager editManager, AjaxRequestDispatcher dispatcher)
			: base(dispatcher)
		{
			this.persister = persister;
			this.editManager = editManager;
		}

		public override string Name
		{
			get { return "edit"; }
		}

		public override NameValueCollection Handle(NameValueCollection request)
		{
			NameValueCollection response = new NameValueCollection();
			response["url"] = GetRedirectUrl(request); ;
			return response;
		}

		private string GetRedirectUrl(NameValueCollection request)
		{
			ContentItem item = persister.Get(int.Parse(request["item"]));
			string url = editManager.GetEditExistingItemUrl(item);
			url = Utility.ToAbsolute(url);
			if (!string.IsNullOrEmpty(request["returnUrl"]))
				url += "&returnUrl=" + HttpUtility.UrlEncode(request["returnUrl"]);
			return url;
		}
	}
}