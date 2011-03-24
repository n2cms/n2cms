using System;
using System.Linq;
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
	public abstract class EditableListControlAttribute : AbstractEditableAttribute, IDisplayable, IWritingDisplayable
	{
		public EditableListControlAttribute(): base() { }

		public EditableListControlAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			ListControl ddl = editor as ListControl;
			string current = GetValue(item);
			object value = GetValue(ddl);
			if (!value.Equals(current))
			{
				if (value != null && value.Equals(item[Name]))
					item[Name] = null;
				else
					item[Name] = value;
				return true;
			}
			else if (current != null && current.Equals(DefaultValue))
			{
				item[Name] = value;
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
            return (item[Name] ?? DefaultValue) as string;
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

		#region IWritingDisplayable Members

		public void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
		{
			var selected = item[propertyName] as string;
			if (selected != null)
				writer.Write(GetListItems().Where(li => li.Value == selected).Select(li => li.Text).FirstOrDefault());
		}

		#endregion
	}
}
