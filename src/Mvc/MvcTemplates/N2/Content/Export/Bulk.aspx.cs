using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data;
using N2.Edit.Web;
using N2.Persistence;
using System.IO;

namespace N2.Management.Content.Export
{
    public partial class Bulk : EditPage
    {
        public void UploadCommand(object sender, CommandEventArgs args)
        {
            var tfs = Engine.Resolve<TemporaryFileHelper>();
            if(string.IsNullOrEmpty(fuImportFile.PostedFile.FileName))
            {
                rfvImportFile.IsValid = false;
                return;
            }

            var rows = new CsvParser(',').Parse(new StreamReader(fuImportFile.PostedFile.InputStream)).ToList();
            var data = rows.Select(r => new { a = r[0], b = r[1], c = r[2], d = r[3] }).ToList();
            dgrTEst.DataSource = data;
            dgrTEst.DataBind();
        }
    }
}