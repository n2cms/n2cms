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
	public class EditableMediaUploadAttribute : EditableMediaAttribute
	{
        public bool UseDefaultUploadDirectory { get; set; }

        public EditableMediaUploadAttribute()
			: this(null, 41)
		{
		}

        public EditableMediaUploadAttribute(string title, int sortOrder = 41)
			: base(title, sortOrder)
		{
			Extensions = MediaSelector.AudioExtensions + "," + MediaSelector.ImageExtensions + "," + MediaSelector.MovieExtensions + "," + MediaSelector.DocumentExtensions;
		}

        protected override Control AddEditor(Control container)
        {
            var composite = (SelectingMediaControl)base.AddEditor(container);

            composite.SelectorControl.UseDefaultUploadDirectory = UseDefaultUploadDirectory;

            return composite;
        }
    }
}