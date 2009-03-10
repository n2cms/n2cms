using System;
using System.Reflection;
using N2.Tests.Fakes;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using N2.Web.UI;
using Rhino.Mocks;
using System.Configuration;
using N2.Configuration;

namespace N2.Tests.Persistence.NH
{
	[TestFixture]
	public class VersionManagerTests : ItemTestsBase
	{
		ContentPersister persister;
		SessionProvider sessionProvider;
		IItemFinder finder;
		VersionManager versioner;
		
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			mocks = new MockRepository();

			ITypeFinder typeFinder = mocks.StrictMock<ITypeFinder>();
			Expect.On(typeFinder).Call(typeFinder.GetAssemblies())
				.Return(new Assembly[] { typeof(Definitions.PersistableItem1).Assembly }).Repeat.Any();
			Expect.On(typeFinder).Call(typeFinder.Find(typeof(ContentItem)))
				.Return(new Type[] { typeof(Definitions.PersistableItem1) }).Repeat.AtLeastOnce();

			mocks.ReplayAll();

			IDefinitionManager definitions = new DefinitionManager(new DefinitionBuilder(typeFinder, new EditableHierarchyBuilder<IEditable>(), new AttributeExplorer<EditorModifierAttribute>(), new AttributeExplorer<IDisplayable>(), new AttributeExplorer<IEditable>(), new AttributeExplorer<IEditableContainer>()), null);
			ConfigurationBuilder configurationBuilder = new ConfigurationBuilder(definitions, (DatabaseSection)ConfigurationManager.GetSection("n2/database"));

			sessionProvider = new SessionProvider(configurationBuilder, new NotifyingInterceptor(new ItemNotifier()), new Fakes.FakeWebContextWrapper());

			finder = new ItemFinder(sessionProvider, definitions);

			mocks.VerifyAll();
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			IRepository<int, ContentItem> itemRepository = new NHRepository<int, ContentItem>(sessionProvider);
			INHRepository<int, LinkDetail> linkRepository = new NHRepository<int, LinkDetail>(sessionProvider);

			persister = new ContentPersister(itemRepository, linkRepository, finder);
			versioner = new VersionManager(itemRepository, new ItemFinder(sessionProvider, null));
		}

		[Test]
		public void SaveVersion()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			persister.Save(item);
			ContentItem version = versioner.SaveVersion(item);
			Assert.AreEqual(item, version.VersionOf);
			persister.Delete(item);
		}

		[Test]
		public void RestoreVersion()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			item["VersionIndex"] = 1;
			persister.Save(item);
			ContentItem version = versioner.SaveVersion(item);
			item["VersionIndex"] = 2;
			persister.Save(item);

			versioner.ReplaceVersion(item, version);

			ContentItem restoredItem = persister.Get(item.ID);
			Assert.AreEqual(1, restoredItem["VersionIndex"]);

			persister.Delete(item);
		}

		[Test]
		public void RestoreVersionSetExpireDate()
		{
			ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
			item["VersionIndex"] = 1;
			persister.Save(item);
			ContentItem version = versioner.SaveVersion(item);
			item["VersionIndex"] = 2;
			persister.Save(item);

			versioner.ReplaceVersion(item, version);

			ContentItem restoredItem = persister.Get(item.ID);
			Assert.IsNull(restoredItem.Expires, "Expires was supposed to be null but was " + restoredItem.Expires);

			persister.Delete(item);
		}

		[Test]
		public void GetVersions_DisplaysVersions_InChangedOrder()
		{
			PersistableItem1 item = CreateOneItem<PersistableItem1>(0, "item", null);
			using (new TimeCapsule(DateTime.Now.AddSeconds(-10)))
			{
				persister.Save(item);
			}

			ContentItem version = versioner.SaveVersion(item);

			var versions = versioner.GetVersionsOf(item);

			Assert.That(versions.Count, Is.EqualTo(2));
			Assert.That(versions[0], Is.EqualTo(version));
			Assert.That(versions[1], Is.EqualTo(item));
		}

		[Test]
		public void GetVersions_IncludesPublishedVersion_WhenItsTheOnlyVersion()
		{
			PersistableItem1 item = CreateOneItem<PersistableItem1>(0, "item", null);
			persister.Save(item);

			var versions = versioner.GetVersionsOf(item);

			Assert.That(versions.Count, Is.EqualTo(1));
		}

		[Test]
		public void CanGetLatestVersionOnly()
		{
			PersistableItem1 item = CreateOneItem<PersistableItem1>(0, "item", null);
			using (new TimeCapsule(DateTime.Now.AddSeconds(-10)))
			{
				persister.Save(item);
			}

			ContentItem version = versioner.SaveVersion(item);

			var versions = versioner.GetVersionsOf(item, 1);

			Assert.That(versions.Count, Is.EqualTo(1));
			Assert.That(versions[0], Is.EqualTo(version));
		}

	}
}
