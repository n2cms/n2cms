using System;
using System.IO;
using System.Collections.Generic;

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

		public TemplateData(ContentItem item, string path, string templateUrl)
			: this(item, path, templateUrl, DefaultAction, string.Empty)
		{
			
		}

		public TemplateData(ContentItem item, string path, string templateUrl, string action, string arguments)
			: this()
		{
			CurrentItem = item;

			PagePath = path;
			TemplateUrl = templateUrl;
			Action = action;
			Argument = arguments;
		}

		public TemplateData()
		{
			QueryParameters = new Dictionary<string, string>();
		}

		public ContentItem CurrentItem { get; set; }

		public string TemplateUrl { get; set; }
		public string PagePath { get; set; }
		public string Action { get; set; }
		public string Argument { get; set; }
		public IDictionary<string, string> QueryParameters { get; set; }

		public virtual Url RewrittenUrl
		{
			get
			{
				if(CurrentItem == null)
					return null;

				if (CurrentItem.IsPage)
					return Url.Parse(TemplateUrl).UpdateQuery(QueryParameters).SetQueryParameter("page", CurrentItem.ID);
				
				for (ContentItem ancestor = CurrentItem.Parent; ancestor != null; ancestor = ancestor.Parent)
					if (ancestor.IsPage)
						return ancestor.FindTemplate(DefaultAction).RewrittenUrl.UpdateQuery(QueryParameters).SetQueryParameter("item", CurrentItem.ID);

				if (CurrentItem.VersionOf != null)
					return CurrentItem.VersionOf.FindTemplate(DefaultAction).RewrittenUrl.UpdateQuery(QueryParameters).SetQueryParameter("item", CurrentItem.ID);

				throw new TemplateNotFoundException(CurrentItem);
			}
		}

		public TemplateData UpdateParameters(IDictionary<string, string> queryString)
		{
			foreach (KeyValuePair<string, string> pair in queryString)
				QueryParameters[pair.Key] = pair.Value;
			return this;
		}
	}
}