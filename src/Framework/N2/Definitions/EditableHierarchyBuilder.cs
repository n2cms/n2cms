using System;
using System.Collections.Generic;
using N2.Engine;

namespace N2.Definitions
{
	/// <summary>
	/// Adds editables and containers to containers.
	/// </summary>
	[Service]
	public class EditableHierarchyBuilder
	{
		/// <summary>Build the container hierarchy adding containers and editors to a root container.</summary>
		/// <param name="containers">The containers to add to themselves or the root container.</param>
		/// <param name="editables">The editables to add to the containers or a root container.</param>
		/// <returns>A new root container.</returns>
		public virtual IEditableContainer Build<T>(IList<IEditableContainer> containers, IList<T> editables, string defaultContainerName) where T : IContainable
		{
			IEditableContainer rootContainer = new RootContainer();
			ClearContainers(containers);
			AddContainersToRootContainer(rootContainer, containers);
			AddEditorsToContainers(rootContainer, containers, editables, defaultContainerName);
			return rootContainer;
		}

		#region Helpers

		private void ClearContainers(IList<IEditableContainer> containers)
		{
			foreach (IEditableContainer container in containers)
				container.ClearContained();
		}

		private static void AddContainersToRootContainer(IEditableContainer rootContainer, IEnumerable<IEditableContainer> containers)
		{
			foreach (IEditableContainer container in  containers)
			{
				if (container.ContainerName != null)
				{
					if (container.ContainerName == container.Name)
						throw new N2Exception("The container '{0}' cannot reference itself as containing container. Change the ContainerName property.", container.Name);

					IEditableContainer parentContainer = FindContainer(container.ContainerName, containers);

					if (parentContainer == null)
						rootContainer.AddContained(container);
					else
						parentContainer.AddContained(container);
				}
				else
				{
					rootContainer.AddContained(container);
				}
			}
		}

		private static IEditableContainer FindContainer(string name, IEnumerable<IEditableContainer> containers)
		{
			foreach (IEditableContainer container in containers)
			{
				if (container.Name == name)
				{
					return container;
				}
			}
			return null;
		}

		private static void AddEditorsToContainers<T>(IEditableContainer rootContainer, IEnumerable<IEditableContainer> containers, IEnumerable<T> editables, string defaultContainerName) where T : IContainable
		{
			foreach (T editable in editables)
			{
				if (editable.ContainerName != null)
				{
					IEditableContainer container = FindContainer(editable.ContainerName, containers)
						?? FindContainer(defaultContainerName, containers);

					if (container != null)
						container.AddContained(editable);
					else
						rootContainer.AddContained(editable);
				}
				else
				{
					rootContainer.AddContained(editable);
				}
			}
		}

		#endregion
	}
}