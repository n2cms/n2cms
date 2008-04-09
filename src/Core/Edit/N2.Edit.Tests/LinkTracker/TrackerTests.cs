using System;
using System.Collections.Generic;
using System.Text;
using N2.Edit.LinkTracker;
using MbUnit.Framework;
using Rhino.Mocks;
using N2.Engine;
using N2.Persistence;
using Rhino.Mocks.Interfaces;
using N2.Web;
using N2.Details;

namespace N2.Tests.Edit.LinkTracker
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

			IWebContext wrapper = CreateWrapper(true);
			IItemNotifier notifier = CreateNotifier(true);
			parser = new UrlParser(persister, wrapper, notifier, 1);

			root = CreateOneItem<Items.TrackableItem>(1, "root", null);
			item1 = CreateOneItem<Items.TrackableItem>(2, "item1", root);
			item2 = CreateOneItem<Items.TrackableItem>(3, "item2", root);
			mocks.Replay(persister);

			linkFactory = new Tracker(persister, null, parser);
			linkFactory.Start();
		}

		private IItemNotifier CreateNotifier(bool replay)
		{
			IItemNotifier notifier = mocks.CreateMock<IItemNotifier>();
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

			root["TestDetail"] = "<a href='/item1.aspx'>first item</a>";
			persister.Save(root);
			Assert.AreEqual(1, root.GetDetailCollection("TrackedLinks", false).Count);
			root["TestDetail"] = null;
			persister.Save(root);

			DetailCollection links = root.GetDetailCollection("TrackedLinks", false);
			Assert.AreEqual(0, links.Count);
		}

		[Test]
		public void ChangeLink()
		{
			mocks.ReplayAll();

			root["TestDetail"] = "<a href='/item1.aspx'>first item</a>";
			persister.Save(root);
			Assert.AreEqual(item1, root.GetDetailCollection("TrackedLinks", false)[0]);
			root["TestDetail"] = "<a href='/item2.aspx'>first item</a>";
			persister.Save(root);

			DetailCollection links = root.GetDetailCollection("TrackedLinks", false);
			Assert.AreEqual(item2, links[0]);
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
	}
}
