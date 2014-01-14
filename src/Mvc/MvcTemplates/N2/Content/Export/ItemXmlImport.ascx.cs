using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Persistence.Serialization;
using System.IO;
using N2.Xml;
using N2.Engine;
using N2.Edit.Web;
using N2.Edit;
using System.Text;
using N2.Edit.FileSystem;

namespace N2.Management.Content.Export
{
    public partial class ItemXmlImport : EditUserControl
    {
        public string UploadedFilePath
        {
            get { return (string)(ViewState["UploadedFilePath"] ?? ""); }
            set { ViewState["UploadedFilePath"] = value; }
        }

        public void ContinueWithImport(string tempFile)
        {
            UploadedFilePath = tempFile;

            try
            {
                IImportRecord record = Engine.Resolve<Importer>().Read(UploadedFilePath);
                importedItems.CurrentItem = record.RootItem;
                rptAttachments.DataSource = record.Attachments;
                if (Selection.SelectedItem.Children.FindNamed(record.RootItem.Name) != null)
                {
                    pnlNewName.Visible = true;
                    txtNewName.Text = record.RootItem.Name;
                }
                ShowErrors(record);
            }
            catch (WrongVersionException)
            {
                using (Stream s = File.OpenRead(UploadedFilePath))
                {
                    N2XmlReader xr = new N2XmlReader(N2.Context.Current);
                    importedItems.CurrentItem = xr.Read(s);
                }
            }

            DataBind();
        }

        protected void btnImportUploaded_Click(object sender, EventArgs e)
        {
            Importer importer = Engine.Resolve<Importer>();

            IImportRecord record;
            try
            {
                record = importer.Read(UploadedFilePath);
                ShowErrors(record);
            }
            catch (WrongVersionException)
            {
                N2XmlReader xr = new N2XmlReader(N2.Context.Current);
                ContentItem item = xr.Read(File.OpenRead(UploadedFilePath));
                record = CreateRecord(item);
            }

            Import(importer, record);
        }

        private static IImportRecord CreateRecord(ContentItem item)
        {
            ReadingJournal rj = new ReadingJournal();
            rj.Report(item);
            return rj;
        }

        private void Import(Importer importer, IImportRecord record)
        {
            try
            {
                if (pnlNewName.Visible)
                {
                    record.RootItem.Name = txtNewName.Text;
                }

                if (chkSkipRoot.Checked)
                {
                    importer.Import(record, Selection.SelectedItem, ImportOption.Children);
                    Refresh(Selection.SelectedItem, ToolbarArea.Both);
                }
                else
                {
                    importer.Import(record, Selection.SelectedItem, ImportOption.All);
                    Refresh(record.RootItem, ToolbarArea.Both);
                }

                ShowErrors(record);
            }
            catch (N2Exception ex)
            {
                cvImport.ErrorMessage = ex.Message;
                cvImport.IsValid = false;
                btnImportUploaded.Enabled = false;
            }
            finally
            {
                if (File.Exists(UploadedFilePath))
                    File.Delete(UploadedFilePath);
            }
        }

        void ShowErrors(IImportRecord record)
        {
            if (record.Errors.Count > 0)
            {
                StringBuilder errorText = new StringBuilder("<ul>");
                foreach (Exception ex in record.Errors)
                {
                    errorText.Append("<li>").Append(ex.Message).Append("</li>");
                }
                errorText.Append("</ul>");

                cvImport.IsValid = false;
                cvImport.ErrorMessage = errorText.ToString();
            }
        }

        internal void ImportNow(HttpPostedFile postedFile)
        {
            Importer importer = Engine.Resolve<Importer>();

            IImportRecord record;
            try
            {
                record = importer.Read(postedFile.InputStream, postedFile.FileName);
            }
            catch (WrongVersionException)
            {
                N2XmlReader xr = new N2XmlReader(N2.Context.Current);
                ContentItem item = xr.Read(postedFile.InputStream);
                record = CreateRecord(item);
                ShowErrors(record);
            }

            Import(importer, record);
        }

        protected string CheckExists(string url)
        {
            if (Engine.Resolve<IFileSystem>().FileExists(url))
                return "(existing file will be overwritten)";
            return string.Empty;
        }
    }
}
