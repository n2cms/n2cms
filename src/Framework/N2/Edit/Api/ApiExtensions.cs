using N2.Collections;
using N2.Edit;
using N2.Engine;
using N2.Persistence.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Management.Api
{
    public static class ApiExtensions
    {
        public static Node<InterfaceMenuItem> FindRecursive(this Node<InterfaceMenuItem> startingPoint, string requestedName)
        {
            if (startingPoint.Current != null && startingPoint.Current.Name == requestedName)
                return startingPoint;
            if (startingPoint.Children != null)
                foreach (var child in startingPoint.Children)
                {
                    var node = FindRecursive(child, requestedName);
                    if (node != null)
                        return node;
                }
            return null;
        }

        public static void Add(this Node<InterfaceMenuItem> startingPoint, string parentName, Node<InterfaceMenuItem> nodeToAdd, bool requireParent = false, int insertIndex = -1, string insertBeforeSiblingWithName = null, string insertAfterSiblingWithName = null)
        {
            var parentNode = FindRecursive(startingPoint, parentName);
            if (parentNode == null && requireParent && parentName != null)
                throw new Exception("Missing parent with name " + parentName);
            if (parentNode == null)
                parentNode = startingPoint;

            var siblings = (parentNode.Children ?? new Node<InterfaceMenuItem>[0]).ToList();

            if (insertBeforeSiblingWithName != null)
                insertIndex = siblings.FindIndex(n => n.Current != null && n.Current.Name == insertBeforeSiblingWithName);
            else if (insertAfterSiblingWithName != null)
            {
                insertIndex = siblings.FindIndex(n => n.Current != null && n.Current.Name == insertAfterSiblingWithName);
                if (insertIndex >= 0)
                    insertIndex++;
            }

            if (insertIndex >= 0)
                siblings.Insert(insertIndex, nodeToAdd);
            else
                siblings.Add(nodeToAdd);

            parentNode.Children = siblings;
        }

        internal static Node<TreeNode> CreateNode(this HierarchyNode<ContentItem> structure, IContentAdapterProvider adapters, Collections.ItemFilter filter)
        {
            var adapter = adapters.ResolveAdapter<NodeAdapter>(structure.Current);

            var children = structure.Children.Select(c => CreateNode(c, adapters, filter)).ToList();
			return new Node<TreeNode>
			{
				Current = adapter.GetTreeNode(structure.Current),
				HasChildren = adapter.HasChildren(new Query { Parent = structure.Current, Filter = filter, Interface = Interfaces.Managing, OnlyPages = true }),
				Expanded = children.Any(),
				Children = children
			};
        }

        internal static HierarchyNode<ContentItem> BuildBranchStructure(ItemFilter filter, IContentAdapterProvider adapters, ContentItem selectedItem, ContentItem root)
        {
            var structure = new BranchHierarchyBuilder(selectedItem, root, true) { UseMasterVersion = false }
                .Children((item) =>
                {
                    var q = new N2.Persistence.Sources.Query { Parent = item, OnlyPages = true, Interface = Interfaces.Managing, Filter = filter };
                    return adapters.ResolveAdapter<NodeAdapter>(item).GetChildren(q);
                })
                .Build();
            return structure;
        }

        internal static HierarchyNode<ContentItem> BuildTreeStructure(ItemFilter filter, IContentAdapterProvider adapters, ContentItem selectedItem, int maxDepth)
        {
            var structure = new TreeHierarchyBuilder(selectedItem, maxDepth)
                .Children((item) =>
                {
                    var q = new N2.Persistence.Sources.Query { Parent = item, OnlyPages = true, Interface = Interfaces.Managing, Filter = filter };
                    return adapters.ResolveAdapter<NodeAdapter>(item).GetChildren(q);
                })
                .Build();
            return structure;
        }

		internal static T TryGet<T>(this IDictionary<string, T> settings, string key)
		{
			if (settings.ContainsKey(key))
				return settings[key];
			return default(T);
		}
    }
}
