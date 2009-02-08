using System;
using System.Collections.Generic;
using System.Text;
using N2.Definitions;

namespace N2.Edit.Settings
{
	[Obsolete]
	public class SettingsManager
	{
		private readonly List<IServiceEditable> editableComponents = new List<IServiceEditable>();
		private readonly List<IEditableContainer> containers = new List<IEditableContainer>();
		private readonly AttributeExplorer<IServiceEditable> serviceExplorer;
		private readonly AttributeExplorer<IEditableContainer> editableExplorer;

		public SettingsManager(AttributeExplorer<IServiceEditable> serviceExplorer, AttributeExplorer<IEditableContainer> editableExplorer)
		{
			this.serviceExplorer = serviceExplorer;
			this.editableExplorer = editableExplorer;
		}

		public List<IServiceEditable> EditableComponents
		{
			get { return editableComponents; }
		}

		public List<IEditableContainer> Containers
		{
			get { return containers; }
		}

		public virtual void Handle(string name, Type serviceType)
		{
			AddEditables(serviceType, name);
			AddContainers(serviceType);
		}

		private void AddContainers(Type serviceType)
		{
			foreach (IEditableContainer container in editableExplorer.Find(serviceType))
			{
				if (!Containers.Contains(container))
				{
					Containers.Add(container);
				}
			}
			Containers.Sort();
		}

		private void AddEditables(Type serviceType, string name)
		{
			IList<IServiceEditable> editables = serviceExplorer.Find(serviceType);
			foreach (IServiceEditable editable in editables)
			{
				editable.ServiceName = name;
				EditableComponents.Add(editable);
			}
			EditableComponents.Sort();
		}

		public void Remove(string key)
		{
			for (int i = EditableComponents.Count - 1; i >= 0; --i)
			{
				if (EditableComponents[i].ServiceName == key)
				{
					EditableComponents.RemoveAt(i);
				}
			}
		}
	}
}
