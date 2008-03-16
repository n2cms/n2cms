using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Web.UI.WebControls;
using System.Web.UI.WebControls;

namespace N2.Details
{
	public class EditableImageAttribute : AbstractEditableAttribute, IDisplayable
	{
		private string alt = string.Empty;
		private string cssClass = string.Empty;


		public EditableImageAttribute()
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
			return AddImage(container, item, detailName, CssClass, Alt);
		}

		public static Control AddImage(Control container, ContentItem item, string detailName, string cssClass, string altText)
		{
			string imageUrl = item[detailName] as string;
			if (!string.IsNullOrEmpty(imageUrl))
			{
				Image image = new Image();
				image.ImageUrl = imageUrl;
				image.AlternateText = altText;
				image.CssClass = cssClass;
				container.Controls.Add(image);
				return image;
			}
			return null;
		}

		#endregion
	}
}
