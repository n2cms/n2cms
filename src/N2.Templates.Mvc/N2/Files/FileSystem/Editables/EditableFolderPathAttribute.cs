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
using N2.Configuration;

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
