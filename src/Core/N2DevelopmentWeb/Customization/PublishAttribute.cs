using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using N2.Web.UI.WebControls;
using N2.Web.UI;

namespace N2.TemplateWeb.Customization
{
	public class PublishAttribute : N2.Details.EditableCheckBox
	{
		public PublishAttribute(string checkBoxText, int sortOrder)
			: base(checkBoxText, sortOrder)
		{
		}

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			CheckBox cb = editor as CheckBox;
			item.Published = cb.Checked ?
				(DateTime?)(item.Published ?? DateTime.Now) :
				null;
			return true;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			CheckBox cb = editor as CheckBox;
			cb.Checked = item.Published.HasValue;
		}

		protected override Control CreateEditor(Control container)
		{
			CheckBox cb = base.CreateEditor(container) as CheckBox;
			cb.CheckedChanged += new EventHandler(OnCheckedChanged);
			cb.Unload += new EventHandler(OnUnload);
			return cb;
		}

		/// <summary>Update the item editors saving mode to save changes as a version.</summary>
		protected virtual void OnCheckedChanged(object sender, EventArgs e)
		{
			CheckBox cb = sender as CheckBox;
			IItemEditor editor = ItemUtility.FindInParents<N2.Web.UI.WebControls.IItemEditor>(cb);
			if(!cb.Checked && editor.CurrentItem.ID > 0 && editor.CurrentItem.VersionOf == null)
				editor.VersioningMode = ItemEditorVersioningMode.VersionOnly;
		}

		void OnUnload(object sender, EventArgs e)
		{
			CheckBox cb = sender as CheckBox;
			cb.CheckedChanged -= new EventHandler(OnCheckedChanged);
			cb.Unload -= new EventHandler(OnUnload);
		}

	}
}
