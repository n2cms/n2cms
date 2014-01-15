using System;
using N2.Security;
using N2.Web;

namespace N2.Edit
{
    [NavigationLinkPlugin("Paste", "paste", "{ManagementUrl}/Content/Paste.aspx?{Selection.SelectedQueryKey}={selected}&memory={memory}&action={action}", Targets.Preview, "{ManagementUrl}/Resources/icons/page_paste.png", 60,
        GlobalResourceClassName = "Navigation",
        RequiredPermission = Permission.Publish,
        Legacy = true)]
    [ToolbarPlugin("", "paste_tool", "{ManagementUrl}/Content/Paste.aspx?{Selection.SelectedQueryKey}={selected}&memory={memory}&action={action}", ToolbarArea.Operations, Targets.Preview, "{ManagementUrl}/Resources/icons/page_paste.png", 50, ToolTip = "paste", 
        GlobalResourceClassName = "Toolbar",
        RequiredPermission = Permission.Publish,
        Legacy = true)] 
    public partial class paste : Web.EditPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string selected = Request.QueryString[SelectionUtility.SelectedQueryKey];
            string memory = Request.QueryString["memory"];
            string action = Request.QueryString["action"];
            if (string.IsNullOrEmpty(memory) || string.IsNullOrEmpty(action))
            {
                Title = "You must select what to paste and click on the appropriate action first.";
            }
            else
            {
                string url = string.Format("{{ManagementUrl}}/Content/{0}.aspx?" + SelectionUtility.SelectedQueryKey + "={1}&memory={2}", action, Server.UrlEncode(selected), Server.UrlEncode(memory));
                Response.Redirect(Url.ResolveTokens(url));
            }
        }
    }
}
