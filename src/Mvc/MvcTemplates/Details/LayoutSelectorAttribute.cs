using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Hosting;
using N2.Details;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using System.IO;

namespace N2.Templates.Details
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LayoutSelectorAttribute : DropDownAttribute
    {
        public LayoutSelectorAttribute(string title, int sortOrder)
            :base(title, null, sortOrder)
        {
        }

        protected override IEnumerable<ListItem> GetListItems(Control container)
        {
            string path = HostingEnvironment.MapPath("~/Layouts/");

            foreach(string file in Directory.GetFiles(path, "*.Master"))
            {
                string url = "~/Layouts/" + Path.GetFileName(file);
                string title = Path.GetFileNameWithoutExtension(file);
                yield return new ListItem(title, url);
            }
        }
    }
}
