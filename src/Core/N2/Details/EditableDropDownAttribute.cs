using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace N2.Details
{
	/// <summary>
	/// An abstract base class that implements editable drop down functionality.
	/// Override and implement GetListItems to use.
	/// </summary>
	public abstract class EditableDropDownAttribute : AbstractEditableAttribute
	{
		public EditableDropDownAttribute()
		{
		}

		public EditableDropDownAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			DropDownList ddl = editor as DropDownList;
			if (!ddl.SelectedValue.Equals(item[Name] as string, StringComparison.InvariantCultureIgnoreCase))
			{
				item[Name] = ddl.SelectedValue;
				return true;
			}
			return false;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			DropDownList ddl = editor as DropDownList;
			if (item[Name] != null)
			{
				ddl.SelectedValue = item[Name] as string;
			}
		}

		protected override Control AddEditor(Control container)
		{
			DropDownList ddl = new DropDownList();
			if (!Required)
				ddl.Items.Add(new ListItem());

			ddl.Items.AddRange(GetListItems());
			container.Controls.Add(ddl);
			return ddl;
		}

		protected abstract ListItem[] GetListItems();
	}
}
