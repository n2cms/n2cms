using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>Attribute used to mark properties as editable. This attribute is predefined to use the <see cref="N2.Web.UI.WebControls.FreeTextArea"/> web control as editor.</summary>
	/// <example>
	/// [N2.Details.EditableFreeTextArea("Text", 110)]
    /// public virtual string Text
    /// {
	///     get { return (string)GetDetail("Text"); }
    ///		set { SetDetail("Text", value); }
	/// }
	/// </example>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableFreeTextAreaAttribute : EditableTextBoxAttribute
	{
		public EditableFreeTextAreaAttribute(string title, int sortOrder) 
			: base(title, sortOrder)
		{
		}

		protected override void ModifyEditor(TextBox tb)
		{
			// do nothing
		}

		protected override TextBox CreateEditor()
		{
			return new FreeTextArea();
		}

		protected override Control AddRequiredFieldValidator(Control container, Control editor)
		{
			RequiredFieldValidator rfv = base.AddRequiredFieldValidator(container, editor) as RequiredFieldValidator;
			rfv.EnableClientScript = false;
			return rfv;
		}
	}
}
