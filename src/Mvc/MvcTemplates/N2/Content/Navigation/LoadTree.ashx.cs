using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI;
using N2.Collections;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Web.UI.WebControls;
using N2.Web;
using System.Web.Mvc;
using N2.Management.Content.Navigation;

namespace N2.Edit.Navigation
{
    public class LoadTree : HelpfulHandler
    {
        public override void ProcessRequest(HttpContext context)
        {
            string target = context.Request["target"] ?? Targets.Preview;

            var selection = new SelectionUtility(context, N2.Context.Current);
            ContentItem selectedItem = selection.SelectedItem;
            
            context.Response.ContentType = "text/plain";

            ItemFilter filter = Engine.EditManager.GetEditorFilter(context.User);
            IContentAdapterProvider adapters = Engine.Resolve<IContentAdapterProvider>();
            var node = new TreeHierarchyBuilder(selectedItem, 2)
                .Children((item) => adapters.ResolveAdapter<NodeAdapter>(item).GetChildren(item, Interfaces.Managing))
                .Build();

            string selectableTypes = context.Request["selectableTypes"];
            string selectableExtensions = context.Request["selectableExtensions"];

            TreeUtility.Write(node, selectedItem, adapters, filter, selectableTypes, selectableExtensions, excludeRoot: true, target: target, writer: context.Response.Output);
        }
    }
}
