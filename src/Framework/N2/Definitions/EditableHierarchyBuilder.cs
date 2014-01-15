using System;
using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using N2.Configuration;
using N2.Engine;
using N2.Security;

namespace N2.Definitions
{
    /// <summary>
    /// Adds editables and containers to containers.
    /// </summary>
    [Service]
    public class EditableHierarchyBuilder
    {
        ISecurityManager security;
        string defaultContainerName;
        
        public EditableHierarchyBuilder(ISecurityManager security, EngineSection config)
        {
            this.security = security;
            this.defaultContainerName = config.Definitions.DefaultContainerName;
        }

        /// <summary>Build the container hierarchy adding containers and editors to a root container.</summary>
        /// <param name="containers">The containers to add to themselves or the root container.</param>
        /// <param name="editables">The editables to add to the containers or a root container.</param>
        /// <returns>A new root container.</returns>
        public virtual HierarchyNode<IContainable> Build(IEnumerable<IContainable> containers, IEnumerable<IContainable> editables)
        {
            HierarchyNode<IContainable> root = new HierarchyNode<IContainable>(new RootContainer());
            var containerNodes = AddContainersToRootContainer(root, containers);
            AddEditorsToContainers(root, containerNodes, editables, defaultContainerName);
            SortChildrenRecursive(root);
            return root;
        }

        private void SortChildrenRecursive(HierarchyNode<IContainable> root)
        {
            root.Children = root.Children.OrderBy(c => c.Current.SortOrder).ToList();
            foreach (var c in root.Children)
                SortChildrenRecursive(c);
        }

        #region Helpers

        private IEnumerable<HierarchyNode<IContainable>> AddContainersToRootContainer(HierarchyNode<IContainable> rootContainer, IEnumerable<IContainable> containersToProcess)
        {
            var addedContainers = new List<HierarchyNode<IContainable>>(new HierarchyNode<IContainable>[] { rootContainer });
            var queue = new Queue<IContainable>(containersToProcess);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.ContainerName == current.Name) throw new N2Exception("The container '{0}' cannot reference itself as containing container. Change the ContainerName property.", current.Name);

                string containerName = current.ContainerName;
                var container = addedContainers.FirstOrDefault(c => c.Current.Name == containerName);
                if (container != null)
                {
                    // the container was previously in the loop
                    var node = new HierarchyNode<IContainable>(current);
                    container.Add(node);
                    addedContainers.Add(node);
                }
                else if (!queue.Any(c => c.Name == containerName))
                {
                    // no container - add to root
                    var node = new HierarchyNode<IContainable>(current);
                    rootContainer.Add(node);
                    addedContainers.Add(node);
                }
                else
                    // the container is in the queue - add this to end
                    queue.Enqueue(current);
            }
            return addedContainers;
        }

        private static void AddEditorsToContainers<T>(HierarchyNode<IContainable> rootContainer, IEnumerable<HierarchyNode<IContainable>> containers, IEnumerable<T> editables, string defaultContainerName) where T : IContainable
        {
            foreach (T editable in editables)
            {
                string containerName = editable.ContainerName ?? defaultContainerName;
                var container = containers.FirstOrDefault(c => string.Equals(c.Current.Name, containerName, StringComparison.InvariantCultureIgnoreCase));
                if (container != null)
                    container.Add(new HierarchyNode<IContainable>(editable));
                else
                    rootContainer.Add(new HierarchyNode<IContainable>(editable));
            }
        }

        #endregion
    }
}
