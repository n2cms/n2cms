using N2.Engine;
using N2.Web.Parts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace N2.Web.Mvc.Html
{
	public class PartHelper : IHtmlString
	{
		private System.Web.Mvc.HtmlHelper Html { get; set; }
		private ContentItem item;

		public PartHelper(System.Web.Mvc.HtmlHelper helper, ContentItem item)
		{
			this.Html = helper;
			this.item = item;
		}

		protected IContentAdapterProvider Adapters
		{
			get { return Html.ResolveService<IContentAdapterProvider>(); }
		}

		public string Render(TextWriter writer = null)
		{
			Adapters.ResolveAdapter<PartsAdapter>(item).RenderPart(Html, item, writer ?? Html.ViewContext.Writer);

			return "";
		}

		public override string ToString()
		{
			using (var sw = new StringWriter())
			{
				Render(sw);
				return sw.ToString();
			}
		}

		string IHtmlString.ToHtmlString()
		{
			return ToString();
		}
	}
}
