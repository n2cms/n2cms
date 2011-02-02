using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using N2.Web.Mvc.Html;
using N2.Definitions;
using System.IO;
using System.Web.UI;

namespace N2.Web.Mvc
{
	public class DisplayHelper<TModel> : DynamicObject where TModel : class
	{
		HtmlHelper<TModel> html;
		ContentItem current;

		public DisplayHelper(HtmlHelper<TModel> html, ContentItem current)
		{
			this.html = html;
			this.current = current;
		}

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return current.GetContentType().GetProperties().Select(p => p.Name);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if(current == null)
			{
				result = null;
				return true;
			}

			string name = binder.Name;

			try
			{
				object data = html.DisplayContent(current, name).ToString();
				result = data.ToHtmlString();
			}
			catch (N2Exception)
			{
				if (html.ViewData.ContainsKey("RegistrationExpression"))
				{
					result = null;
					return true;
				}

				var template = html.ResolveServices<ITemplateProvider>().GetTemplate(current);
				var displayable = template.Definition.Displayables.FirstOrDefault(d => d.Name == name);

				object data;
				if (displayable != null)
				{
					var vp = new ViewPage();
					displayable.AddTo(current, name, vp);

					using (var sw = new StringWriter())
					using (var htw = new HtmlTextWriter(sw))
					{
						vp.RenderControl(htw);
						data = sw.ToString();
					}
				}
				else
					data = current[name];

				result = data.ToHtmlString();
			}

			return true;
		}
	}
}