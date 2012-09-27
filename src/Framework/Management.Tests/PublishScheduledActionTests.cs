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

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			action = engine.Resolve<PublishScheduledAction>();
			engine.SecurityManager.ScopeEnabled = false;
		}

		[Test]
		public void ItemBecomePublished_ByDateChange_ChangesStateToPublished()
		{
			var item = new ExternalItem { State = ContentState.Waiting, Published = DateTime.Now.AddSeconds(-10) };
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
			var item = new ExternalItem { State = ContentState.Published, Published = DateTime.Now.AddSeconds(-10), Expires = DateTime.Now.AddSeconds(-5) };
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
			using (engine.SecurityManager.Disable())
			{
				var item = new ExternalItem { Title = "Original", State = ContentState.Published, Published = DateTime.Now.AddSeconds(-10) };
				engine.Persister.Save(item);

				var version = new ExternalItem { Title = "ToBePublished", State = ContentState.Published, Published = DateTime.Now.AddSeconds(-10), VersionOf = item };
				action.MarkForFuturePublishing(version, DateTime.Now.AddSeconds(-5));
				engine.Persister.Save(version);
			}

			action.Execute();

			var all = engine.Persister.Repository.Find().ToList();
			var published = all.Single(i => i.State == ContentState.Published && !i.VersionOf.HasValue);
			//var unpublished = all.Single(i => i.State == ContentState.Unpublished && i.VersionOf.HasValue);
			var versions = engine.Resolve<IRepository<ContentVersion>>().Find();
			var unpublished = versions.Where(i => i.State == ContentState.Unpublished).Select(v => v.Version).Single();

			published.Title.ShouldBe("ToBePublished");
			unpublished.Title.ShouldBe("Original");
		}

		[Test, Ignore]
		public void VersionToBePublished_IsNotStoredInVersionList()
		{
			using (engine.SecurityManager.Disable())
			{
				var item = new ExternalItem { Title = "Original", State = ContentState.Published, Published = DateTime.Now.AddSeconds(-10) };
				engine.Persister.Save(item);

				var version = new ExternalItem { Title = "ToBePublished", State = ContentState.Published, Published = DateTime.Now.AddSeconds(-10), VersionOf = item };
				action.MarkForFuturePublishing(version, DateTime.Now.AddSeconds(-5));
				engine.Persister.Save(version);
			}

			action.Execute();

			var all = engine.Persister.Repository.Find();
			all.Count().ShouldBe(2);
		}
	}
}
