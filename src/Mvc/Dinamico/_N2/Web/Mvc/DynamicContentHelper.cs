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
			get { return new DisplayHelper { Html = Html, Current = Path.CurrentItem }; }
		}

		public dynamic Has
		{
			get { return new HasValueHelper(HasValue); }
		}

		public dynamic Data
		{
			get
			{
				if (Path.CurrentItem == null)
					return new DataHelper(() => Path.CurrentItem);

				string key = "DataHelper" + Path.CurrentItem.ID;
				var data = Html.ViewContext.ViewData[key] as DataHelper;
				if (data == null)
					Html.ViewContext.ViewData[key] = data = new DataHelper(() => Path.CurrentItem);
				return data;
			}
		}
	}
}