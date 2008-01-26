using System.Web;
using System.Web.UI;
using N2.Resources;
using N2.Web.UI.WebControls;

[assembly: WebResource("N2.Parts.Web.UI.WebControls.SlidingCurtain_bg.png", "image/png")]
[assembly: WebResource("N2.Parts.Web.UI.WebControls.SlidingCurtain_bg_bottom.png", "image/png")]

namespace N2.Parts.Web.UI.WebControls
{
	public class SlidingCurtain : Control
	{
		protected override void OnInit(System.EventArgs e)
		{
			ControlPanelState state = ControlPanel.GetState();
			Visible = state != ControlPanelState.Hidden;
			
			base.OnInit(e);
		}

		public string VerticalBgUrl
		{
			get { return (string)(ViewState["VerticalBgUrl"] ?? string.Empty); }
			set { ViewState["VerticalBgUrl"] = value; }
		}

		public string BottomBgUrl
		{
			get { return (string)(ViewState["BottomBgUrl"] ?? string.Empty); }
			set { ViewState["BottomBgUrl"] = value; }
		}

		public string ScriptUrl
		{
			get { return (string)(ViewState["ScriptUrl"] ?? GetWebResourceUrl("N2.Parts.Web.UI.WebControls.Parts.js")); }
			set { ViewState["ScriptUrl"] = value; }
		}

		public string StyleSheetUrl
		{
			get { return (string)(ViewState["StyleSheetUrl"] ?? GetWebResourceUrl("N2.Parts.Web.UI.WebControls.Parts.css")); }
			set { ViewState["StyleSheetUrl"] = value; }
		}

		private static readonly string scriptFormat = "SlidingCurtain('#{0}');";

		protected override void OnPreRender(System.EventArgs e)
		{
			if (string.IsNullOrEmpty(ID))
				ID = "SC";

			Register.JQuery(Page);
			Register.JavaScript(Page, ScriptUrl);
			Register.StyleSheet(Page, StyleSheetUrl);

			string startupScript = string.Format(scriptFormat, ClientID);
			Register.JavaScript(Page, startupScript, ScriptOptions.DocumentReady);

			base.OnPreRender(e);
		}

		private string GetWebResourceUrl(string name)
		{
			return Page.ClientScript.GetWebResourceUrl(typeof(SlidingCurtain), name);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write("<div id='");
			writer.Write(ClientID);
			writer.Write("' class='sc'");
			if(BottomBgUrl.Length > 0)
			{
				WriteBgStyle(BottomBgUrl, writer);
			}
			writer.Write(">");

			writer.Write("<div class='scContent'");
			if (VerticalBgUrl.Length > 0)
			{
				WriteBgStyle(VerticalBgUrl, writer);
			}
			writer.Write(">");

			base.Render(writer);
			writer.Write("<span class='close'>&laquo;</span><span class='open'>&raquo;</span>");
			writer.Write("</div></div>");
		}

		private void WriteBgStyle(string url, HtmlTextWriter writer)
		{
			url = Utility.ToAbsolute(url);
			writer.Write(" style='background-image:url({0});'", url);
		}
	}
}
