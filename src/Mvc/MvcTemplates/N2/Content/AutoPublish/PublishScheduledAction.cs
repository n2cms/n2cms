using System;
using System.Diagnostics;
using System.Linq;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Plugin.Scheduling;
using N2.Security;
using N2.Edit.Workflow;
using N2.Edit.Versioning;

namespace N2.Edit.AutoPublish
{
    [ScheduleExecution(30, TimeUnit.Seconds)]
    public class PublishScheduledAction : ScheduledAction
    {
		private ContentVersionRepository versionRepository;
		private IVersionManager versioner;
		private IPersister persister;
		private ISecurityManager security;
		private StateChanger changer;
		
		public PublishScheduledAction(ContentVersionRepository versionRepository, IVersionManager versioner, IPersister persister, ISecurityManager security, StateChanger changer)
		{
			this.versionRepository = versionRepository;
			this.versioner = versioner;
			this.persister = persister;
			this.security = security;
			this.changer = changer;
		}

		public virtual void MarkForFuturePublishing(ContentItem item, DateTime futureDate)
		{
			if (!item.VersionOf.HasValue)
				item.Published = futureDate;
			else
				item["FuturePublishDate"] = futureDate;
			changer.ChangeTo(item, ContentState.Waiting);

		}

        public override void Execute()
        {
			//if (Debugger.IsAttached)
			//    return;
			
			using (security.Disable())
			{
				PublishPendingVersions();

				ChangeStateToItemsBecomePublished();

				ChangeStateToExpiredItems();
			}
        }

		private void ChangeStateToExpiredItems()
		{
			var implicitExpire = persister.Repository
				.Find(Parameter.LessOrEqual("Expires", Utility.CurrentTime())
					& Parameter.Equal("State", ContentState.Published))
				.ToList();
			for (int i = 0; i < implicitExpire.Count; i++)
			{
				// reset status on expired items
				var item = implicitExpire[i];
				changer.ChangeTo(item, ContentState.Unpublished);
				persister.Save(item);
			}
		}

		private void ChangeStateToItemsBecomePublished()
		{
			var implicitAutoPublish = persister.Repository
				.Find(Parameter.LessThan("Published", Utility.CurrentTime())
					& Parameter.Equal("State", ContentState.Waiting))
				.ToList();
			for (int i = 0; i < implicitAutoPublish.Count; i++)
			{
				// saving the master version for auto-publish will be eventually become published without this, but we want to update the state
				var item = implicitAutoPublish[i];
				changer.ChangeTo(item, ContentState.Published);
				persister.Save(item);
			}
		}

		private void PublishPendingVersions()
		{
			var scheduledForAutoPublish = persister.Repository
				.Find(Parameter.LessOrEqual("FuturePublishDate", Utility.CurrentTime()).Detail())
				.ToList();
			for (int i = 0; i < scheduledForAutoPublish.Count; i++)
			{
				// Get the relevant versions
				ContentItem scheduledVersion = scheduledForAutoPublish[i];
				ContentItem masterVersion = scheduledVersion.VersionOf;
				// Removing the DelayPublishingUntil Date so that it won't get picked up again
				scheduledVersion["FuturePublishDate"] = null;

				if (masterVersion == null)
					persister.Save(scheduledVersion);
				else
					versioner.ReplaceVersion(masterVersion, scheduledVersion, true);
			}

            //var implicitAutoPublish = Finder
            //    .Where.Published.Le(Utility.CurrentTime())
            //    .And.State.Eq(ContentState.Waiting)
            //    .Select();
            var implicitAutoPublish = persister.Repository
                .Find(Parameter.LessOrEqual("Published", Utility.CurrentTime()) & Parameter.Equal("State", ContentState.Waiting))
                .ToList();
			for (int i = 0; i < implicitAutoPublish.Count; i++)
			{
				try
				{
					security.ScopeEnabled = false;
				    // saving the master version for auto-publish will be eventually become published without this, but we want to update the state
				    var item = implicitAutoPublish[i];
				    item.State = ContentState.Published;
				    persister.Save(item);
            }
            finally
            {
                security.ScopeEnabled = true;
            }
          }
		}
	}
}
