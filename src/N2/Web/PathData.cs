using System;
using System.Collections.Generic;

namespace N2.Web
{
	/// <summary>
	/// A data carrier used to pass data about a found content item and 
	/// it's template from the content item to the url rewriter.
	/// </summary>
	[Serializable]
	public class PathData
	{
		public const string DefaultAction = "";
		public static PathData EmptyTemplate()
		{
			return new PathData(null, null, null, null);
		}

		public PathData(ContentItem item, string templateUrl, string action, string arguments)
			: this()
		{
			if(item != null)
			{
				CurrentItem = item;
				ID = item.ID;
			}
			TemplateUrl = templateUrl;
			Action = action;
			Argument = arguments;
		}

		public PathData(ContentItem item, string templateUrl)
			: this(item, templateUrl, DefaultAction, string.Empty)
		{

		}

		public PathData(int id, string path, string templateUrl, string action, string arguments)
			: this()
		{
			ID = id;
			Path = path;
			TemplateUrl = templateUrl;
			Action = action;
			Argument = arguments;
		}

		public PathData()
		{
			QueryParameters = new Dictionary<string, string>();
		}

		public ContentItem CurrentItem { get; set; }

		public string TemplateUrl { get; set; }
		public int ID { get; set; }
		public string Path { get; set; }
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
						return ancestor.FindPath(DefaultAction).RewrittenUrl.UpdateQuery(QueryParameters).SetQueryParameter("item", CurrentItem.ID);

				if (CurrentItem.VersionOf != null)
					return CurrentItem.VersionOf.FindPath(DefaultAction).RewrittenUrl.UpdateQuery(QueryParameters).SetQueryParameter("item", CurrentItem.ID);

				throw new TemplateNotFoundException(CurrentItem);
			}
		}

		public PathData UpdateParameters(IDictionary<string, string> queryString)
		{
			foreach (KeyValuePair<string, string> pair in queryString)
				QueryParameters[pair.Key] = pair.Value;
			return this;
		}

		public PathData Detach()
		{
			PathData data = new PathData(ID, Path, TemplateUrl, Action, Argument);
			data.QueryParameters = new Dictionary<string, string>(data.QueryParameters);
			return data;
		}

		public PathData Attach(N2.Persistence.IPersister persister)
		{
			ContentItem item = persister.Repository.Load(ID);
			PathData data = new PathData(item, TemplateUrl, Action, Argument);
			data.QueryParameters = new Dictionary<string, string>(QueryParameters);
			return data;
		}

		public PathData SetArguments(string argument)
		{
			Argument = argument;
			return this;
		}
	}
}