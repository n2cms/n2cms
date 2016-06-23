using System.Collections.Generic;
using System.Linq;
using N2.Definitions;
using N2.Engine;
using N2.Edit.Api;

namespace N2.Edit.Wizard
{
    [Service]
	[Service(typeof(ITemplateInfoProvider))]
	public class WizardOptionProvider : IProvider<ToolbarOption>, ITemplateInfoProvider
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
			return GetLocations()
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

		public string Area
		{
			get { return "Wizard"; }
		}

		public IEnumerable<TemplateInfo> GetTemplates()
		{
			return GetLocations()
				.Select(ml => new { Location = ml, Definition = ml.GetDefinition(definitions) })
				.Select(ml => new TemplateInfo(ml.Definition) 
				{
					Title = ml.Location.Title, 
					Description = ml.Location.Description, 
					EditUrl = editUrlManager.GetEditNewPageUrl(ml.Location, ml.Definition, ml.Location.ZoneName) + "&template=" + ml.Location.ContentTemplate
				});
		}

		private IEnumerable<Items.MagicLocation> GetLocations()
		{
			return wizard.GetLocations()
				.Where(m => m.Location != null);
		}

	}
}
