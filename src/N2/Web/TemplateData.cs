using System;
using System.IO;

namespace N2.Web
{
	/// <summary>
	/// A data carrier used to pass data about a found content item and 
	/// it's template from the content item to the url rewriter.
	/// </summary>
	[Serializable]
	public class TemplateData
	{
		public const string DefaultAction = "";
		public static TemplateData EmptyTemplate()
		{
			return new TemplateData(null, null, null, null, null);
		}

		public TemplateData()
		{
		}

		public TemplateData(ContentItem item, string path, string templateUrl)
			: this(item, path, templateUrl, DefaultAction, string.Empty)
		{
		}

		public TemplateData(ContentItem item, string path, string templateUrl, string action, string arguments)
		{
			CurrentItem = item;

			PagePath = path;
			TemplateUrl = templateUrl;
			Action = action;
			Arguments = arguments;
		}

		public ContentItem CurrentItem { get; set; }

		public string TemplateUrl { get; set; }
		public string PagePath { get; set; }
		public string Action { get; set; }
		public string Arguments { get; set; }

		public virtual Url RewrittenUrl
		{
			get
			{
				if(CurrentItem == null)
					return null;

				if (CurrentItem.IsPage)
					return Url.Parse(TemplateUrl).AppendQuery("page", CurrentItem.ID);
				
				for (ContentItem ancestor = CurrentItem.Parent; ancestor != null; ancestor = ancestor.Parent)
					if (ancestor.IsPage)
						return ancestor.FindTemplate(DefaultAction).RewrittenUrl.AppendQuery("item", CurrentItem.ID);

				if (CurrentItem.VersionOf != null)
					return CurrentItem.VersionOf.FindTemplate(DefaultAction).RewrittenUrl.SetQueryParameter("item", CurrentItem.ID);

				throw new TemplateNotFoundException(CurrentItem);
			}
		}
	}
}