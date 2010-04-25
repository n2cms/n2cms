using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Templates.Web.UI;
using N2.Engine;

namespace N2.Templates.Services
{
    /// <summary>
    /// Adds SEO title, keywords and description to the page.
    /// </summary>
	[Service(typeof(TemplateConcern))]
	public class SeoConcern : TemplateConcern
    {
        public const string HeadTitle = "HeadTitle";
        public const string MetaKeywords = "MetaKeywords";
        public const string MetaDescription = "MetaDescription";

		public override void OnPreInit(ITemplatePage template)
		{
			var item = template.CurrentItem;
			if (item != null)
			{
				template.Page.Init += delegate
				{
					template.Page.Title = item[HeadTitle] as string ?? item.Title;
					AddMeta(template.Page, "keywords", item[MetaKeywords] as string);
					AddMeta(template.Page, "description", item[MetaDescription] as string);
				};
			}
		}

        private void AddMeta(Page page, string name, string content)
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