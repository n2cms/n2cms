using System.Web.UI.WebControls;
using N2.Web.UI.WebControls;

namespace N2.Details
{
    /// <summary>
    /// An abstract base class that implements editable drop down functionality.
    /// Override and implement GetListItems to use.
    /// Override and implement AddRequiredFieldValidator to use when Required = true.
    /// </summary>
    public abstract class EditableDropDownWithValidationAttribute : EditableListControlAttribute
    {
        public EditableDropDownWithValidationAttribute(): base() {}

        public EditableDropDownWithValidationAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
			ClientAdapter = "n2autosave.select";
        }

        protected sealed override ListControl CreateEditor()
        {
            return new DropDownListWithValidation();
        }
    }
}
