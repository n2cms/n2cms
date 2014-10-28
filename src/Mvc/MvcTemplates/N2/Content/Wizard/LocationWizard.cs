using System.Linq;
using System.Collections.Generic;
using N2.Edit.Wizard.Items;
using N2.Engine;
using N2.Persistence;
using N2.Web;

namespace N2.Edit.Wizard
{
    [Service]
    public class LocationWizard
    {
        private Settings wizardSettings;
        private readonly IPersister persister;
        private readonly ContainerRepository<Wonderland> containers;

        public LocationWizard(IPersister persister, IHost host, ContainerRepository<Wonderland> containers)
        {
            this.persister = persister;
            this.containers = containers;
            wizardSettings = new Settings("Wizard", string.Empty, "Wizard settings");
        }

        public virtual Settings WizardSettings
        {
            get { return wizardSettings; }
            set { wizardSettings = value; }
        }

        public virtual IEnumerable<MagicLocation> GetLocations()
        {
            var container = containers.GetBelowRoot();
            if (container == null)
                yield break;

			foreach (var child in container.Children.WhereAccessible().OfType<MagicLocation>())
            {
                if (child.Location == null || child.Location.State == ContentState.Deleted)
                    continue;
                yield return child as MagicLocation;
            }
        }

        public Items.MagicLocation AddLocation(ContentItem location, string discriminator, string templateKey, string title, string zone)
        {
            ContentItem wonderland = containers.GetOrCreateBelowRoot((container) =>
            {
                container.Title = WizardSettings.Title;
                container.Name = WizardSettings.Name;
                container.ZoneName = WizardSettings.ZoneName;
            });
            Items.MagicLocation ml = Context.Current.Resolve<ContentActivator>().CreateInstance<Items.MagicLocation>(wonderland);
            ml.Location = location;
            ml.ItemDiscriminator = discriminator;
            ml.ContentTemplate = templateKey;
            ml.Title = title;
            ml.ItemZoneName = zone;
            persister.Save(ml);
            return ml;
        }

        public void RemoveLocation(int id)
        {
            var location = persister.Get<MagicLocation>(id);
            if (location != null)
                persister.Delete(location);
        }
    }
}
