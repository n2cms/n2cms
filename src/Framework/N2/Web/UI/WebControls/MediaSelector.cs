using N2.Resources;
using N2.Definitions;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System;
using N2.Engine;

namespace N2.Web.UI.WebControls
{
    /// <summary>An input box that can be updated with the url to a file through a popup window.</summary>
    public class MediaSelector : HtmlGenericControl
    {
        public MediaSelector()
			: base("div")
        {
            PopupOptions = "height=580,width=960,resizable=yes,status=no,scrollbars=yes";

			Input = new TextBox();
			Buttons = new HtmlGenericControl("span");
			ClearButton = new HtmlButton();
			PopupButton = new HtmlButton();
            ShowButton = new HtmlButton();
            UploadButton = new HtmlButton();
		}

		public MediaSelector(string name)
			: this()
		{
			Input.ID = name + "_input";
		}

		/// <summary>File extensions that may be selected using this selector.</summary>
		public string SelectableExtensions { get; set; }

        public string BrowserUrl { get; set; }
        public string PopupOptions { get; set; }
        public string PreferredSize { get; set; }

        /// <summary>The selected url.</summary>
        public virtual string Url
        {
            get { return Input.Text; }
            set { Input.Text = value; }
        }

        /// <summary>Format for the javascript invoked when the open popup button is clicked.</summary>
        protected virtual string OpenPopupFormat
        {
            get { return "n2MediaSelection.openMediaSelectorPopup('{0}', '{1}', '{2}', '{3}', '{4}'); return false;"; }
        }

		private IEngine engine;
		protected IEngine Engine
		{
			get { return engine ?? N2.Context.Current; }
			set { engine = value; }
		}

		/// <summary>Text on the button used to open the popup.</summary>
		public string ButtonText
        {
            get { return (string)ViewState["ButtonText"] ?? "..."; }
            set { ViewState["ButtonText"] = value; }
        }

		public TextBox Input { get; private set; }
		public HtmlButton ClearButton { get; private set; }
        public HtmlButton PopupButton { get; private set; }
        public HtmlButton ShowButton { get; private set; }
		public HtmlButton UploadButton { get; private set; }
        public HtmlGenericControl Buttons { get; private set; }

		protected virtual void RegisterClientScripts()
        {
            Page.JavaScript("n2MediaSelection.initializeEditableMedia();", 
                ScriptPosition.Bottom, ScriptOptions.DocumentReady);
        }

        /// <summary>Initializes the UrlSelector control.</summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EnsureChildControls();

            Page.JavaScript("{ManagementUrl}/Resources/Js/MediaSelection.js");
        }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			Attributes["class"] = "mediaSelector selector input-append";

            Controls.Add(ShowButton);
            Controls.Add(Input);
			Controls.Add(ClearButton);
			Controls.Add(Buttons);
			Buttons.Controls.Add(PopupButton);
            Buttons.Controls.Add(UploadButton);

			ShowButton.InnerHtml = "<b class='fa fa-eye'></b>";
			ShowButton.Attributes["title"] = Utility.GetGlobalResourceString("UrlSelector", "View") ?? "View";
			ShowButton.Attributes["class"] = "revealer showLayoverButton";

			Input.CssClass = "input-xxlarge";

			ClearButton.InnerHtml = "<b class='fa fa-times'></b>";
			ClearButton.Attributes["title"] = Utility.GetGlobalResourceString("UrlSelector", "Clear") ?? "Clear";
			ClearButton.Attributes["class"] = "clearButton revealer";

			Buttons.Attributes["class"] = "selectorButtons";
			
			PopupButton.InnerHtml = ButtonText;
			PopupButton.Attributes["title"] = Utility.GetGlobalResourceString("UrlSelector", "Select") ?? "Select";
			PopupButton.Attributes["class"] = "btn popupButton selectorButton";
			UploadButton.InnerHtml = "<b class='fa fa-upload'></b>";
			UploadButton.Attributes["title"] = Utility.GetGlobalResourceString("UrlSelector", "Upload") ?? "View";
			UploadButton.Attributes["class"] = "btn uploadButton";
		}

		protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            RegisterClientScripts();

            PopupButton.Attributes["onclick"] = string.Format(OpenPopupFormat,
													  N2.Web.Url.ResolveTokens(BrowserUrl ?? Engine.ManagementPaths.MediaBrowserUrl.ToUrl().AppendQuery("mc=true")),
													  Input.ClientID,
													  PopupOptions,
													  PreferredSize,
													  !string.IsNullOrWhiteSpace(SelectableExtensions) ? SelectableExtensions : ImageExtensions
                                                      );

			UploadButton.Attributes["onclick"] = string.Format(OpenPopupFormat,
													  N2.Web.Url.ResolveTokens(BrowserUrl ?? Engine.ManagementPaths.MediaBrowserUrl.ToUrl().AppendQuery("mc=true").AppendQuery("tab=upload")),
													  Input.ClientID,
													  PopupOptions,
													  PreferredSize,
													  !string.IsNullOrWhiteSpace(SelectableExtensions) ? SelectableExtensions : ImageExtensions
													  );

			ClearButton.Attributes["onclick"] = "n2MediaSelection.clearMediaSelector('" + Input.ClientID + "'); return false;";

            ShowButton.Attributes["onclick"] = "n2MediaSelection.showMediaSelectorOverlay('" + Input.ClientID + "'); return false;";
        }

        /// <summary>Renders and tag and the open popup window button.</summary>
   //     public override void RenderEndTag(HtmlTextWriter writer)
   //     {
   //         base.RenderEndTag(writer);

   //         RenderButton(writer);
   //     }

   //     /// <summary>Renders the open popup button.</summary>
   //     private void RenderButton(HtmlTextWriter writer)
   //     {
			//HtmlGenericControl buttons = new HtmlGenericControl("span");
			//Controls.Add(buttons);
   //         var clearButton = new HtmlButton();
   //         buttons.Controls.Add(clearButton);
   //         var popupButton = new HtmlButton();
   //         buttons.Controls.Add(popupButton);

   //         buttons.Attributes["class"] = "selectorButtons";

   //         popupButton.InnerHtml = ButtonText;
   //         popupButton.Attributes["title"] = Utility.GetGlobalResourceString("UrlSelector", "Select") ?? "Select";
   //         popupButton.Attributes["class"] = "btn popupButton selectorButton";
   //         popupButton.Attributes["onclick"] = string.Format(OpenPopupFormat,
   //                                                   N2.Web.Url.ResolveTokens(BrowserUrl),
   //                                                   ClientID,
   //                                                   PopupOptions,
   //                                                   PreferredSize,
   //                                                   SelectableExtensions
   //                                                   );
			//clearButton.InnerHtml = "<b class='fa fa-times'></b>";
   //         clearButton.Attributes["title"] = Utility.GetGlobalResourceString("UrlSelector", "Clear") ?? "Clear";
   //         clearButton.Attributes["class"] = "clearButton revealer";
   //         clearButton.Attributes["onclick"] = "document.getElementById('" + ClientID + "').value = '';";

   //         buttons.RenderControl(writer);
   //     }


        public static string ImageExtensions = ".jpg,.png,.gif,.jpeg,.ico,.bmp";
		public static string MovieExtensions = ".swf,.mpg,.mpeg,.mp4,.avi,.wmv,.mkv";
		public static string AudioExtensions = ".aif,.m4a,.mid,.mp3,.wav,.wma";
	}
}
