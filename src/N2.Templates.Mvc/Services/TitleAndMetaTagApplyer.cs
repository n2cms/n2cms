using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Templates.Mvc.Services
{
	/// <summary>
	/// Adds SEO title, keywords and description to the page.
	/// </summary>
	public class TitleAndMetaTagApplyer
	{
		private readonly Page page;
		private readonly ContentItem item;

		public const string HeadTitle = "HeadTitle";
		public const string MetaKeywords = "MetaKeywords";
		public const string MetaDescription = "MetaDescription";

		public TitleAndMetaTagApplyer(Page page, ContentItem item)
		{
			this.page = page;
			this.item = item;
			if(item != null)
				page.Init += Page_Init;
		}

		void Page_Init(object sender, EventArgs e)
		{
			page.Title = item[HeadTitle] as string ?? item.Title;
			AddMeta("keywords", item[MetaKeywords] as string);
			AddMeta("description", item[MetaDescription] as string);
		}

		private void AddMeta(string name, string content)
		{
			if (!string.IsNullOrEmpty(content))
			{
				HtmlGenericControl meta = new HtmlGenericControl("meta");
				meta.Attributes["name"] = name;
				meta.Attributes["content"] = content;
				page.Header.Controls.Add(meta);
			}
		}
	}
}