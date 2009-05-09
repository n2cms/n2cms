using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace N2.Details
{
	/// <summary>
	/// An abstract base class that implements editable list functionality.
	/// Override and implement GetListItems to use.
	/// Implement a CreateEditor() method to instantiate a desired editor control.
	/// </summary>
	public abstract class EditableListControlAttribute : AbstractEditableAttribute, IDisplayable
	{
		public EditableListControlAttribute(): base() { }

		public EditableListControlAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			ListControl ddl = editor as ListControl;
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
        protected virtual object GetValue(ListControl ddl)
        {
            return ddl.SelectedValue;
        }

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			ListControl ddl = editor as ListControl;
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
		
		protected abstract ListControl CreateEditor();
		 
		protected override Control AddEditor(Control container)
		{
			ListControl ddl = this.CreateEditor();
            ddl.ID = Name;
			if (!Required)
				ddl.Items.Add(new ListItem());

			ddl.Items.AddRange(GetListItems());
			container.Controls.Add(ddl);
			return ddl;
		}

		protected abstract ListItem[] GetListItems();
		#region IDisplayable Members

		Control IDisplayable.AddTo(ContentItem item, string detailName, Control container)
		{
			string value = item[Name] as string;
			if (!string.IsNullOrEmpty(value))
			{
				foreach (ListItem li in GetListItems())
				{
					if (li.Value == value)
					{
						Literal l = new Literal();
						l.Text = li.Text;
						container.Controls.Add(l);
						return l;
					}
				}
			}
			return null;
		}

		#endregion

	}
}
