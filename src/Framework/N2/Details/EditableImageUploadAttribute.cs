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
	public class EditableImageUploadAttribute : EditableFileUploadAttribute, IRelativityTransformer, IWritingDisplayable, IDisplayable
	{
		public EditableImageUploadAttribute()
			: this(null, 41)
		{
		}

		public EditableImageUploadAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		protected override Control AddEditor(Control container)
		{
			SelectingUploadCompositeControl control = (SelectingUploadCompositeControl)base.AddEditor(container);
			control.SelectorControl.SelectableExtensions = FileSelector.ImageExtensions;
			control.SelectorControl.SelectableTypes = typeof(N2.Definitions.IFileSystemFile).Name;
            control.SelectorControl.Placeholder(GetLocalizedText("Placeholder") ?? Placeholder);
			return control;
		}
	}
}
