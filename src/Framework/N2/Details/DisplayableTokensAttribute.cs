using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using N2.Web.Parsing;
using N2.Web.Rendering;
using N2.Web.Wiki.Analyzers;
using System.Diagnostics;
using System.Web.Routing;
using System.Web;
using N2.Web.Mvc;

namespace N2.Details
{
	public class DisplayableTokensAttribute : AbstractDisplayableAttribute, IContentTransformer
	{
		public override Control AddTo(ContentItem item, string detailName, Control container)
		{
			using(var sw = new StringWriter())
			{
				var rc = new RenderingContext { Content = item, Displayable = this, Html = CreateHtmlHelper(item, sw), PropertyName = detailName };
				Render(rc, sw);

				var lc = new LiteralControl(sw.ToString());
				container.Controls.Add(lc);
				return lc;
			}
		}

		private static HtmlHelper CreateHtmlHelper(ContentItem item, TextWriter writer)
		{
			var httpContext = new HttpContextWrapper(HttpContext.Current);
			var routeData = new RouteData();
			RouteExtensions.ApplyCurrentItem(routeData, "webforms", "index", item.ClosestPage(), item);
			return new HtmlHelper(
				new ViewContext(
					new ControllerContext() { HttpContext = httpContext, RequestContext = new RequestContext(httpContext, routeData), RouteData = routeData },
					new WebFormView(HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath), 
					new ViewDataDictionary(), 
					new TempDataDictionary(), 
					writer), 
				new ViewPage(), 
				RouteTable.Routes);
		}



		#region IContentTransformer Members

		public ContentState ChangingTo
		{
			get { return ContentState.Published; }
		}

		public bool Transform(ContentItem item)
		{
			string text = item[Name] as string;
			if(text != null)
			{
				string detailName = Name + "_Tokens";
				int i = 0;
				var p = new Parser(new TemplateAnalyzer());
				foreach (var c in p.Parse(text).Where(c => c.Command != Parser.TextCommand))
				{
					var dc = item.GetDetailCollection(detailName, true);
					var cd = ContentDetail.Multi(detailName, stringValue: c.Tokens.Select(t => t.Fragment).StringJoin(), integerValue: c.Tokens.First().Index);
					cd.EnclosingItem = item;
					cd.EnclosingCollection = dc;

					if (dc.Details.Count > i)
						dc.Details[i] = cd;
					else
						dc.Details.Add(cd);
					i++;					
				}
				if (i > 0)
				{
					var dc = item.GetDetailCollection(detailName, true);
					for (int j = dc.Details.Count - 1; j >= i; j--)
					{
						dc.Details.RemoveAt(j);
					}
					return true;
				}
			}
			return false;
		}

		#endregion

		internal void Render(RenderingContext context, TextWriter writer)
		{
			string text = context.Content[context.PropertyName] as string;
			if (text == null)
				return;

			var tokens = context.Content.GetDetailCollection(context.PropertyName + "_Tokens", false);
			if (tokens != null)
			{
				int lastFragmentEnd = 0;

				foreach (var detail in tokens.Details)
				{
					if (lastFragmentEnd < detail.IntValue)
						writer.Write(text.Substring(lastFragmentEnd, detail.IntValue.Value - lastFragmentEnd));

					string tokenTemplate = detail.StringValue.TextUntil(2, '|', '}');

					ViewEngineResult vr = null;
					if (context.Html.ViewContext.HttpContext.IsCustomErrorEnabled)
					{
						try
						{
							vr = ViewEngines.Engines.FindPartialView(context.Html.ViewContext, "TokenTemplates/" + tokenTemplate);
						}
						catch (System.Exception ex)
						{
							Trace.WriteLine(ex);
						}
					}
					else
					{
						vr = ViewEngines.Engines.FindPartialView(context.Html.ViewContext, "TokenTemplates/" + tokenTemplate); // duplicated to preserve stack trace
					}
					if (vr != null && vr.View != null)
					{
						var model = detail.StringValue[tokenTemplate.Length + 2] == '|'
							? detail.StringValue.Substring(tokenTemplate.Length + 3, detail.StringValue.Length - tokenTemplate.Length - 5)
							: null;
						var vc = new ViewContext(context.Html.ViewContext, vr.View, new ViewDataDictionary(model) { { "ParentViewContext", context.Html.ViewContext } }, context.Html.ViewContext.TempData, writer);
						vr.View.Render(vc, writer);
					}
					else
						writer.Write(detail.StringValue);

					lastFragmentEnd = detail.IntValue.Value + detail.StringValue.Length;
				}

				if (lastFragmentEnd < text.Length)
				{
					writer.Write(text.Substring(lastFragmentEnd, text.Length - lastFragmentEnd));
				}
			}
			else
			{
				writer.Write(text);
			}
		}
	}
}
