using System.Web;
using N2.Edit.Activity;
using N2.Management.Activity;

namespace N2.Edit.Navigation
{
    public class SortDown : HelpfulHandler
    {
        public override void ProcessRequest(HttpContext context)
        {
            ContentItem selectedNode = GetSelectedItem(context.Request.QueryString);
            Engine.Resolve<ITreeSorter>().MoveDown(selectedNode);

            Engine.AddActivity(new ManagementActivity { Operation = "Sort", PerformedBy = context.User.Identity.Name, Path = selectedNode.Path, ID = selectedNode.ID });

            context.Response.Redirect("Tree.aspx?" + SelectionUtility.SelectedQueryKey + "=" + HttpUtility.UrlEncode(selectedNode.Path));
        }
    }
}
