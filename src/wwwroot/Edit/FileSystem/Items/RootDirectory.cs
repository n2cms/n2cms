using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Collections;
using System.Collections.Generic;
using N2.Web;
using System.IO;
using N2.Integrity;
using N2.Details;
using N2.Edit.Trash;
using N2.Definitions;
using N2.Installation;

namespace N2.Edit.FileSystem.Items
{
    [Definition("File Folder", SortOrder = 600, Installer = InstallerHint.NeverRootOrStartPage)]
    [RestrictParents(typeof(IFileSystemContainer))]
    [ItemAuthorizedRoles("Administrators", "admin")]
    [Editables.EditableFolderPath]
    public class RootDirectory : AbstractDirectory
    {
        public RootDirectory()
        {
            Visible = false;
            SortOrder = 10000;
        }

        private string physicalPath = null;
        public override string PhysicalPath
        {
            get 
            {
                if (physicalPath == null)
                {
                    Url u = N2.Web.Url.Parse("~/");
                    physicalPath = GetWebContext().MapPath(u.AppendSegment(Name, "")); ;
                }
                return physicalPath;
            }
            set { physicalPath = value;}
        }

        private IWebContext GetWebContext()
        {
            return N2.Context.Current.Resolve<IWebContext>();
        }
    }
}
