using System.Web.Mvc;

namespace N2.Web.Mvc
{
	public abstract class ContentWebViewPage : ContentWebViewPage<ContentItem>
	{
	}

	public abstract class ContentWebViewPage<TModel> : WebViewPage<TModel> where TModel:class
	{
		private ContentContext<TModel> content;

		public ContentContext<TModel> Content
		{
			get { return content ?? (content = new ContentContext<TModel>(this)); }
			set { content = value; }
		}
	}
}