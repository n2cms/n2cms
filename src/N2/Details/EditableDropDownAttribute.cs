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
			if (!ddl.SelectedValue.Equals(GetValue(item), StringComparison.InvariantCultureIgnoreCase))
			{
                item[Name] = GetValue(ddl);
				return true;
			}
			return false;
		}

        /// <summary>Gets the object to store as content from the drop down list editor.</summary>
        /// <param name="ddl">The editor.</param>
        /// <returns>The value to store.</returns>
        protected virtual object GetValue(DropDownList ddl)
        {
            return ddl.SelectedValue;
        }

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			DropDownList ddl = editor as DropDownList;
			if (item[Name] != null)
			{
                ddl.SelectedValue = GetValue(item);
			}
        }

        /// <summary>Gets a string value from the drop down list editor from the content item.</summary>
        /// <param name="item">The item containing the value.</param>
        /// <returns>A string to use as selected value.</returns>
        protected virtual string GetValue(ContentItem item)
        {
            return item[Name] as string;
        }

		protected override Control AddEditor(Control container)
		{
			DropDownList ddl = new DropDownList();
            ddl.ID = Name;
			if (!Required)
				ddl.Items.Add(new ListItem());

			ddl.Items.AddRange(GetListItems());
			container.Controls.Add(ddl);
			return ddl;
		}

		protected abstract ListItem[] GetListItems();
	}
}
