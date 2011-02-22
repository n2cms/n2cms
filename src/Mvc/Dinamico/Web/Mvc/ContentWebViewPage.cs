using System.Web.Mvc;
using N2.Web.Mvc.Html;

namespace N2.Web.Mvc
{
	public abstract class ContentWebViewPage : ContentWebViewPage<ContentItem>
	{
	}

	public abstract class ContentWebViewPage<TModel> : WebViewPage<TModel> where TModel:class
	{
		private ContentHelper<TModel> content;

		public ContentHelper<TModel> Content
		{
			get { return content ?? (content = this.Html.Content()); }
			set { content = value; }
		}
	}
}