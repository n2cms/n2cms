using N2.Integrity;
using N2.Definitions;
using N2.Installation;
using N2.Web;

namespace N2.Edit.FileSystem.Items
{
    [PageDefinition("File Folder", 
		Description = "A node that maps to files in the file system.",
		SortOrder = 600, 
		InstallerVisibility = InstallerHint.NeverRootOrStartPage,
        IconUrl = "|Management|/Resources/icons/folder.png",
		TemplateUrl = "|Management|/Files/FileSystem/Directory.aspx")]
    [RestrictParents(typeof(IFileSystemContainer))]
    [ItemAuthorizedRoles("Administrators", "admin")]
    [Editables.EditableFolderPath]
	[Template("info", "|Management|/Files/FileSystem/Directory.aspx")]
    public class RootDirectory : AbstractDirectory
    {
        public RootDirectory()
			: base(N2.Context.Current.Resolve<IFileSystem>())
        {
            Visible = false;
            SortOrder = 10000;
        }
    }
}
