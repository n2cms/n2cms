using System;
using System.Collections.Generic;
using System.Linq;
using N2.Definitions;
using N2.Web.Mvc.Html;
using System.Dynamic;
using System.Web.Mvc;
using System.Web.UI;
using System.IO;
using N2.Details;

namespace N2.Web.Mvc
{
	/// <remarks>This code is here since it has dependencies on ASP.NET 3.0 which isn't a requirement for N2 in general.</remarks>
	public class DisplayHelper : DynamicObject
	{
		public ContentItem Current { get; set; }
		public HtmlHelper Html { get; set; }
		
		public DisplayRenderer<IDisplayable> this[string detailname]
		{
			get { return new DisplayRenderer<IDisplayable>(Html, detailname); }
		}

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return Current.GetContentType().GetProperties().Select(p => p.Name);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (Current == null)
			{
				result = null;
				return true;
			}

			string name = binder.Name;

			try
			{
				object data = Html.DisplayContent(Current, name).ToString();
				result = data.ToHtmlString();
			}
			catch (N2Exception)
			{
				if (Html.ViewData.ContainsKey("RegistrationExpression"))
				{
					result = null;
					return true;
				}

				var template = Html.ResolveService<ITemplateAggregator>().GetTemplate(Current);
				var displayable = template.Definition.Displayables.FirstOrDefault(d => d.Name == name);

				object data;
				if (displayable != null)
				{
					var vp = new ViewPage();
					displayable.AddTo(Current, name, vp);

					using (var sw = new StringWriter())
					using (var htw = new HtmlTextWriter(sw))
					{
						vp.RenderControl(htw);
						data = sw.ToString();
					}
				}
				else
					data = Current[name];

				result = data.ToHtmlString();
			}

			return true;
		}
	}
}
