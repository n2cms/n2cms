using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Web.UI.WebControls;
using N2.Details;

namespace N2.Templates.Details
{
    [AttributeUsage(AttributeTargets.Property)]
    [Obsolete("Use N2.Details.EditableThemeSelectionAttribute")]
    public class ThemeSelectorAttribute : EditableDropDownAttribute
    {
        public ThemeSelectorAttribute(string title, int sortOrder)
            :base(title, sortOrder)
        {
        }

        protected override ListItem[] GetListItems()
        {
            List<ListItem> items = new List<ListItem>();
            items.Add(new ListItem());

            string path = HostingEnvironment.MapPath("~/App_Themes/");
            
            if(Directory.Exists(path)) {
                foreach(string directoryPath in Directory.GetDirectories(path))
                {
                    string directoryName = Path.GetFileName(directoryPath);
                    if (!directoryName.StartsWith(".") && !directoryName.StartsWith("_"))
                        items.Add(new ListItem(directoryName));
                }
            }

            return items.ToArray();
        }
    }
}
