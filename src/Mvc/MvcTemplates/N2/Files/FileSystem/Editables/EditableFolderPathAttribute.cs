using System.Configuration;
using System.Web.UI.WebControls;
using N2.Configuration;
using N2.Details;

namespace N2.Edit.FileSystem.Editables
{
    public class EditableFolderPathAttribute : EditableDropDownAttribute
    {
        public EditableFolderPathAttribute()
            : base("Upload Root Folder", 10)
        {
            Name = "Name";
            Required = true;
        }

        protected override ListItem[] GetListItems()
        {
            EditSection config = ConfigurationManager.GetSection("n2/edit") as EditSection;
            if (config == null)
                return new ListItem[] { new ListItem("~/Upload") };
            else
            {
                ListItem[] items = new ListItem[config.UploadFolders.Count];
                for (int i = 0; i < items.Length; i++)
                {
                    string path = config.UploadFolders[i].Path;
                    items[i] = new ListItem(path.Trim('/', '~', '.'));
                }
                return items;
            }
        }
    }
}
