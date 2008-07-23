using N2.Collections;

namespace N2.Tests.Web
{
	public class PageItem : ContentItem
	{
		public override string Url
		{
			get
			{
				return "/" + Name + Extension;
			}
		}

		public override string RewrittenUrl
		{
			get
			{
				return TemplateUrl.Replace("~/", "/") + "?page=" + ID;
			}
		}

		public override ItemList GetChildren()
		{
			return GetChildren(new ItemFilter[0]);
		}
	}
}
