using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using N2.Collections;
using N2.Web;
using N2.Web.UI.WebControls;
using N2.Engine;
using N2.Edit.Workflow;
using System.Web.Mvc;
using N2.Management.Content.Navigation;

namespace N2.Edit.Web.UI.Controls
{
    public class Tree : Control
    {
        ContentItem selectedtItem = null;
        ContentItem rootItem = null;
        ItemFilter filter = null;
        string target = Targets.Preview;
        IEngine engine;

        public Tree()
        {
        }

        public string SelectableExtensions { get; set; }

        public string SelectableTypes { get; set; }

        private static IEditUrlManager ManagementPaths
        {
            get { return N2.Context.Current.ManagementPaths; }
        }

        public HierarchyNode<ContentItem> Nodes { get; set; }

        public ContentItem SelectedItem
        {
            get { return selectedtItem ?? (selectedtItem = Find.CurrentPage ?? Find.StartPage); }
            set { selectedtItem = value; }
        }

        public ContentItem RootNode
        {
            get { return rootItem ?? Find.RootItem; }
            set { rootItem = value; }
        }

        public ItemFilter Filter
        {
            get { return filter ?? N2.Context.Current.EditManager.GetEditorFilter(Page.User); }
            set { filter = value; }
        }

        public string Target
        {
            get { return target; }
            set { target = value; }
        }

        public IEngine Engine
        {
            get { return engine ?? (engine = N2.Context.Current); }
            set { engine = value; }
        }

        public override void DataBind()
        {
            EnsureChildControls();
            base.DataBind();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            IContentAdapterProvider adapters = Engine.Resolve<IContentAdapterProvider>();

            if (Nodes == null)
                Nodes = new BranchHierarchyBuilder(SelectedItem, RootNode, true) { UseMasterVersion = false }
                    .Children((item) => adapters.ResolveAdapter<NodeAdapter>(item).GetChildren(item, Interfaces.Managing).Where(Filter))
                    .Build();

            TreeUtility.Write(Nodes, SelectedItem, adapters, Filter, SelectableTypes, SelectableExtensions, excludeRoot: false, target: Target, writer: writer);
        }
    }
}
