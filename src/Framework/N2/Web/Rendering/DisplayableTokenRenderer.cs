using N2.Details;
using N2.Engine;
using System.IO;
using System.Web.Mvc;
using System.Diagnostics;

namespace N2.Web.Rendering
{
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
				int lastFragmentEnd = 0;

				for (int i = 0; i < tokens.Details.Count; i++)
				{
					var detail = tokens.Details[i];
					if (lastFragmentEnd < detail.IntValue)
						writer.Write(text.Substring(lastFragmentEnd, detail.IntValue.Value - lastFragmentEnd));

					string tokenTemplate = detail.StringValue.TextUntil(2, '|', '}');

					ViewEngineResult vr = null;
					try
					{
						vr = ViewEngines.Engines.FindPartialView(context.Html.ViewContext, "TokenTemplates/" + tokenTemplate);
					}
					catch (System.Exception ex)
					{
						Trace.WriteLine(ex);
					}
					if (vr != null && vr.View != null)
					{
						var data = (detail.StringValue.Length > tokenTemplate.Length + 4)
							? detail.StringValue.Substring(2 + tokenTemplate.Length + 1, detail.StringValue.Length - 2 - tokenTemplate.Length - 1 - 2)
							: detail.StringValue;
						var vc = new ViewContext(context.Html.ViewContext, vr.View, new ViewDataDictionary(detail.StringValue.Length > tokenTemplate.Length ? detail.StringValue.Substring(tokenTemplate.Length + 1) : null) { { "ParentViewContext", context.Html.ViewContext } }, context.Html.ViewContext.TempData, writer);
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
