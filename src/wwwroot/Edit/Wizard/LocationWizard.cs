using System.Collections.Generic;
using N2.Edit.Wizard.Items;
using N2.Persistence;
using N2.Web;

namespace N2.Edit.Wizard
{
	public class LocationWizard
	{
		private Settings wizardSettings;
		private readonly IPersister persister;
		private readonly IHost host;

		public LocationWizard(IPersister persister, IHost host)
		{
			this.persister = persister;
			this.host = host;
			wizardSettings = new Settings("Wizard", string.Empty, "Wizard settings");
		}

		public virtual Settings WizardSettings
		{
			get { return wizardSettings; }
			set { wizardSettings = value; }
		}

		public virtual IList<MagicLocation> GetLocations()
		{
			List<MagicLocation> locations = new List<MagicLocation>();
			foreach (ContentItem child in GetWizardContainer().GetChildren())
			{
				locations.Add(child as MagicLocation);
			}
			return locations;
		}

		public virtual Wonderland GetWizardContainer()
		{
			ContentItem root = persister.Get(host.DefaultSite.RootItemID);
			Wonderland container = root.GetChild(WizardSettings.Name) as Wonderland;
			if(container == null)
			{
				container = new Wonderland();
				container.Parent = root;
				container.Title = WizardSettings.Title;
				container.Name = WizardSettings.Name;
				container.ZoneName = WizardSettings.ZoneName;
				persister.Save(container);
			}
			return container;
		}

        public Items.MagicLocation AddLocation(ContentItem location, string discriminator, string title, string zone)
		{
            ContentItem wonderland = GetWizardContainer();
			Items.MagicLocation ml = Context.Definitions.CreateInstance<Items.MagicLocation>(wonderland);
			ml.Location = location;
			ml.ItemDiscriminator = discriminator;
			ml.Title = title;
			ml.ItemZoneName = zone;
			Context.Persister.Save(ml);
			return ml;
		}
	}
}
