using N2.Resources;
using N2.Definitions;
using System.Web.UI.WebControls;
using N2.Engine;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System;

namespace N2.Web.UI.WebControls
{
    /// <summary>An input box that can be updated with the url to a file through a popup window.</summary>
    public class MediaSelector : TextBox
    {
        public MediaSelector()
        {
            CssClass = "fileSelector selector";
            BrowserUrl = N2.Web.Url.Parse(Engine.ManagementPaths.MediaBrowserUrl).AppendQuery("mc=true");
            PopupOptions = "height=600,width=960,resizable=yes,status=no,scrollbars=yes";
        }

        private IEngine engine;
        protected IEngine Engine
        {
            get { return engine ?? N2.Context.Current; }
            set { engine = value; }
        }

        /// <summary>File extensions that may be selected using this selector.</summary>
        public string SelectableExtensions
        {
            get { return ViewState["SelectableExtensions"] as string; }
            set { ViewState["SelectableExtensions"] = value; }
        }

        public string BrowserUrl { get; set; }
        public string PopupOptions { get; set; }
        public string PreferredSize { get; set; }

        /// <summary>The selected url.</summary>
        public virtual string Url
        {
            get { return Text; }
            set { Text = value; }
        }

        /// <summary>Format for the javascript invoked when the open popup button is clicked.</summary>
        protected virtual string OpenPopupFormat
        {
            get { return "openMediaSelectorPopup('{0}', '{1}', '{2}', '{3}', '{4}');"; }
        }



        /// <summary>Text on the button used to open the popup.</summary>
        public string ButtonText
        {
            get { return (string)ViewState["ButtonText"] ?? "..."; }
            set { ViewState["ButtonText"] = value; }
        }


        protected virtual void RegisterClientScripts()
        {
            Page.JavaScript("$('#" + ClientID + "').n2autocomplete({ selectableExtensions:'" + SelectableExtensions + "' });", 
                ScriptPosition.Bottom, ScriptOptions.DocumentReady | ScriptOptions.ScriptTags);
        }

        /// <summary>Initializes the UrlSelector control.</summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EnsureChildControls();

            Page.JavaScript("{ManagementUrl}/Resources/Js/MediaSelection.js");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            RegisterClientScripts();
        }

        /// <summary>Renders and tag and the open popup window button.</summary>
        public override void RenderEndTag(HtmlTextWriter writer)
        {
            base.RenderEndTag(writer);

            RenderButton(writer);
        }

        /// <summary>Renders the open popup button.</summary>
        private void RenderButton(HtmlTextWriter writer)
        {
            HtmlGenericControl span = new HtmlGenericControl("span");
            Controls.Add(span);
            HtmlInputButton cb = new HtmlInputButton();
            span.Controls.Add(cb);
            HtmlInputButton pb = new HtmlInputButton();
            span.Controls.Add(pb);

            span.Attributes["class"] = "selectorButtons";

            pb.Value = ButtonText;
            pb.Attributes["title"] = Utility.GetGlobalResourceString("UrlSelector", "Select") ?? "Select";
            pb.Attributes["class"] = "popupButton selectorButton";
            pb.Attributes["onclick"] = string.Format(OpenPopupFormat,
                                                      N2.Web.Url.ResolveTokens(BrowserUrl),
                                                      ClientID,
                                                      PopupOptions,
                                                      PreferredSize,
                                                      SelectableExtensions
                                                      );
            cb.Value = "x";
            cb.Attributes["title"] = Utility.GetGlobalResourceString("UrlSelector", "Clear") ?? "Clear";
            cb.Attributes["class"] = "clearButton revealer";
            cb.Attributes["onclick"] = "document.getElementById('" + ClientID + "').value = '';";

            span.RenderControl(writer);
        }


        public static string ImageExtensions = ".jpg,.png,.gif,.jpeg,.ico,.bmp";
		public static string MovieExtensions = ".swf,.mpg,.mpeg,.mp4,.avi,.wmv,.mkv";
		public static string AudioExtensions = ".aif,.m4a,.mid,.mp3,.wav,.wma";
	}
}
