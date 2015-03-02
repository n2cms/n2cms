using System;
using System.Diagnostics;
using System.Linq;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Plugin.Scheduling;
using N2.Security;
using N2.Edit.Workflow;
using N2.Edit.Versioning;
using N2.Engine;

namespace N2.Edit.AutoPublish
{
    [Service]
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
            VersioningExtensions.MarkForFuturePublishing(changer, item, futureDate);
        }

        public override void Execute()
        {
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
            var scheduledForAutoPublish = versionRepository.GetVersionsScheduledForPublish(Utility.CurrentTime()).ToList();
            foreach (var version in scheduledForAutoPublish)
            {
                var scheduledVersion = versionRepository.DeserializeVersion(version);
                scheduledVersion["FuturePublishDate"] = null;
                versioner.Publish(persister, scheduledVersion);
            }
          }
    }
}
