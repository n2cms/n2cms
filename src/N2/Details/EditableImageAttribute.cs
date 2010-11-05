using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Web.UI.WebControls;
using System.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>
	/// Defines an editable image this allows to select an image with the file picker.
	/// </summary>
	/// <example>
	///		public class MyItem : N2.ContentItem
	///		{
	/// 		[N2.Details.EditableImage("Image", 100)]
	/// 		public virtual string ImageUrl
	/// 		{
	/// 			get { return (string)(GetDetail("ImageUrl")); }
	/// 			set { SetDetail("ImageUrl", value); }
	/// 		}
	///		}
	/// </example>
	public class EditableImageAttribute : AbstractEditableAttribute, IDisplayable, IRelativityTransformer
	{
		private string alt = string.Empty;
		private string cssClass = string.Empty;



		public EditableImageAttribute()
			: this(null, 40)
		{
		}

		public EditableImageAttribute(string title, int sortOrder)
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

		protected override Control AddEditor(Control container)
		{
			FileSelector selector = new FileSelector();
			selector.ID = Name;
			container.Controls.Add(selector);
			return selector;
		}
		
		public override void UpdateEditor(ContentItem item, Control editor)
		{
			FileSelector selector = (FileSelector)editor;
			selector.Url = item[Name] as string;
		}

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			FileSelector selector = (FileSelector)editor;
			if (selector.Url != item[Name] as string)
			{
				item[Name] = selector.Url;
				return true;
			}
			return false;
		}

		#region IDisplayable Members
		public Control AddTo(ContentItem item, string detailName, Control container)
		{
			return DisplayableImageAttribute.AddImage(container, item, detailName, CssClass, Alt);
		}

		[Obsolete("Use DisplayableImageAttribute.AddImage")]
		public static Control AddImage(Control container, ContentItem item, string detailName, string cssClass, string altText)
		{
			return DisplayableImageAttribute.AddImage(container, item, detailName, cssClass, altText);
		}
		#endregion

		#region IRelativityTransformer Members

		public RelativityMode RelativeWhen { get; set; }

		string IRelativityTransformer.Rebase(string currentPath, string fromAppPath, string toAppPath)
		{
			return N2.Web.Url.Rebase(currentPath, fromAppPath, toAppPath);
		}

		#endregion
	}
}
