using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using NHibernate.Event;
using N2.Edit.Versioning;

namespace N2.Persistence.NH
{
	[Service(typeof(ConfigurationBuilderParticipator))]
	public class RelationConfigurationBuilderParticipator : ConfigurationBuilderParticipator
	{
		class RelationInsertEventListener : IPostLoadEventListener
		{
			public void OnPostLoad(PostLoadEvent @event)
			{
                InitialializeRelations(@event.Entity as ContentItem, @event.Session);
                InitialializeRelations(@event.Entity as ContentVersion, @event.Session);
			}

            private void InitialializeRelations(ContentVersion version, IEventSource session)
            {
                if (version == null)
                    return;
                version.AssociatedVersion.ValueAccessor = session.Get<ContentItem>;
                version.MasterVersion.ValueAccessor = session.Get<ContentItem>;
            }

			private void InitialializeRelations(ContentItem item, IEventSource session)
			{
				if (item == null)
					return;
				item.VersionOf.ValueAccessor = session.Get<ContentItem>;
			}
		}

		public override void AlterConfiguration(NHibernate.Cfg.Configuration cfg)
		{
			cfg.AppendListeners(NHibernate.Event.ListenerType.PostLoad, new[] { new RelationInsertEventListener() });
		}
	}
}
