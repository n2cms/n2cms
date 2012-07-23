using System.Linq;
using System.Diagnostics;
using N2.Web;
using N2.Engine;

namespace N2.Addons.UITests.Items
{
	[Adapts(typeof(AdaptiveItemPage))]
	public class AdaptiveRequestAdapter : RequestAdapter
	{
		private readonly Engine.Logger<AdaptiveRequestAdapter> logger;

		public override void AuthorizeRequest(PathData path, System.Security.Principal.IPrincipal user)
		{
			logger.Debug("AuthorizeRequest");
			base.AuthorizeRequest(path, user);
		}
		protected override string GetHandlerPath(PathData path)
		{
			logger.Debug("GetHandlerPath");
			return base.GetHandlerPath(path);
		}
		public override void InjectCurrentPage(PathData path, System.Web.IHttpHandler handler)
		{
			logger.Debug("InjectCurrentPage");
			base.InjectCurrentPage(path, handler);
		}
		public override void RewriteRequest(PathData path, N2.Configuration.RewriteMethod rewriteMethod)
		{
			logger.Debug("RewriteRequest");
			base.RewriteRequest(path, rewriteMethod);
		}
	}

	[Adapts(typeof(AdaptiveItemPage))]
	public class AdaptiveZoneAdapter : N2.Web.Parts.PartsAdapter
	{
        public override System.Collections.Generic.IEnumerable<ContentItem> GetParts(ContentItem parentItem, string zoneName, string @interface)
        {
			var items = base.GetParts(parentItem, zoneName, @interface).ToList();

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