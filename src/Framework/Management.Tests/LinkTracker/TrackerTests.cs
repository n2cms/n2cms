using System.Linq;
using N2.Configuration;
using N2.Details;
using N2.Edit.FileSystem;
using N2.Edit.FileSystem.Items;
using N2.Edit.LinkTracker;
using N2.Edit.Tests.FileSystem;
using N2.Engine;
using N2.Persistence;
using N2.Tests;
using N2.Tests.Fakes;
using N2.Web;
using NUnit.Framework;
using Shouldly;
using Rhino.Mocks;

namespace N2.Edit.Tests.LinkTracker
{
	[TestFixture]
	public class TrackerTests : ItemPersistenceMockingBase
	{
		#region SetUp
		Tracker tracker;
		IUrlParser parser;

		ContentItem root;
		ContentItem item1;
		ContentItem item2;
		private FakeRepository<ContentDetail> detailRepository;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			var wrapper = new N2.Tests.Fakes.FakeWebContextWrapper();
			parser = TestSupport.Setup(persister, wrapper, new Host(wrapper, 1, 1));

			root = CreateOneItem<N2.Tests.Edit.LinkTracker.Items.TrackableItem>(1, "root", null);
			item1 = CreateOneItem<N2.Tests.Edit.LinkTracker.Items.TrackableItem>(2, "item1", root);
			item2 = CreateOneItem<N2.Tests.Edit.LinkTracker.Items.TrackableItem>(3, "item2", root);

			var errorHandler = new FakeErrorHandler();
			var monitor = new N2.Plugin.ConnectionMonitor();
			tracker = new Tracker(persister, detailRepository = new FakeRepository<ContentDetail>(), parser, monitor, errorHandler, new EditSection());
			monitor.SetConnected(Installation.SystemStatusLevel.UpAndRunning);
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
		public void TrackMultipleLinks()
		{
			mocks.ReplayAll();

			root["TestDetail"] = "<a href='/item1.aspx'>first item</a><a href='/item1.aspx'>first item</a>";
			root["TestDetail2"] = "<a href='/item1.aspx'>first item</a>";
			persister.Save(root);

			var details = root.GetDetailCollection("TrackedLinks", false);
			details.Count.ShouldBe(3);
			details.Details[0].IntValue.Value.ShouldBe(9);
			details.Details[0].StringValue.ShouldBe("/item1.aspx");
			details.Details[0].Meta.ShouldBe("TestDetail");
			details.Details[1].IntValue.Value.ShouldBe(45);
			details.Details[1].StringValue.ShouldBe("/item1.aspx");
			details.Details[1].Meta.ShouldBe("TestDetail");
			details.Details[2].IntValue.Value.ShouldBe(9);
			details.Details[2].StringValue.ShouldBe("/item1.aspx");
			details.Details[2].Meta.ShouldBe("TestDetail2");
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
				Assert.That(links.Details[0].LinkedItem, Is.EqualTo(item1));
			}

			using (persister)
			{
				root["TestDetail"] = "<a href='/item2.aspx'>first item</a>";
				persister.Save(root);

				var links = root.GetDetailCollection("TrackedLinks", false);
				Assert.That(links, Is.Not.Null);
				Assert.That(links.Details[0].LinkedItem, Is.EqualTo(item2));
			}
		}

		[Test]
		public void UpdateReferencesTo_ChangesDetail_OnReferencingPage()
		{
			SaveAndMove(root, "<a href='/item1.aspx'>first item</a>", item1, item2);

			root.GetDetailCollection(Tracker.LinkDetailName, false).Details[0].StringValue.ShouldBe("/item2/item1.aspx");
		}

		[Test]
		public void UpdateReferencesTo_ChangesDetail_OnReferencingPage_OtherLinksAreUnaffected()
		{
			SaveAndMove(root, "<a href='/item1.aspx'>first item</a><a href='/item2.aspx'>first item</a>", item1, item2);

			root.GetDetailCollection(Tracker.LinkDetailName, false).Details[0].StringValue.ShouldBe("/item2/item1.aspx");
			root.GetDetailCollection(Tracker.LinkDetailName, false).Details[1].StringValue.ShouldBe("/item2.aspx");
		}

		[Test]
		public void UpdateReferencesTo_ChangesContent_OnReferencingPage()
		{
			SaveAndMove(root, "<a href='/item1.aspx'>first item</a>", item1, item2);

			root["TestDetail"].ToString().ShouldBe("<a href='/item2/item1.aspx'>first item</a>");
		}

		[Test]
		public void UpdateReferencesTo_ChangesContent_OnReferencingPage_OtherLinksAreUnaffected()
		{
			SaveAndMove(root, "<a href='/item1.aspx'>first item</a><a href='/item2.aspx'>first item</a>", item1, item2);

			root["TestDetail"].ToString().ShouldBe("<a href='/item2/item1.aspx'>first item</a><a href='/item2.aspx'>first item</a>");
		}

		[Test]
		public void UpdateReferencesTo_ChangesContent_OnMultipleReferencingPages()
		{
			root["TestDetail"] = "<a href='/item1.aspx'>first item</a>";
			SaveWithBenefits(root);
			item2["TestDetail"] = "<a href='/item1.aspx'>first item</a>";
			SaveWithBenefits(item2);

			persister.Move(item1, item2);
			tracker.UpdateReferencesTo(item1);

			root["TestDetail"].ShouldBe("<a href='/item2/item1.aspx'>first item</a>");
			item2["TestDetail"].ShouldBe("<a href='/item2/item1.aspx'>first item</a>");
		}

