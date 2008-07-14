using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using N2.Edit.Web;

namespace N2.Edit.FileSystem
{
    public partial class File1 : EditPage
    {
        protected Items.File SelectedFile
        {
            get { return SelectedItem as Items.File; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = SelectedItem.Title;
        }
    }
}
