using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Tests;
using N2.Edit.AutoPublish;
using N2.Persistence;
using N2.Security;
using N2.Web;
using N2.Tests.Persistence;
using N2.Management.Externals;
using Shouldly;
using N2.Edit.Versioning;

namespace N2.Management.Tests
{
    [TestFixture]
    public class PublishScheduledActionTests : DatabasePreparingBase
    {
        private PublishScheduledAction action;
        private IVersionManager versionManager;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            action = engine.Resolve<PublishScheduledAction>();
            engine.SecurityManager.ScopeEnabled = false;
            versionManager = engine.Resolve<IVersionManager>();
        }

        [Test]
        public void ItemBecomePublished_ByDateChange_ChangesStateToPublished()
        {
            var item = new ExternalItem { State = ContentState.Waiting, Published = N2.Utility.CurrentTime().AddSeconds(-10) };
            using (engine.SecurityManager.Disable())
            {
                engine.Persister.Save(item);
            }

            action.Execute();

            item.State.ShouldBe(ContentState.Published);
        }

        [Test]
        public void ItemBecomeExpired_ByDateChange_ChangesStateToUnpublished()
        {
            var item = new ExternalItem { State = ContentState.Published, Published = N2.Utility.CurrentTime().AddSeconds(-10), Expires = N2.Utility.CurrentTime().AddSeconds(-5) };
            using (engine.SecurityManager.Disable())
            {
                engine.Persister.Save(item);
            }
            action.Execute();

            item.State.ShouldBe(ContentState.Unpublished);
        }

        [Test]
        public void ItemMarkedForFuturePublishing_IsPublished_WhenPublishingTimeIsReached()
        {
            var item = new ExternalItem { Title = "Original", State = ContentState.Published, Published = N2.Utility.CurrentTime().AddSeconds(-10) };
            using (engine.SecurityManager.Disable())
            {
                engine.Persister.Save(item);

                var version = versionManager.AddVersion(item, asPreviousVersion: false);
                version.Title = "ToBePublished";
                action.MarkForFuturePublishing(version, N2.Utility.CurrentTime().AddSeconds(-5));
                versionManager.UpdateVersion(version);
            }

            action.Execute();

            var published = engine.Persister.Get(item.ID);
            var allVersions = versionManager.GetVersionsOf(published).ToList();
            var unpublished = allVersions.Single(v => v.State == ContentState.Unpublished);

            allVersions.Count.ShouldBe(2);
            published.Title.ShouldBe("ToBePublished");
            unpublished.Title.ShouldBe("Original");
        }

        [Test, Ignore]
        public void VersionToBePublished_IsNotStoredInVersionList()
        {
            using (engine.SecurityManager.Disable())
            {
                var item = new ExternalItem { Title = "Original", State = ContentState.Published, Published = N2.Utility.CurrentTime().AddSeconds(-10) };
                engine.Persister.Save(item);

                var version = new ExternalItem { Title = "ToBePublished", State = ContentState.Published, Published = N2.Utility.CurrentTime().AddSeconds(-10), VersionOf = item };
                action.MarkForFuturePublishing(version, N2.Utility.CurrentTime().AddSeconds(-5));
                engine.Persister.Save(version);
            }

            action.Execute();

            var all = engine.Persister.Repository.Find();
            all.Count().ShouldBe(2);
        }
    }
}
