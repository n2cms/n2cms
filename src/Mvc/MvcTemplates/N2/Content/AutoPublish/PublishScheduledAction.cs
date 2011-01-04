using System;
using System.Configuration;
using N2.Plugin.Scheduling;
using System.Diagnostics;
using N2.Engine;
using System.Net;
using N2.Web;
using N2.Configuration;
using N2.Persistence;
using System.Collections.Generic;
using N2.Persistence.Finder;

namespace N2.Edit.AutoPublish
{
    [ScheduleExecution(30, TimeUnit.Seconds)]
    public class PublishScheduledAction : ScheduledAction
    {
        IVersionManager Versioner { get { return Engine.Resolve<IVersionManager>(); } }
        IPersister Persister { get { return Engine.Resolve<IPersister>(); } }
        IItemFinder Finder { get { return Engine.Resolve<IItemFinder>(); } }

        public override void Execute()
        {
            if (Debugger.IsAttached)
                return;

            var scheduledForAutoPublish = Finder
				.Where.Detail("FuturePublishDate").Lt(DateTime.Now)
				.PreviousVersions(VersionOption.Include).Select();
			for (int i = 0; i < scheduledForAutoPublish.Count; i++)
			{
                // Get the relevant versions
				var scheduledVersion = scheduledForAutoPublish[i];
				var masterVersion = scheduledVersion.VersionOf;
				// Removing the DelayPublishingUntil Date so that it won't get picked up again
                scheduledVersion["FuturePublishDate"] = null;
				if (masterVersion == null)
					Persister.Save(scheduledVersion);
				else
					Versioner.ReplaceVersion(masterVersion, scheduledVersion, true);
            }
        }
    }
}
