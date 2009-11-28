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

namespace N2.Edit.AutoPublish
{
    [ScheduleExecution(30, TimeUnit.Seconds)]
    public class PublishScheduledAction : ScheduledAction
    {
        private IVersionManager versioner;

        public PublishScheduledAction()
        {
            versioner = N2.Context.Current.Resolve<N2.Persistence.IVersionManager>();
        }

        public override void Execute()
        {
            var allAutoPublishPages = N2.Find.Items.Where.Detail("FuturePublishDate").Lt(DateTime.Now).Select();
            foreach (var page in allAutoPublishPages)
            {
                var allVersions = versioner.GetVersionsOf(page);

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
                N2.ContentItem latestVersion = N2.Engine.Singleton<IEngine>.Instance.Persister.Get(newVersion);
                N2.ContentItem masterVersion = N2.Engine.Singleton<IEngine>.Instance.Persister.Get(page.ID);

                // Removing the DelayPublishingUntil Date so that it won't get picked up again
                latestVersion["FuturePublishDate"] = null;
                masterVersion["FuturePublishDate"] = null;
                versioner.ReplaceVersion(masterVersion, latestVersion);

                // Get rid of all the "FuturePublishDate" dates so that it won't get called again
                // There might be a few hanging around because new version get saved
                foreach (ContentItem item in allVersions)
                {
                    N2.ContentItem version = N2.Engine.Singleton<IEngine>.Instance.Persister.Get(item.ID);
                    version["FuturePublishDate"] = null;
                    N2.Context.Persister.Save(latestVersion);
                }
            }
        }
    }
}
