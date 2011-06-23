using System.Web.Mvc;
using System.Web;
using N2.Details;
using N2.Definitions;
using N2.Web.Rendering;
using N2.Definitions.Runtime;
using N2.Collections;

namespace N2.Web.Mvc
{
	/// <remarks>This code is here since it has dependencies on ASP.NET 3.0 which isn't a requirement for N2 in general.</remarks>
	public class DynamicContentHelper : ViewContentHelper
	{
		public DynamicContentHelper(HtmlHelper html)
			: base(html)
		{
		}

		public dynamic Display
		{
			get { return new DisplayHelper { Html = Html, Current = Current.Item }; }
		}

		public dynamic Has
		{
			get { return new HasValueHelper(HasValue); }
		}

		public dynamic Data
		{
			get
			{
				if (Current.Item == null)
					return new DataHelper(() => Current.Item);

				string key = "DataHelper" + Current.Item.ID;
				var data = Html.ViewContext.ViewData[key] as DataHelper;
				if (data == null)
					Html.ViewContext.ViewData[key] = data = new DataHelper(() => Current.Item);
				return data;
			}
		}

		public TranslateHelper Translate
		{
			get { return new TranslateHelper(); }
		}

		// Room for future improvement.
		public class TranslateHelper
		{
			public IHtmlString this[string key]
			{
				get { return Html(key); }
			}

			public IHtmlString Html(string key)
			{
				return new HtmlString(key);
			}

			public string Text(string key)
			{
				return key;
			}
		}
	}
}