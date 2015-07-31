using N2.Engine;
using N2.Web.Parts;
using System.Text;
using N2.Details;
using N2.Web.Mvc.Html;
using N2.Web.UI.WebControls;

namespace N2.ReusableParts
{

	[PartDefinition("Html Anchor", IconClass = "fa fa-anchor")]
	[WithEditableTitle]
	class HtmlAnchor : ContentItem
	{
	}

	[Adapts(typeof(HtmlAnchor))]
	class HtmlAnchorAdapter : PartsAdapter
	{
		public static string GetHtml(HtmlAnchor part, bool designMode)
		{
			StringBuilder sb = new StringBuilder();
			if (designMode)
				sb.Append(@"<span class=""label""><i class=""fa fa-anchor""></i> ");
			sb.AppendFormat(@"<a name=""{0}"" />", part.Title);
			if (designMode)
				sb.Append("</span>");
			return sb.ToString();
		}

		public override void RenderPart(System.Web.Mvc.HtmlHelper html, ContentItem part, System.IO.TextWriter writer = null)
		{
			bool designMode = ControlPanelExtensions.GetControlPanelState(html).HasFlag(ControlPanelState.DragDrop);
			html.ViewContext.Writer.Write(GetHtml(part as HtmlAnchor, designMode));
		}
	}
}
