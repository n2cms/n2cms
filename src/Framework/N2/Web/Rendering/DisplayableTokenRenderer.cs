using N2.Details;
using N2.Engine;
using System.IO;
using System.Web.Mvc;
using System.Diagnostics;
using System;

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

				foreach(var detail in tokens.Details)
				{
					if (lastFragmentEnd < detail.IntValue)
						writer.Write(text.Substring(lastFragmentEnd, detail.IntValue.Value - lastFragmentEnd));

					int pipeIndex = detail.StringValue.IndexOf('|', 2);
					int bracketIndex = detail.StringValue.IndexOf('}', 2);

					string tokenTemplate = detail.StringValue.Substring(2, Math.Min(pipeIndex, bracketIndex) - 2);

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
						var model = (pipeIndex < bracketIndex)
							? detail.StringValue.Substring(pipeIndex + 1, bracketIndex - pipeIndex - 1)
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
