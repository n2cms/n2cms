using System.Collections.Generic;
using System.Web.Mvc;
using N2.Persistence.Finder;
using N2.Persistence.NH;
using N2.Web.Mvc.Html;

namespace N2.Web.Mvc
{
	public class QueryHelper
	{
		HtmlHelper html;

		public QueryHelper(HtmlHelper html)
		{
			this.html = html;
		}

		public IItemFinder Items()
		{
			return html.ResolveService<IItemFinder>();
		}

		public IQueryAction Descendants(ContentItem root = null)
		{
			if (root == null)
				root = html.CurrentItem();
			return html.ResolveService<IItemFinder>().Where.AncestralTrail.Like(Utility.GetTrail(root) + "%");
		}

		public IEnumerable<ContentItem> Text(string text)
		{
			return html.ResolveService<ISessionProvider>().OpenSession.FullText(text).Enumerable<ContentItem>();
		}
	}
}