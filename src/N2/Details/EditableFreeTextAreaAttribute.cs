using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit;
using N2.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using N2.Web;

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

		public override Control AddTo(Control container)
		{
			HtmlTableCell labelCell = new HtmlTableCell();
			Label label = AddLabel(labelCell);

			HtmlTableCell editorCell = new HtmlTableCell();
			Control editor = AddEditor(editorCell);
			if (label != null && editor != null && !string.IsNullOrEmpty(editor.ID))
				label.AssociatedControlID = editor.ID;

			HtmlTableCell extraCell = new HtmlTableCell();
			if (Required)
				AddRequiredFieldValidator(extraCell, editor);
			if (Validate)
				AddRegularExpressionValidator(extraCell, editor);

			AddHelp(extraCell);

			HtmlTableRow row = new HtmlTableRow();
			row.Cells.Add(labelCell);
			row.Cells.Add(editorCell);
			row.Cells.Add(extraCell);
			
			HtmlTable editorTable = new HtmlTable();
			editorTable.Attributes["class"] = "editDetail";
			editorTable.Controls.Add(row);
			container.Controls.Add(editorTable);
			
			return editor;
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

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            base.UpdateEditor(item, editor);

            FreeTextArea fta = (FreeTextArea)editor;

			if (item is IDocumentBaseSource)
				fta.DocumentBaseUrl = (item as IDocumentBaseSource).BaseUrl;
        }
	}
}
