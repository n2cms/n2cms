using N2.Web.Drawing;
using N2.Web.UI.WebControls;
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Details
{
    /// <summary>
    /// Allows to upload or select an media file using the new Media Browser.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableMediaAttribute : AbstractEditableAttribute, IRelativityTransformer, IWritingDisplayable, IDisplayable
    {
        private string alt = string.Empty;
        private string cssClass = string.Empty;
        private string uploadDirectory = string.Empty;
        private string extensions = string.Empty;

        public EditableMediaAttribute()
            : this(null, 41)
        {
        }

        public EditableMediaAttribute(string title, int sortOrder = 41)
            : base(title, sortOrder)
        {
		}

		/// <summary>Image alt text.</summary>
		public string Alt
        {
            get { return alt; }
            set { alt = value; }
        }

        /// <summary>CSS class on the image element.</summary>
        public string CssClass
        {
            get { return cssClass; }
            set { cssClass = value; }
        }

        /// <summary>Set a specific upload directory.</summary>
        public string UploadDirectory
        {
            get { return uploadDirectory; }
            set { uploadDirectory = value; }
        }

        /// <summary>CSS class on the image element.</summary>
        public string UploadText { get; set; }

        /// <summary>The image size to display by default if available.</summary>
        public string PreferredSize { get; set; }

        /// <summary>A comma separated list of extensions: ie '.jpg,.png'; if blank image extensions are used.</summary>
        public string Extensions
        {
            get { return extensions; }
            set { extensions = value; }
        }
		
		/// <summary>The height of the media element.</summary>
		public string Height { get; set; }

		/// <summary>The width of the media element.</summary>
		public string Width { get; set; }

		/// <summary>The background color of a media element.</summary>
		public string BackgroundColor { get; set; }

		/// <summary>One of <see cref="ImagesUtility.ExtensionGroups"/></summary>
		public string ExtensionGroup { get; set; }

		public override bool UpdateItem(ContentItem item, Control editor)
        {
            var composite = (SelectingMediaControl)editor;
            
            if (composite.SelectorControl.Url != item[Name] as string)
            {
                item[Name] = composite.SelectorControl.Url;
                return true;
            }

            return false;
        }

        public bool ReadOnly { get; set; }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            var composite = (SelectingMediaControl)editor;
            composite.Select(item[Name] as string);
        }


        protected override Control AddEditor(Control container)
        {
            var composite = new SelectingMediaControl(Name);
            composite.SelectorControl.Placeholder(GetLocalizedText("Placeholder") ?? Placeholder);
            composite.SelectorControl.SelectableExtensions = Extensions;
            composite.SelectorControl.PreferredSize = PreferredSize;

            if (ReadOnly)
                composite.SelectorControl.Attributes["readonly"] = "readonly";

            if(!string.IsNullOrEmpty(PreferredSize))
                composite.SelectorControl.Attributes["preferredSize"] = PreferredSize;

            container.Controls.Add(composite);
            return composite;
        }

        /// <summary>Adds a required field validator.</summary>
        /// <param name="container">The container control for this validator.</param>
        /// <param name="editor">The editor control to validate.</param>
        protected override Control AddRequiredFieldValidator(Control container, Control editor)
        {
            var composite = (SelectingMediaControl)editor;
            if (composite == null) return null;
            var rfv = new RequiredFieldValidator();
            rfv.ID = Name + "_rfv";
            rfv.ControlToValidate = composite.SelectorControl.Input.ID;
            rfv.Display = ValidatorDisplay.Dynamic;
            rfv.Text = GetLocalizedText("RequiredText") ?? RequiredText;
            rfv.ErrorMessage = GetLocalizedText("RequiredMessage") ?? RequiredMessage;
            editor.Controls.Add(rfv);

            return rfv;
        }

        #region IDisplayable Members

        public override Control AddTo(ContentItem item, string detailName, Control container)
        {
            using (var sw = new StringWriter())
            {
                Write(item, detailName, sw);

                LiteralControl lc = new LiteralControl(sw.ToString());
                container.Controls.Add(lc);
                return lc;
            }
        }

        #endregion

        #region IRelativityTransformer Members

        public RelativityMode RelativeWhen { get; set; }

        string IRelativityTransformer.Rebase(string currentPath, string fromAppPath, string toAppPath)
        {
            return N2.Web.Url.Rebase(currentPath, fromAppPath, toAppPath);
        }

        #endregion

        #region IWritingDisplayable Members

        public virtual void Write(ContentItem item, string propertyName, TextWriter writer)
        {
            string url = item[propertyName] as string;
            if (string.IsNullOrEmpty(url))
                return;

            switch (ExtensionGroup ?? ImagesUtility.GetExtensionGroup(VirtualPathUtility.GetExtension(url)))
            {
				case ImagesUtility.ExtensionGroups.Flash:
					WriteFlash(url, writer);
					return;
				case ImagesUtility.ExtensionGroups.Video:
					WriteMovie(url, writer);
					return;
				case ImagesUtility.ExtensionGroups.Audio:
					WriteAudio(url, writer);
					return;
				case ImagesUtility.ExtensionGroups.ClientCode:
				case ImagesUtility.ExtensionGroups.Compressed:
				case ImagesUtility.ExtensionGroups.Excel:
				case ImagesUtility.ExtensionGroups.Pdf:
				case ImagesUtility.ExtensionGroups.ServerCode:
				case ImagesUtility.ExtensionGroups.Text:
				case ImagesUtility.ExtensionGroups.Word:
					WriteUrl(item, propertyName, CssClass, writer, url);
					return;
				case ImagesUtility.ExtensionGroups.Images:
				default:
					var sizes = DisplayableImageAttribute.GetSizes(PreferredSize);
					DisplayableImageAttribute.WriteImage(item, propertyName, sizes, alt, CssClass, writer);
					return;
            }
        }

		static string flashFormat = @"<object classid=""clsid:d27cdb6e-ae6d-11cf-96b8-444553540000"" width=""{1}"" height=""{2}"" codebase=""http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,115,0"">
		    <param name=""src"" value=""{0}""/>
		    <param name=""bgcolor"" value=""{3}""/>
		    <param name=""quality"" value=""best""/>
		    <embed name=""whatever"" src=""{0}"" width=""{1}"" height=""{2}"" bgcolor=""{3}"" quality=""best"" pluginspage=""http://www.adobe.com/shockwave/download/index.cgi?P1_Prod_Version=ShockwaveFlash""></embed>
		</object>
		";
		private void WriteFlash(string url, TextWriter writer)
		{
			writer.Write(string.Format(flashFormat, url, Width, Height, BackgroundColor));
		}

		static string movieFormat = @"<object width=""{1}"" height=""{2}"" classid=""CLSID:22D6f312-B0F6-11D0-94AB-0080C74C7E95"" standby=""Loading Windows Media Player components..."" type=""application/x-oleobject"" codebase=""http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=6,4,7,1112"">
		    <param name=""filename"" value=""{0}"">
		    <param name=""Showcontrols"" value=""True"">
		    <param name=""autoStart"" value=""True"">
		    <embed type=""application/x-mplayer2"" src=""{0}"" name=""MediaPlayer"" width=""{1}"" height=""{2}""></embed>
		</object>";

		private void WriteMovie(string url, TextWriter writer)
		{
			writer.Write(string.Format(movieFormat, url, Width, Height, BackgroundColor));
		}

		static string audioFormat = @"<object width=""{1}"" height=""{2}"" classid=""CLSID:22D6f312-B0F6-11D0-94AB-0080C74C7E95"" standby=""Loading Windows Media Player components..."" type=""application/x-oleobject"" codebase=""http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=6,4,7,1112"">
		    <param name=""filename"" value=""{0}"">
		    <param name=""Showcontrols"" value=""True"">
		    <param name=""autoStart"" value=""True"">
		    <embed type=""application/x-mplayer2"" src=""{0}"" name=""MediaPlayer"" width=""{1}"" height=""{2}""></embed>
		</object>";

		private void WriteAudio(string url, TextWriter writer)
		{
			writer.Write(string.Format(audioFormat, url, Width, Height, BackgroundColor));
		}

		private static void WriteUrl(ContentItem item, string propertyName, string cssClass, TextWriter writer, string url)
        {
            cssClass = item[propertyName + "_CssClass"] as string ?? cssClass;
			string fileName = "";
			try
			{
				if (!url.Contains("//"))
					fileName = VirtualPathUtility.GetFileName(url);
            }
			catch (Exception)
			{
			}

            if (string.IsNullOrEmpty(cssClass))
                writer.Write(string.Format("<a href=\"{0}\">{1}</a>", url, fileName));
            else
                writer.Write(string.Format("<a href=\"{0}\" class=\"{1}\">{2}</a>", url, cssClass, fileName));
        }

        #endregion

        public const string ValidCharactersExpression = "^[^+]*$";
        public const string InvalidCharactersExpression = "[+]";
    }
}

