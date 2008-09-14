using N2.Web;
using N2.Integrity;
using N2.Definitions;
using N2.Installation;

namespace N2.Edit.FileSystem.Items
{
    [Definition("File Folder", "RootDirectory", "A node that maps to files in the file system.", "", 600, Installer = InstallerHint.NeverRootOrStartPage)]
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
