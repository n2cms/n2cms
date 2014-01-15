using System.Web.UI;
using N2.Resources;

namespace N2.Web.UI.WebControls
{
    public class SlidingCurtain : Control
    {
        protected override void OnInit(System.EventArgs e)
        {
            ControlPanelState state = ControlPanel.GetState(Page.GetEngine());
            Visible = !state.IsFlagSet(ControlPanelState.Hidden);
            
            base.OnInit(e);
        }

        public string BackgroundUrl
        {
            get { return (string)(ViewState["VerticalBgUrl"] ?? string.Empty); }
            set { ViewState["VerticalBgUrl"] = value; }
        }

        public string ScriptUrl
        {
            get { return (string)(ViewState["ScriptUrl"] ?? "{ManagementUrl}/Resources/Js/parts.js"); }
            set { ViewState["ScriptUrl"] = value; }
        }

        public string StyleSheetUrl
        {
            get { return (string)(ViewState["StyleSheetUrl"] ?? "{ManagementUrl}/Resources/Css/Parts.css"); }
            set { ViewState["StyleSheetUrl"] = value; }
        }

        private static readonly string scriptFormat = "n2SlidingCurtain.init('#{0}',{1});";

        protected override void OnPreRender(System.EventArgs e)
        {
            if (string.IsNullOrEmpty(ID))
                ID = "SC";

            Register.JQuery(Page);
            Register.JQueryPlugins(Page);
            Register.JavaScript(Page, ScriptUrl);
            Register.StyleSheet(Page, StyleSheetUrl);

            bool isOpen = (ControlPanel.GetState(Page.GetEngine()).IsFlagSet(ControlPanelState.Previewing));
            string startupScript = string.Format(scriptFormat, ClientID, isOpen.ToString().ToLower());
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
            writer.Write("' class='sc");
            var item = ItemUtility.FindCurrentItem(this);
            if (item != null)
                writer.Write(" state" + item.State.ToString());
            writer.Write("'");
            if (BackgroundUrl.Length > 0)
            {
                WriteBgStyle(BackgroundUrl, writer);
            }
            writer.Write(">");
            writer.Write("<div class='scContent'>");

            base.Render(writer);
            writer.Write("<a href='javascript:void(0);' class='close sc-toggler' title='Close'>&laquo;</a><a href='javascript:void(0);' class='open sc-toggler' title='Open'>&raquo;</a>");
            writer.Write("</div></div>");
        }

        private void WriteBgStyle(string url, HtmlTextWriter writer)
        {
            url = N2.Web.Url.ToAbsolute(url);
            writer.Write(" style='background-image:url({0});'", url);
        }
    }
}
