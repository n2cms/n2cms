using System.Diagnostics;
using N2;
using N2.Web;
using N2.Details;

namespace N2.Addons.UITests.Items
{
	[Controls(typeof(AdaptiveItemPage))]
	public class AdaptiveController : RequestAspectController
	{
		public override void AuthorizeRequest(System.Security.Principal.IPrincipal user, N2.Security.ISecurityEnforcer security)
		{
			Debug.WriteLine("AuthorizeRequest");
			base.AuthorizeRequest(user, security);
		}
		public override void HandleError(System.Exception ex)
		{
			Debug.WriteLine("HandleError");
			base.HandleError(ex);
		}
		public override void InjectCurrentPage(System.Web.IHttpHandler handler)
		{
			Debug.WriteLine("InjectCurrentPage");
			base.InjectCurrentPage(handler);
		}
		public override void RewriteRequest(IWebContext webContext)
		{
			Debug.WriteLine("RewriteRequest");
			base.RewriteRequest(webContext);
		}
	}

	[Definition("Multiple Tests Page", SortOrder = 10000)]
	[WithEditableTitle, WithEditableName]
	public class AdaptiveItemPage : ContentItem
	{
		[EditableFreeTextArea("Text", 100)]
		public virtual string Text
		{
			get { return GetDetail("Text", ""); }
			set { SetDetail("Text", value, ""); }
		}

		public override PathData FindPath(string remainingUrl)
		{
			PathData data = base.FindPath(remainingUrl);
			if(data.CurrentItem != null && data.CurrentItem != this)
				return data;

			if(string.IsNullOrEmpty(remainingUrl))
				return new PathData(this, "~/Addons/UITests/UI/AdaptiveItem.aspx");

			return new PathData(this, "~/Addons/UITests/UI/" + remainingUrl + ".aspx");
		}
	}
}
