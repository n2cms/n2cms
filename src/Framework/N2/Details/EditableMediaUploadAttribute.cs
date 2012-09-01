using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using N2.Edit.FileSystem;
using N2.Web;
using N2.Web.UI.WebControls;
using System.Text.RegularExpressions;
using N2.Web.Drawing;
using System;

namespace N2.Details
{
	/// <summary>
	/// Allows to upload or select a media file to use.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableMediaUploadAttribute : EditableFileUploadAttribute, IRelativityTransformer, IWritingDisplayable, IDisplayable
	{
		public EditableMediaUploadAttribute()
			: this(null, 41)
		{
		}

		public EditableMediaUploadAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
			Height = "480px";
			Width = "100%";
			BackgroundColor = "#FFFFFF";
		}

		/// <summary>The height of the media element.</summary>
		public string Height { get; set; }
		
		/// <summary>The width of the media element.</summary>
		public string Width { get; set; }

		/// <summary>The background color of a media element.</summary>
		public string BackgroundColor { get; set; }

		protected override Control AddEditor(Control container)
		{
			SelectingUploadCompositeControl control = (SelectingUploadCompositeControl)base.AddEditor(container);
			control.SelectorControl.SelectableExtensions = FileSelector.AudioExtensions + "," + FileSelector.ImageExtensions + "," + FileSelector.MovieExtensions;
			control.SelectorControl.SelectableTypes = typeof(N2.Definitions.IFileSystemFile).Name;
            control.SelectorControl.Placeholder(GetLocalizedText("Placeholder") ?? Placeholder);
			return control;
		}

		public override void Write(ContentItem item, string propertyName, TextWriter writer)
		{
			string url = item[propertyName] as string;
			if (string.IsNullOrEmpty(url))
				return;

			string extension = VirtualPathUtility.GetExtension(url);
			switch (ImagesUtility.GetExtensionGroup(extension))
			{
				case ImagesUtility.ExtensionGroups.Images:
					base.Write(item, propertyName, writer);
					return;
				case ImagesUtility.ExtensionGroups.Flash:
					WriteFlash(url, writer);
					return;
				case ImagesUtility.ExtensionGroups.Video:
					WriteMovie(url, writer);
					return;
				case ImagesUtility.ExtensionGroups.Audio:
					WriteAudio(url, writer);
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
	}
}
