using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Collections;
using N2.Linq;
using N2.Web.Mvc.Html;
using N2.Persistence.Finder;
using N2.Engine;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Provides quick acccess to often used APIs.
	/// </summary>
	public class ViewContentHelper : ContentHelperBase
	{
		HtmlHelper html;

		public ViewContentHelper(HtmlHelper html)
			: base(html.ContentEngine(), () => html.CurrentPath())
		{
			this.html = html;
		}

		public ViewContentHelper(HtmlHelper html, IEngine engine, Func<PathData> pathGetter)
			: base (engine, pathGetter)
		{
			this.html = html;
		}

		public HtmlHelper Html
		{
			get { return html; }
		}

		public virtual RegisterHelper Register
		{
			get { return new RegisterHelper(Html); }
		}

		public virtual RenderHelper Render
		{
			get { return new RenderHelper { Html = Html, Content = Current.Page }; }
		}

		public bool HasValue(string detailName)
		{
			return Current.Item != null 
				&& Current.Item[detailName] != null 
				&& !("".Equals(Current.Item[detailName]));
		}
	}


}