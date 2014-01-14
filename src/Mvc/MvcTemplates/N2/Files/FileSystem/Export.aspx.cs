using System;
using System.Collections.Generic;
using N2.Edit.FileSystem.Items;
using N2.Edit.Web;
using N2.Engine;
using ICSharpCode.SharpZipLib.Zip;

namespace N2.Edit.FileSystem
{
    public partial class Export : EditPage
    {
        private IFileSystem fileSystem;
        private string rootPath;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            fileSystem = Engine.Resolve<IFileSystem>();
            rootPath = Request.QueryString["path"].TrimEnd('/');

            litRootPath.Text = Server.HtmlEncode(rootPath);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var zipFileName = System.IO.Path.GetFileName(rootPath) + ".zip";

            var directory = Directory.New(fileSystem.GetDirectory(rootPath), null, Engine.Resolve<IDependencyInjector>());

            Response.Clear();
            Response.BufferOutput = false;
            Response.ContentType = "application/zip";
            Response.AddHeader("content-disposition", "filename=" + zipFileName);
            
            using (var zip = new ZipOutputStream(Response.OutputStream))
            {
                ExportDirectory(zip, directory);
            }

            Response.End();
        }

        private void ExportDirectory(ZipOutputStream zip, Directory directory)
        {
            foreach (var file in directory.GetFiles())
            {
                zip.PutNextEntry(new ZipEntry(file.Url.Substring(rootPath.Length + 1)));
                fileSystem.ReadFileContents(file.Url, zip);
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
                ExportDirectory(zip, subDirectory);
            }
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (!IsValid) return;

            var imported = new List<string>();

            using (var zip = new ZipInputStream(fupImport.FileContent))
            {
                for (var entry = zip.GetNextEntry(); entry != null; entry = zip.GetNextEntry())
                {
                    if (entry.IsDirectory) continue;
                    var path = rootPath + '/' + entry.Name;
                    fileSystem.WriteFile(path, zip);
                    imported.Add(path);
                }
            }

            rptImportedFiles.DataSource = imported;
            rptImportedFiles.DataBind();

            mvwImport.ActiveViewIndex = 1;

            Refresh(Selection.SelectedItem, ToolbarArea.Navigation);
        }
    }
}
