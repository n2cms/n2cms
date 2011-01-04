using System;
using System.Collections.Specialized;
using N2.Edit;
using N2.Engine;

namespace N2.Web.Parts
{
	[Service]
	public class EditUrlProvider : PartsAjaxService
	{
		private readonly IEditUrlManager editUrlManager;
		private readonly Navigator navigator;

		public EditUrlProvider(Navigator navigator, IEditUrlManager editUrlManager, AjaxRequestDispatcher dispatcher)
			: base(dispatcher)
		{
			this.navigator = navigator;
			this.editUrlManager = editUrlManager;
		}

		public override string Name
		{
			get { return "edit"; }
		}

		public override NameValueCollection HandleRequest(NameValueCollection request)
		{
			var response = new NameValueCollection();
			response["redirect"] = GetRedirectUrl(request);
			;
			return response;
		}

		private string GetRedirectUrl(NameValueCollection request)
		{
			ContentItem item = navigator.Navigate(request["item"]);
			Url url = editUrlManager.GetEditExistingItemUrl(item);
			if (!string.IsNullOrEmpty(request["returnUrl"]))
				url = url.AppendQuery("returnUrl", request["returnUrl"]);
			return url;
		}
	}
}