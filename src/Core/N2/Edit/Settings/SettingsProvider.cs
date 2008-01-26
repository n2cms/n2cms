using System.Collections.Generic;
using N2.Definitions;

namespace N2.Edit.Settings
{
	public class SettingsProvider : ISettingsProvider
	{
		private IList<IServiceEditable> settings;
		private IEditableContainer rootContainer;

		public SettingsProvider(SettingsManager manager, EditableHierarchyBuilder<IServiceEditable> hierarchyBuilder)
		{
			settings = manager.EditableComponents;
			rootContainer = hierarchyBuilder.Build(manager.Containers, manager.EditableComponents);
		}

		public IList<IServiceEditable> Settings
		{
			get { return settings; }
			set { settings = value; }
		}

		public IEditableContainer RootContainer
		{
			get { return rootContainer; }
			set { rootContainer = value; }
		}
	}
}
