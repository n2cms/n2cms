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
	public abstract class EditableDropDownAttribute : EditableListControlAttribute
	{
		public EditableDropDownAttribute(): base() {}

		public EditableDropDownAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		protected sealed override ListControl CreateEditor()
		{
			return new DropDownList();
		}
	}
}
