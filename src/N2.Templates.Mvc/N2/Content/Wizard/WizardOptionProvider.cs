using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Engine;
using N2.Edit;
using N2.Definitions;

namespace N2.Edit.Wizard
{
	[Service]
	public class WizardOptionProvider : IProvider<ToolbarOption>
	{
		LocationWizard wizard;
		IEditUrlManager editUrlManager;
		IDefinitionManager definitions;

		public WizardOptionProvider(LocationWizard wizard, IEditUrlManager editUrlManager, IDefinitionManager definitions)
		{
			this.wizard = wizard;
			this.editUrlManager = editUrlManager;
			this.definitions = definitions;
		}

		#region IProvider<ToolbarOption> Members

		public ToolbarOption Get()
		{
			return GetAll().FirstOrDefault();
		}

		public IEnumerable<ToolbarOption> GetAll()
		{
			return wizard.GetLocations()
				.Where(m => m.Location != null)
				.Select((m, i) => new ToolbarOption
				{
					Title = m.Title,
					Target = Targets.Preview,
					SortOrder = i,
					Name = m.Name,
					Url = editUrlManager.GetEditNewPageUrl(m.Location, m.GetDefinition(definitions), m.ZoneName, CreationPosition.Below) + "&template=" + m.ContentTemplate
				});
		}

		#endregion
	}
}
