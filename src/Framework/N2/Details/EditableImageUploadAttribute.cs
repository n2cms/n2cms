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
	/// Allows to upload or select an image file to use.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableImageUploadAttribute : EditableMediaAttribute
    {
        public bool ShowThumbnail { get; set; }

        /// <summary>Use default upload directory for templated content.</summary>
        public bool UseDefaultUploadDirectory { get; set; }

        public EditableImageUploadAttribute()
			: this(null, 41)
		{
		}

		public EditableImageUploadAttribute(string title, int sortOrder = 41)
			: base(title, sortOrder)
		{
            ShowThumbnail = false;
		}

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            var composite = (SelectingMediaControl)editor;
            composite.SelectorControl.ShowThumbnail = ShowThumbnail;

            base.UpdateEditor(item, editor);
        }


        protected override Control AddEditor(Control container)
        {
            var composite = (SelectingMediaControl)base.AddEditor(container);
            composite.SelectorControl.ShowThumbnail = ShowThumbnail;

            composite.SelectorControl.UseDefaultUploadDirectory = UseDefaultUploadDirectory;
            composite.SelectorControl.InitialPath = GetInitialPath(container);

            return composite;
        }

        /// <summary>
        /// Default initial path is set to empty string. Override this method to set initial path.
        /// </summary>
        protected virtual string GetInitialPath(Control container)
        {
            return "";
        }
    }
}