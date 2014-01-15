using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Details;
using N2.Edit.FileSystem.Items;

namespace N2.Edit.FileSystem.Editables
{
    public class EditableUploadAttribute : AbstractEditableAttribute
    {
        public EditableUploadAttribute()
        {
            Title = "Upload";
            Name = "Name";
        }
        private class CompositeEditor : Control
        {
            public HtmlInputFile Upload = new HtmlInputFile { ID = "u" };
            public TextBox ChangeName = new TextBox { ID = "n" };
            public RegularExpressionValidator ChangeNameExpressionValidator = new RegularExpressionValidator { ControlToValidate = "n", ValidationExpression = EditableFileUploadAttribute.ValidCharactersExpression, Display = ValidatorDisplay.Dynamic, ErrorMessage = "Invalid characters in file name" };
            public RequiredFieldValidator ChangeNameRequiredValidator = new RequiredFieldValidator { ControlToValidate = "n", Display = ValidatorDisplay.Dynamic, ErrorMessage = "File name required" };

            protected override void OnInit(EventArgs e)
            {
                Controls.Add(Upload);
                Controls.Add(ChangeName);
                Controls.Add(ChangeNameExpressionValidator);
                Controls.Add(ChangeNameRequiredValidator);

                base.OnInit(e);
            }
        }

        public override bool UpdateItem(ContentItem item, Control editor)
        {
            CompositeEditor ce = editor as CompositeEditor;
            File f = item as File;
            if (ce.Upload.PostedFile != null && ce.Upload.PostedFile.ContentLength > 0)
            {
                f.Name = System.IO.Path.GetFileName(ce.Upload.PostedFile.FileName);
                f.WriteToDisk(ce.Upload.PostedFile.InputStream);

                return true;
            }
            if (ce.ChangeName.Text.Length > 0)
            {
                f.NewName = ce.ChangeName.Text;
                return true;
            }
            return false;
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            CompositeEditor ce = editor as CompositeEditor;
            File f = item as File;

            if (f.Exists)
            {
                ce.Upload.Visible = false;
                ce.ChangeName.Text = f.Name;
            }
            else
            {
                ce.ChangeName.Visible = false;
                ce.ChangeNameExpressionValidator.Enabled = false;
                ce.ChangeNameRequiredValidator.Enabled = false;
            }
        }

        protected override Control AddEditor(Control container)
        {
            CompositeEditor editor = new CompositeEditor();
            container.Controls.Add(editor);
            return editor;
        }
    }
}
