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
            public HtmlInputFile Upload = new HtmlInputFile();
            public TextBox ChangeName = new TextBox();

            protected override void OnInit(EventArgs e)
            {
                Upload.ID = "u";
                Controls.Add(Upload);
                ChangeName.ID = "n";
                Controls.Add(ChangeName);

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
                f.PhysicalPath = System.IO.Path.Combine(f.Directory.PhysicalPath, f.Name);
                ce.Upload.PostedFile.SaveAs(f.PhysicalPath);
                return true;
            }
            else if (ce.ChangeName.Text.Length > 0)
            {
                f.Name = ce.ChangeName.Text;
            }
            return false;
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            CompositeEditor ce = editor as CompositeEditor;
            File f = item as File;
            if (!string.IsNullOrEmpty(f.PhysicalPath) && System.IO.File.Exists(f.PhysicalPath))
            {
                ce.Upload.Visible = false;
                ce.ChangeName.Text = f.Name;
            }
            else
            {
                ce.ChangeName.Visible = false;
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
