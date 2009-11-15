using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

namespace N2.Edit.Navigation
{
    internal class SearchControlAttribute : ToolbarPluginAttribute
    {
        public SearchControlAttribute()
            : base("Search", "search", "Navigation/Search.aspx", ToolbarArea.Options)
        {
            GlobalResourceClassName = "Toolbar";
        }

        public override System.Web.UI.Control AddTo(System.Web.UI.Control container, PluginContext context)
        {
            Literal l = new Literal();
            l.Text = string.Format(@"
<form target='navigation' method='get' action='Navigation/Search.aspx'>
    <input type='text' name='query' class='tb'/>
    <input type='submit' value='{0}' name='submit' class='s'/>
</form>", Utility.GetResourceString(GlobalResourceClassName, Name + ".Title") ?? Title);
            container.Controls.Add(l);

            return l;
        }
    }
}