		[Test]
		public void UpdateReferencesTo_ChangesContent_OnChildItems()
		{
			var fi = typeof(ContentItem).GetField("url", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var item3 = CreateOneItem<N2.Tests.Edit.LinkTracker.Items.TrackableItem>(4, "item3", root);
			
			item1["TestDetail"] = "<a href='/item2.aspx'>2</a><a href='/item3.aspx'>3</a>";
			item2["TestDetail"] = "<a href='/item1.aspx'>1</a><a href='/item3.aspx'>3</a>";
			item3["TestDetail"] = "<a href='/item1.aspx'>1</a><a href='/item2.aspx'>2</a>";
			SaveWithBenefits(item1);
			SaveWithBenefits(item2);
			SaveWithBenefits(item3);

			persister.Move(item3, item2);
			tracker.UpdateReferencesTo(item3);

			persister.Move(item2, item1);

			fi.SetValue(item3, null);
			tracker.UpdateReferencesTo(item2);
			tracker.UpdateReferencesTo(item3);

			item1["TestDetail"].ShouldBe("<a href='/item1/item2.aspx'>2</a><a href='/item1/item2/item3.aspx'>3</a>");
			item2["TestDetail"].ShouldBe("<a href='/item1.aspx'>1</a><a href='/item1/item2/item3.aspx'>3</a>");
			item3["TestDetail"].ShouldBe("<a href='/item1.aspx'>1</a><a href='/item1/item2.aspx'>2</a>");
		}

		[Test]
		public void UpdateReferencesTo_ChangesContent_OnPage_WithMultipleReferences()
		{
			root["TestDetail"] = "<ul><li><a href='/item1.aspx'>first item 1</a></li><li><a href='/item1.aspx'>first item 2</a></li></ul>";
			root["TestDetail2"] = "<p>Lorem ipsum dolor sit <a href='/item1.aspx'>amet</a></p>";
			SaveWithBenefits(root);

			persister.Move(item1, item2);
			tracker.UpdateReferencesTo(item1);

			root["TestDetail"].ShouldBe("<ul><li><a href='/item2/item1.aspx'>first item 1</a></li><li><a href='/item2/item1.aspx'>first item 2</a></li></ul>");
			root["TestDetail2"].ShouldBe("<p>Lorem ipsum dolor sit <a href='/item2/item1.aspx'>amet</a></p>");
		}

		private void SaveAndMove(ContentItem item, string html, ContentItem moved, ContentItem target)
		{
			item["TestDetail"] = html;
			SaveWithBenefits(item);

			persister.Move(moved, target);
			tracker.UpdateReferencesTo(moved);
		}

		private void SaveWithBenefits(ContentItem item)
		{
			persister.Save(item);
			// simulate persistent details
			foreach (var d in item.Details.Union(item.DetailCollections.SelectMany(dc => dc.Details)))
				detailRepository.SaveOrUpdate(d);
		}

		[Test]
		public void LinkWithHash()
		{
			mocks.ReplayAll();

			root["TestDetail"] = "<a href='/item1.aspx#hash'>first item (a bit further down)</a>";
			persister.Save(root);

			DetailCollection links = root.GetDetailCollection("TrackedLinks", false);
			Assert.AreEqual(item1, links.Details[0].LinkedItem);
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
		public void StoresFtpLink_AsString()
		{
			mocks.ReplayAll();

			root["TestDetail"] = @"<a href=""ftp://ftp.n2cms.com"">download stuff</a>";
			persister.Save(root);

			DetailCollection links = root.GetDetailCollection("TrackedLinks", false);
			Assert.That(links, Is.Not.Null);
			Assert.That(links.Details[0].LinkedItem, Is.Null);
			Assert.That(links.Details[0].StringValue, Is.EqualTo("ftp://ftp.n2cms.com"));
		}

		[Test]
		public void TracksUrl_ToItemsWithoutId()
		{
			RootDirectory rootDir = CreateOneItem<RootDirectory>(4, "FileSystem", root);
			((IInjectable<IUrlParser>)rootDir).Set(TestSupport.Setup(persister, new FakeWebContextWrapper(), new Host(null, 1, 1)));
			var fs = new FakeMemoryFileSystem();
			fs.files["/FileSystem/upload/File.txt"] = new FileData { Name = "File.txt" };
			rootDir.Set(fs);
			rootDir.Set(new ImageSizeCache(new ConfigurationManagerWrapper { Sections = new ConfigurationManagerWrapper.ContentSectionTable(null, null, null, new EditSection()) }));

			root["TestDetail"] = @"<a href=""/FileSystem/upload/File.txt"">download pdf</a>";
			persister.Save(root);

			DetailCollection links = root.GetDetailCollection("TrackedLinks", false);
			Assert.That(links, Is.Not.Null);
			Assert.That(links.Details[0].LinkedItem, Is.Null);
			Assert.That(links.Details[0].StringValue, Is.EqualTo("/FileSystem/upload/File.txt"));
		}
	}
}