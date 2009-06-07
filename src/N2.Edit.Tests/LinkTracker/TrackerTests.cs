using N2.Configuration;
using N2.Edit.FileSystem;
using N2.Edit.FileSystem.Items;
using N2.Edit.LinkTracker;
using N2.Edit.Tests.FileSystem;
using N2.Tests;
using N2.Tests.Fakes;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Persistence;
using N2.Web;
using N2.Details;

namespace N2.Edit.Tests.LinkTracker
{
	[TestFixture]
	public class TrackerTests : ItemPersistenceMockingBase
	{
		#region SetUp
		Tracker linkFactory;
		IUrlParser parser;

		ContentItem root;
		ContentItem item1;
		ContentItem item2;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			var wrapper = new N2.Tests.Fakes.FakeWebContextWrapper();
			var notifier = CreateNotifier(true);
			parser = new UrlParser(persister, wrapper, notifier, new Host(wrapper, 1, 1), new HostSection());

			root = CreateOneItem<N2.Tests.Edit.LinkTracker.Items.TrackableItem>(1, "root", null);
			item1 = CreateOneItem<N2.Tests.Edit.LinkTracker.Items.TrackableItem>(2, "item1", root);
			item2 = CreateOneItem<N2.Tests.Edit.LinkTracker.Items.TrackableItem>(3, "item2", root);

			var errorHandler = new FakeErrorHandler();
			linkFactory = new Tracker(persister, null, parser, errorHandler);
			linkFactory.Start();
		}

		private IItemNotifier CreateNotifier(bool replay)
		{
			IItemNotifier notifier = mocks.StrictMock<IItemNotifier>();
			notifier.ItemCreated += null;
			LastCall.IgnoreArguments().Repeat.Any();
			if (replay)
				mocks.Replay(notifier);
			return notifier;
		}
		#endregion


		[Test]
		public void TrackSingleLink()
		{
			mocks.ReplayAll();

			root["TestDetail"] = "<a href='/item1.aspx'>first item</a>";
			persister.Save(root);

			Assert.AreEqual(1, root.GetDetailCollection("TrackedLinks", false).Count);
		}

		[Test]
		public void RemoveLink()
		{
			mocks.ReplayAll();

			using (persister)
			{
				root["TestDetail"] = "<a href='/item1.aspx'>first item</a>";
				persister.Save(root);

				Assert.AreEqual(1, root.GetDetailCollection("TrackedLinks", false).Count);
			}
			using (persister)
			{
				root["TestDetail"] = null;
				persister.Save(root);

				DetailCollection links = root.GetDetailCollection("TrackedLinks", false);
				Assert.AreEqual(0, links.Count);
			}
		}

		[Test]
		public void ChangeLink()
		{
			mocks.ReplayAll();

			using (persister)
			{
				root["TestDetail"] = "<a href='/item1.aspx'>first item</a>";
				persister.Save(root);

				var links = root.GetDetailCollection("TrackedLinks", false);
				Assert.That(links, Is.Not.Null);
				Assert.That(links[0], Is.EqualTo(item1));
			}

			using (persister)
			{
				root["TestDetail"] = "<a href='/item2.aspx'>first item</a>";
				persister.Save(root);

				var links = root.GetDetailCollection("TrackedLinks", false);
				Assert.That(links, Is.Not.Null);
				Assert.That(links[0], Is.EqualTo(item2));
			}
		}

		[Test]
		public void LinkWithHash()
		{
			mocks.ReplayAll();

			root["TestDetail"] = "<a href='/item1.aspx#hash'>first item (a bit further down)</a>";
			persister.Save(root);

			DetailCollection links = root.GetDetailCollection("TrackedLinks", false);
			Assert.AreEqual(item1, links[0]);
		}


		[Test]
		public void HandlesEmailLinks()
		{
			mocks.ReplayAll();

			root["TestDetail"] = @"<a href=""mailto:cristian@libardo.com"">mail me</a>";
			persister.Save(root);

			DetailCollection links = root.GetDetailCollection("TrackedLinks", false);
			Assert.IsNull(links);
		}

		[Test]
		public void HandlesFtpLinks()
		{
			mocks.ReplayAll();

			root["TestDetail"] = @"<a href=""ftp://ftp.n2cms.com"">download stuff</a>";
			persister.Save(root);

			DetailCollection links = root.GetDetailCollection("TrackedLinks", false);
			Assert.IsNull(links);
		}

		[Test]
		public void DoesNotTrackLinks_ToItems_WithZeroID()
		{
			((FakeFileSystem) Context.Current.Resolve<IFileSystem>()).PathProvider =
				new FakePathProvider(((FakeFileSystem) Context.Current.Resolve<IFileSystem>()).BasePath);

			RootDirectory rootDir = CreateOneItem<RootDirectory>(4, "FileSystem", root);

			root["TestDetail"] = @"<a href=""/FileSystem/upload/File.txt"">download pdf</a>";
			persister.Save(root);

			DetailCollection links = root.GetDetailCollection("TrackedLinks", false);
			Assert.IsNull(links);
		}
	}
}