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

            var allAutoPublishPages = Finder.Where.Detail("FuturePublishDate").Lt(DateTime.Now).Select();
            foreach (var page in allAutoPublishPages)
            {
                var allVersions = Versioner.GetVersionsOf(page);

                // Getting the item wich was created last
                int newVersion = page.ID;
                DateTime latestDateTime = page.Updated;
                foreach (ContentItem item in allVersions)
                {
                    if (item.Updated > latestDateTime)
                    {
                        latestDateTime = item.Updated;
                        newVersion = item.ID;
                    }
                }

                // Get the relevant versions
                N2.ContentItem latestVersion = Persister.Get(newVersion);
                N2.ContentItem masterVersion = Persister.Get(page.ID);

                // Removing the DelayPublishingUntil Date so that it won't get picked up again
                latestVersion["FuturePublishDate"] = null;
                masterVersion["FuturePublishDate"] = null;
                Versioner.ReplaceVersion(masterVersion, latestVersion);

                // Get rid of all the "FuturePublishDate" dates so that it won't get called again
                // There might be a few hanging around because new version get saved
                foreach (ContentItem item in allVersions)
                {
                    N2.ContentItem version = Persister.Get(item.ID);
                    version["FuturePublishDate"] = null;
                    Persister.Save(latestVersion);
                }
            }
        }
    }
}
