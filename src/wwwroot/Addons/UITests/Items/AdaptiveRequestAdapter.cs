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
		public override void RewriteRequest(N2.Configuration.RewriteMethod rewriteMethod)
		{
			Debug.WriteLine("RewriteRequest");
			base.RewriteRequest(rewriteMethod);
		}
	}

	[Controls(typeof(AdaptiveItemPage))]
	public class AdaptiveZoneAdapter : N2.Web.Parts.PartsAdapter
	{
		public override N2.Collections.ItemList GetItemsInZone(ContentItem parentItem, string zoneName)
		{
			var items = base.GetItemsInZone(parentItem, zoneName);

			if (zoneName == "AutoZone1")
			{
				items.Add(new UITestItemItem());
			}
			if (zoneName == "AutoZone2")
			{
				items.Add(new UITestItemItem());
				items.Add(new UITestItemItem());
			}
			if (zoneName == "AutoZone3")
			{
				items.Add(new UITestItemItem());
				items.Add(new UITestItemItem());
				items.Add(new UITestItemItem());
			}

			return items;
		}
	}
}