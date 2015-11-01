using System;
using System.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>
	/// Defines an editable image this allows to select an image with the file picker.
	/// </summary>
	/// <example>
	///     public class MyItem : N2.ContentItem
	///     {
	///         [N2.Details.EditableImage]
	///         public virtual string ImageUrl { get; set; }
	///     }
	/// </example>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableImageAttribute : EditableMediaAttribute
	{
		public EditableImageAttribute()
			: this(null, 40)
		{
		}

		public EditableImageAttribute(string title, int sortOrder = 40)
			: base(title, sortOrder)
		{
		}
	}
}
//		AbstractEditableAttribute, IRelativityTransformer, IWritingDisplayable
//    {
//        private string alt = string.Empty;
//        private string cssClass = string.Empty;

//        public EditableImageAttribute()
//            : this(null, 40)
//        {
//        }

//        public EditableImageAttribute(string title, int sortOrder)
//            : base(title, sortOrder)
//        {
//	        ReadOnly = false;
//        }

//	    /// <summary>Image alt text.</summary>
//        public string Alt
//        {
//            get { return alt; }
//            set { alt = value; }
//        }

//        /// <summary>CSS class on the image element.</summary>
//        public string CssClass
//        {
//            get { return cssClass; }
//            set { cssClass = value; }
//        }

//        /// <summary>The image size to display by default if available.</summary>
//        public string PreferredSize { get; set; }

//	    public bool ReadOnly { get; set; }

//	    protected override Control AddEditor(Control container)
//        {
//            var selector = new FileSelector();
//            selector.SelectableExtensions = FileSelector.ImageExtensions;
//            selector.ID = Name;
//            selector.Placeholder(GetLocalizedText("Placeholder") ?? Placeholder);
//			if (ReadOnly)
//				selector.Attributes["readonly"] = "readonly";
//            container.Controls.Add(selector);
//            return selector;
//        }
        
//        public override void UpdateEditor(ContentItem item, Control editor)
//        {
//            var selector = (FileSelector)editor;
//            selector.Url = item[Name] as string;
//        }

//        public override bool UpdateItem(ContentItem item, Control editor)
//        {
//            var selector = (FileSelector)editor;
//            if (selector.Url != item[Name] as string)
//            {
//                item[Name] = selector.Url;
//                return true;
//            }
//            return false;
//        }

//        #region IDisplayable Members
//        public override Control AddTo(ContentItem item, string detailName, Control container)
//        {
//            return DisplayableImageAttribute.AddImage(container, item, detailName, PreferredSize, CssClass, Alt);
//        }

//        [Obsolete("Use DisplayableImageAttribute.AddImage")]
//        public static Control AddImage(Control container, ContentItem item, string detailName, string cssClass, string altText)
//        {
//            return DisplayableImageAttribute.AddImage(container, item, detailName, null, cssClass, altText);
//        }
//        #endregion

//        #region IRelativityTransformer Members

//        public RelativityMode RelativeWhen { get; set; }

//        string IRelativityTransformer.Rebase(string currentPath, string fromAppPath, string toAppPath)
//        {
//// ReSharper disable once RedundantNameQualifier
//            return N2.Web.Url.Rebase(currentPath, fromAppPath, toAppPath);
//        }

//        #endregion

//        #region IWritingDisplayable Members

//        public void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
//        {
//            var sizes = DisplayableImageAttribute.GetSizes(PreferredSize);
//            DisplayableImageAttribute.WriteImage(item, propertyName, sizes, alt, CssClass, writer);
//        }

//        #endregion
//    }
//}
