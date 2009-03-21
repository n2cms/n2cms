using System.Diagnostics;
using N2.Web;

namespace N2.Addons.UITests.Items
{
	[Controls(typeof(AdaptiveItemPage))]
	public class AdaptiveRequestAdapter : RequestAdapter
	{
		public override void AuthorizeRequest(System.Security.Principal.IPrincipal user)
		{
			Debug.WriteLine("AuthorizeRequest");
			base.AuthorizeRequest(user);
		}
		public override void InjectCurrentPage(System.Web.IHttpHandler handler)
		{
			Debug.WriteLine("InjectCurrentPage");
			base.InjectCurrentPage(handler);
		}
		public override void RewriteRequest()
		{
			Debug.WriteLine("RewriteRequest");
			base.RewriteRequest();
		}
	}
}