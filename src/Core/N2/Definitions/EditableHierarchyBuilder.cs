using System;
using System.Collections.Generic;

namespace N2.Definitions
{
	/// <summary>
	/// Adds editables and containers to containers.
	/// </summary>
	/// <typeparam name="T">The type of editable attribute to look for.</typeparam>
	public class EditableHierarchyBuilder<T> where T : IContainable
	{
		/// <summary>Build the container hierarchy adding containers and editors to a root container.</summary>
		/// <param name="containers">The containers to add to themselves or the root container.</param>
		/// <param name="editables">The editables to add to the containers or a root container.</param>
		/// <returns>A new root container.</returns>
		public virtual IEditableContainer Build(IList<IEditableContainer> containers, IList<T> editables)
		{
			IEditableContainer rootContainer = new RootContainer();
			AddContainersToRootContainer(rootContainer, containers);
			AddEditorsToContainers(rootContainer, containers, editables);
			return rootContainer;
		}

		#region Helpers

		private static void AddContainersToRootContainer(IEditableContainer rootContainer, IEnumerable<IEditableContainer> containers)
		{
			foreach (IEditableContainer container in  containers)
			{
				if (container.ContainerName != null)
				{
					if (container.ContainerName == container.Name)
						throw new N2Exception(
							"The container '{0}' cannot reference itself as containing container. Change the ContainerName property.",
							container.Name);
					IEditableContainer parentContainer = FindContainer(container.ContainerName, containers);
					if (parentContainer == null)
						throw new N2Exception(
							"The container '{0}' references another containing container '{1}' that doesn't seem to be defined. Either add a container with this name or remove the reference to that container.",
							container.Name, container.ContainerName);
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

		private static void AddEditorsToContainers(IEditableContainer rootContainer, IEnumerable<IEditableContainer> containers,
		                                    IEnumerable<T> editables)
		{
			foreach (T editable in editables)
			{
				if (editable.ContainerName != null)
				{
					IEditableContainer container = FindContainer(editable.ContainerName, containers);
					if (container == null)
						throw new N2Exception(
							"The editor '{0}' references a container '{1}' that doesn't seem to be defined. Either add a container with this name or remove the reference to that container.",
							editable.Name, editable.ContainerName);
					container.AddContained(editable);
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