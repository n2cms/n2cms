using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using System.Web.Mvc;
using N2.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Web.Routing;
using N2.Web.Parts;
using N2.Plugin;
using N2.Edit;

namespace N2.Web.Mvc.Html
{
	public static class ControlPanelExtensions
	{

		static string format = @"
<script src='{ManagementUrl}Resources/Js/jquery-1.4.4.js' type='text/javascript'></script>
<script src='{ManagementUrl}Resources/Js/plugins.ashx?v={Version}' type='text/javascript'></script>
<script src='{ManagementUrl}Resources/Js/jquery.ui.ashx?v={Version}' type='text/javascript'></script>
<script src='{ManagementUrl}Resources/Js/parts.js' type='text/javascript'></script>
<link href='{ManagementUrl}Resources/Css/Parts.css' type='text/css' media='all' rel='stylesheet' />
<script type='text/javascript'>//<![CDATA[
	jQuery(document).ready(function () { n2SlidingCurtain.init('#cpCurtain', false); });
	if (window.n2ctx) {
		n2ctx.select('preview');

		jQuery(document).ready(function () {
			if (window.n2ctx) {
				n2ctx.refresh({ navigationUrl: '{NavigationUrl}', path: '{Path}', force: false });
				if (n2ctx.hasTop()) jQuery('.cpAdminister').hide();
				else jQuery('.cpView').hide();
			}
			if (window.n2SlidingCurtain) n2SlidingCurtain.recalculate();
			window.n2ddcp = new n2DragDrop();
		});
	}
//]]></script>

<div id='cpCurtain' class='sc'><div class='scContent'>
	<div class='controlPanel'>
		<div class='plugins'>
			{Plugins}
		</div>
		{Definitions}
	</div>
	<a href='javascript:void(0);' class='close' title='Close'>&laquo;</a>
	<a href='javascript:void(0);' class='open' title='Open'>&raquo;</a>
</div></div>
";

		public static void RenderControlPanel(this HtmlHelper html)
		{
			var engine = html.ContentEngine();
			if (!engine.SecurityManager.IsEditor(html.ViewContext.HttpContext.User))
				return;

			var item = html.CurrentItem() ?? html.StartPage();
			var state = ControlPanel.GetState(html.ViewContext.HttpContext.User, html.ViewContext.HttpContext.Request.QueryString);
			var settings = new
			{
				NavigationUrl = engine.ManagementPaths.GetNavigationUrl(item),
				ManagementUrl = engine.ManagementPaths.GetManagementInterfaceUrl(),
				Path = item.Path,
				Plugins = Plugins(html, item, state),
				Definitions = Definitions(html, engine, item, state),
				Version = typeof(ContentItem).Assembly.GetName().Version.ToString()
			};

			string controlPanelHtml = format.Replace(settings);
			html.ViewContext.Writer.Write(controlPanelHtml);
		}

		private static string Plugins(HtmlHelper html, ContentItem item, ControlPanelState state)
		{
			ContentItem start = html.StartPage();
			ContentItem root = html.RootPage();

			Page p = new Page();
			foreach (IControlPanelPlugin plugin in html.ContentEngine().Resolve<IPluginFinder>().GetPlugins<IControlPanelPlugin>())
			{
				var span = new System.Web.UI.HtmlControls.HtmlGenericControl("span");
				span.Attributes["class"] = "control";
				var pluginControl = plugin.AddTo(span, new PluginContext(item, null, start, root, state, html.ContentEngine(), html.ViewContext.HttpContext));

				if (pluginControl != null)
					p.Controls.Add(span);
			}

			using (var sw = new StringWriter())
			using (var htw = new HtmlTextWriter(sw))
			{
				p.RenderControl(htw);
				return sw.ToString();
			}
		}

		private static string Definitions(HtmlHelper html, IEngine engine, ContentItem item, ControlPanelState state)
		{
			if (state == ControlPanelState.DragDrop)
			{
				StringBuilder sb = new StringBuilder();

				foreach (var d in ControlPanel.GetPartDefinitions(engine.ResolveAdapter<PartsAdapter>(item), item, null, html.ViewContext.HttpContext.User))
				{
					sb.AppendFormat(
						"<div id='{0}' title='{1}' data-type='{2}' class='{3}'>{4}</div>",
						d.Discriminator, d.ToolTip, d.Discriminator, "definition " + d.Discriminator, ControlPanel.FormatImageAndText(d.IconUrl, d.Title));
				}

				if (sb.Length > 0)
					return "<div class='definitions'>" + sb + "</div>";
			}
			return "";
		}

		public static string Replace(this string format, IDictionary<string, object> replacements)
		{
			foreach (var kvp in replacements)
			{
				format = format.Replace("{" + kvp.Key + "}", kvp.Value != null ? kvp.Value.ToString() : "");
			}
			return format;
		}

		public static string Replace(this string format, object replacements)
		{
			return format.Replace(new RouteValueDictionary(replacements));
		}
	}
}
