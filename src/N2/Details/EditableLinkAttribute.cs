using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>
	/// Defines an editable link to another item on this site. The item is 
	/// selected through a popup window displaying the item tree.
	/// </summary>
	/// <example>
	///		[EditableLink("Feed root", 90)]
	///		public virtual ContentItem FeedRoot
	///		{
	/// 		get { return (ContentItem)GetDetail("FeedRoot"); }
	/// 		set { SetDetail("FeedRoot", value); }
	///		}
	/// </example>
	public class EditableLinkAttribute : AbstractEditableAttribute, IDisplayable, IRelativityTransformer
	{
		public EditableLinkAttribute()
			: this(null, 100)
		{
		}

		public EditableLinkAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
			RelativeWhen = RelativityMode.ExportRelativeImportAbsolute;
		}

		protected override Control AddEditor(Control container)
		{
			ItemSelector selector = new ItemSelector();
			selector.ID = Name;
			container.Controls.Add(selector);
			return selector;
		}
		
		public override void UpdateEditor(ContentItem item, Control editor)
		{
			ItemSelector selector = (ItemSelector)editor;
			selector.SelectedItem = item[Name] as ContentItem;
		}
		
		public override bool UpdateItem(ContentItem item, Control editor)
		{
			ItemSelector selector = (ItemSelector)editor;
			if (selector.SelectedItem != item[Name] as ContentItem)
			{
				item[Name] = selector.SelectedItem;
				return true;
			}
			return false;
		}
		
		#region IDisplayable Members
		public Control AddTo(ContentItem item, string detailName, Control container)
		{
			ContentItem itemToAdd = item[detailName] as ContentItem;
			if (itemToAdd != null)
			{
				if (itemToAdd.IsPage)
					return DisplayableAnchorAttribute.AddAnchor(container, itemToAdd);
				else
					return Web.UI.ItemUtility.AddUserControl(container, itemToAdd);
			}
			return null;
		}
		#endregion

		#region IRelativityTransformer Members

		public RelativityMode RelativeWhen { get; set; }

		string IRelativityTransformer.ToAbsolute(string applicationPath, string value)
		{
			return N2.Web.Url.ToAbsolute(applicationPath, value);
		}

		string IRelativityTransformer.ToRelative(string applicationPath, string value)
		{
			return N2.Web.Url.ToRelative(applicationPath, value);
		}

		#endregion
	}
}
