using System;
using N2.Edit;
using N2.Edit.Web;
using N2.Collections;

namespace N2.Addons.MyAddon.Plugins
{
    /// <summary>
    /// This is a plugin that is accessible through once you log in. It can link to 
    /// any page but the usal pattern is to put the attribute on the page that is 
    /// displayed.
    /// </summary>
    [ToolbarPlugin( // The toolbar plugin attribute tells N2 to put an icon in the toolbar
        "", // we don't want any text (only icon)
        "myPlugin", // this is a string that uniquely identifies the plugin
        "~/Addons/MyAddon/Plugins/MyPlugin.aspx?selected={selected}", // this is the url to the page that represents our plugin, {selected} is replaced by the path of the selected item
        ToolbarArea.Preview, // we want to put the icon the right hand toolbar area
        "preview", // this is the preview frame
        "~/Addons/MyAddon/Plugins/plugin.png", // the icon to show in the toolbar
        1000)] // the order in the toolbar
    public partial class MyPlugin : EditPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(SelectedItem != null)
            {
                h1.InnerHtml = SelectedItem.Title;

                int numberOfParts = Engine.Resolve<Services.MyComponent>().MyParts().Count();
                p.InnerHtml = "There are " + numberOfParts + " of MyParts in the system";

                int numberOfPartsBelowMe = new ItemList(Find.EnumerateChildren(SelectedItem), new TypeFilter(typeof (Items.MyPart))).Count;
                p.InnerHtml += " and " + numberOfPartsBelowMe + " MyParts below this page.";
            }
        }
    }
}
