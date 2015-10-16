using System.Web.UI.WebControls;

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
			ClientAdapter = "n2autosave.select";
        }

        protected sealed override ListControl CreateEditor()
        {
            return new DropDownList();
        }
    }
}
