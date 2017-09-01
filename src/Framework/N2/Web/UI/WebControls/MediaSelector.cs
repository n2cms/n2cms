using N2.Resources;
using N2.Definitions;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System;
using N2.Engine;
using System.Linq;
using System.IO;
using N2.Edit;

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
            ThumbnailButton = new HtmlButton();

            ShowThumbnail = false;
        }

		public MediaSelector(string name)
			: this()
		{
			Input.ID = name + "_input";
            ThumbnailButton.ID = Input.ID + "_thumbnail";
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
            set {
                Input.Text = value;

                if (ShowThumbnail)
                {
                    var ext = Path.GetExtension(value);
                    if (!string.IsNullOrWhiteSpace(ext) && ImageExtensions.Split(new[] { ',' }).Contains(ext))
                    {
                        ThumbnailButton.InnerHtml = string.Format("<img src='{0}'/>", value);
                        ThumbnailButton.Attributes["style"] = "";
                    }
                    else
                    {
                        ThumbnailButton.InnerHtml = "";
                        ThumbnailButton.Attributes["style"] = "display:none;";
                    }
                }
            }
        }

        /// <summary>Path to a default upload directory for templated content</summary>
        public bool UseDefaultUploadDirectory { get; set; }

        /// <summary>Format for the javascript invoked when the open popup button is clicked.</summary>
        protected virtual string OpenPopupFormat
        {
            get { return "n2MediaSelection.openMediaSelectorPopup('{0}', '{1}', '{2}', '{3}', '{4}', '{5}'); return false;"; }
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
        public HtmlButton ThumbnailButton { get; private set; }
        public bool ShowThumbnail { get; set; }
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
            
            Controls.Add(ThumbnailButton);
            Controls.Add(ShowButton);
            Controls.Add(Input);
			Controls.Add(ClearButton);
			Controls.Add(Buttons);
			Buttons.Controls.Add(PopupButton);

            ThumbnailButton.Attributes["class"] = "thumbnail";
            ThumbnailButton.Attributes["title"] = Utility.GetGlobalResourceString("UrlSelector", "View") ?? "View";
            ThumbnailButton.Attributes["style"] = "display:none"; //Hide the thumbnail initially. Only show when url is set.

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
		}

		protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            RegisterClientScripts();

            string defaultUploadDirectoryPath = "";
            if (UseDefaultUploadDirectory)
            {
                var selection = new SelectionUtility(this, Context.GetEngine());
                ContentItem item = selection.SelectedItem;
                var start = Find.ClosestOf<IStartPage>(item);
                Type itemType = item.GetContentType();

                defaultUploadDirectoryPath = string.Format("{0}/content/{1}", start.Title.ToLower().Trim().Replace(" ","-"), itemType.Name.ToLower().Trim().Replace(" ", "-"));
            }

            PopupButton.Attributes["onclick"] = string.Format(OpenPopupFormat,
													  N2.Web.Url.ResolveTokens(BrowserUrl ?? Engine.ManagementPaths.MediaBrowserUrl.ToUrl().AppendQuery("mc=true")),
													  Input.ClientID,
													  PopupOptions,
													  PreferredSize,
													  !string.IsNullOrWhiteSpace(SelectableExtensions) ? SelectableExtensions : ImageExtensions,
                                                      defaultUploadDirectoryPath
                                                      );

			ClearButton.Attributes["onclick"] = "n2MediaSelection.clearMediaSelector('" + Input.ClientID + "'); return false;";

            ShowButton.Attributes["onclick"] = ThumbnailButton.Attributes["onclick"] = "n2MediaSelection.showMediaSelectorOverlay('" + Input.ClientID + "'); return false;";
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
