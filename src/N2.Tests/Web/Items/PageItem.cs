using N2.Collections;
using N2.Web;

namespace N2.Tests.Web
{
	public class PageItem : ContentItem
	{
		public override string Url
		{
			get { return "/" + Name + Extension; }
		}

		public override string RewrittenUrl
		{
			get { return TemplateUrl.TrimStart('~') + "?page=" + ID; }
		}

		public override ItemList GetChildren()
		{
			return GetChildren(new ItemFilter[0]);
		}
	}
}
