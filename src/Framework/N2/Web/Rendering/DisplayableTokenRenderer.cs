using N2.Details;
using N2.Engine;
using System.IO;
using System.Web.Mvc;

namespace N2.Web.Rendering
{
	public static class RenderingExtensions
	{
		public static string TextUntil(this string text, char untilFirstIndexOf)
		{
			int index = text.IndexOf(untilFirstIndexOf);
			if (index < 0)
				return text;
			return text.Substring(0, index);
		}
		public static string TextUntil(this string text, string untilFirstIndexOf)
		{
			int index = text.IndexOf(untilFirstIndexOf);
			if (index < 0)
				return text;
			return text.Substring(0, index);
		}
	}

	[Service(typeof(IDisplayableRenderer))]
	public class DisplayableTokenRenderer : DisplayableRendererBase<DisplayableTokensAttribute>
	{
		public override void Render(RenderingContext context, DisplayableTokensAttribute displayable, TextWriter writer)
		{
			string text = context.Content[context.PropertyName] as string;
			if (text == null)
				return;

			var tokens = context.Content.GetDetailCollection(context.PropertyName + "_Tokens", false);
			if (tokens != null)
			{
				writer.Write("<!--" + text + "-->");
				int lastFragmentEnd = 0;

				for (int i = 0; i < tokens.Details.Count; i++)
				{
					var detail = tokens.Details[i];
					if (lastFragmentEnd < detail.IntValue)
						writer.Write(text.Substring(lastFragmentEnd, detail.IntValue.Value - lastFragmentEnd) + "<!--" + lastFragmentEnd + "-" + i + "-->");

					string tokenTemplate = detail.StringValue.TextUntil('|');

					var vr = System.Web.Mvc.ViewEngines.Engines.FindPartialView(context.Html.ViewContext, "TokenTemplates/" + tokenTemplate);
					if (vr.View != null)
						vr.View.Render(new ViewContext(context.Html.ViewContext, vr.View, new ViewDataDictionary(detail.StringValue.Length > tokenTemplate.Length ? detail.StringValue.Substring(tokenTemplate.Length + 1) : null) { { "ParentViewContext", context.Html.ViewContext } }, context.Html.ViewContext.TempData, writer), writer);
					else
						writer.Write("{{" + detail.StringValue + "}}");
					writer.Write("<!--" + lastFragmentEnd + "-" + i + "-->");

					lastFragmentEnd = detail.IntValue.Value + detail.StringValue.Length + 4;
				}

				if (lastFragmentEnd < text.Length)
				{
					writer.Write(text.Substring(lastFragmentEnd, text.Length - lastFragmentEnd) + "<!--" + lastFragmentEnd + "-" + text.Length + "-->");
				}
			}
			else
			{
				writer.Write(text);
			}
		}
	}
}
