using System.Web.UI.WebControls;
using N2.Web;

namespace N2.Edit.Navigation
{
    internal class SearchControlAttribute : ToolbarPluginAttribute
    {
        public SearchControlAttribute()
            : base("Search", "search", "Navigation/Search.aspx", ToolbarArea.Options)
        {
            GlobalResourceClassName = "Toolbar";
            Legacy = true;
        }

        public override System.Web.UI.Control AddTo(System.Web.UI.Control container, PluginContext context)
        {
            Literal l = new Literal();
            l.Text = string.Format(@"
<form target='navigation' method='get' action='{1}'>
    <input type='text' name='query' class='tb' value='{0}' onfocus='if(this.value==""{0}""){{this.value=""""}}' onblur='if(this.value==""""){{this.value=""{0}"";}}'/>
    <button type='submit' name='submit' class='s'>{0}</button>
</form>", Utility.GetResourceString(GlobalResourceClassName, Name + ".Title") ?? Title, Url.ResolveTokens("{ManagementUrl}/Content/Navigation/Search.aspx"));
            container.Controls.Add(l);

            return l;
        }
    }
}
