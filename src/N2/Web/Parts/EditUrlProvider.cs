using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using N2.Edit;
using N2.Persistence;
using N2.Web;
using N2.Engine;

namespace N2.Web.Parts
{
	[Service]
	public class EditUrlProvider : PartsAjaxService
	{
		readonly IEditManager editManager;
        readonly Navigator navigator;

		public EditUrlProvider(Navigator navigator, IEditManager editManager, AjaxRequestDispatcher dispatcher)
			: base(dispatcher)
		{
            this.navigator = navigator;
			this.editManager = editManager;
		}

		public override string Name
		{
			get { return "edit"; }
		}

		public override NameValueCollection HandleRequest(NameValueCollection request)
		{
			NameValueCollection response = new NameValueCollection();
			response["redirect"] = GetRedirectUrl(request); ;
			return response;
		}

		private string GetRedirectUrl(NameValueCollection request)
		{
            ContentItem item = navigator.Navigate(request["item"]);
			Url url = editManager.GetEditExistingItemUrl(item);
			if (!string.IsNullOrEmpty(request["returnUrl"]))
				url = url.AppendQuery("returnUrl", request["returnUrl"]);
			return url;
		}
	}
}