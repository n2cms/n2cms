using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using NHibernate.Event;

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
