using System;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Tests.Fakes;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using Shouldly;
using System.Linq;

namespace N2.Tests.Persistence.NH
{
	[TestFixture, Category("Integration")]
	public class VersionManagerTests : DatabasePreparingBase
	{
		ContentPersister persister;
		VersionManager versioner;

		[TestFixtureSetUp]
		public override void TestFixtureSetUp()
		{
			base.TestFixtureSetUp();

			persister = (ContentPersister) engine.Resolve<IPersister>();
			versioner = (VersionManager) engine.Resolve<IVersionManager>();
		}

        // versioning

		[Test]
		public void SaveVersion()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			persister.Save(item);
			ContentItem version = versioner.SaveVersion(item);

			Assert.AreEqual(item, version.VersionOf.Value);
		}

		[Test]
		public void RestoreVersion()
		{
            string key = "TheKey";

			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			item[key] = 1;
			persister.Save(item);
			ContentItem version = versioner.SaveVersion(item);
			item[key] = 2;
			persister.Save(item);

			versioner.ReplaceVersion(item, version);

			ContentItem restoredItem = persister.Get(item.ID);
			Assert.AreEqual(1, restoredItem[key]);
		}

		[Test]
		public void RestoreVersion_SetExpireDate()
		{
            string key = "TheKey";

			ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
			item[key] = 1;
			persister.Save(item);
			ContentItem version = versioner.SaveVersion(item);
			item[key] = 2;
			persister.Save(item);

			versioner.ReplaceVersion(item, version);

			ContentItem restoredItem = persister.Get(item.ID);
			Assert.IsNull(restoredItem.Expires, "Expires was supposed to be null but was " + restoredItem.Expires);
		}

		[Test]
		public void GetVersions_DisplaysVersions_InInverseVersionIndexOrder()
		{
			PersistableItem1 item = CreateOneItem<PersistableItem1>(0, "item", null);
			using (new TimeCapsule(DateTime.Now.AddSeconds(-10)))
			{
				persister.Save(item);
			}

			ContentItem version = versioner.SaveVersion(item);
            item.VersionIndex++;
            persister.Save(item);

			var versions = versioner.GetVersionsOf(item);

			Assert.That(versions.Count, Is.EqualTo(2));
            Assert.That(versions[0], Is.EqualTo(item));
            Assert.That(versions[1], Is.EqualTo(version));
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
		public void CanGet_LatestVersion_Only()
		{
			PersistableItem1 item = CreateOneItem<PersistableItem1>(0, "item", null);
			using (new TimeCapsule(DateTime.Now.AddSeconds(-10)))
			{
				persister.Save(item);
			}

			ContentItem version = versioner.SaveVersion(item);
            item.VersionIndex++;
            persister.Save(item);

			var versions = versioner.GetVersionsOf(item, 0, 1);

			Assert.That(versions.Count, Is.EqualTo(1));
            Assert.That(versions[0], Is.EqualTo(item));
        }

		[Test]
		public void CanTrim_NumberOfVersions()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			persister.Save(item);
			versioner.SaveVersion(item);
			versioner.SaveVersion(item);

			versioner.TrimVersionCountTo(item, 2);

			var versions = versioner.GetVersionsOf(item);
			Assert.That(versions.Count, Is.EqualTo(2));
		}

		[Test]
		public void CanTrim_NumberOfVersions_FromLargeNumberOfVersions()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			persister.Save(item);
			for (int i = 0; i < 25; i++)
			{
				versioner.SaveVersion(item);
			}

			versioner.TrimVersionCountTo(item, 2);

			var versions = versioner.GetVersionsOf(item);
			Assert.That(versions.Count, Is.EqualTo(2));
		}

		[Test]
		public void CannotRemove_PublishedVersion()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			persister.Save(item);
			ContentItem version = versioner.SaveVersion(item);
			version.Updated = DateTime.Now.AddSeconds(10);
			engine.Persister.Repository.SaveOrUpdate(version);
			engine.Persister.Repository.Flush();

			versioner.TrimVersionCountTo(item, 1);

			var versions = versioner.GetVersionsOf(item);
			Assert.That(versions.Count, Is.EqualTo(1));
			Assert.That(versions[0], Is.EqualTo(item));
        }

        // version index

        [Test]
        public void Index_IsZero_OnSavedItem()
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
            persister.Save(item);

            Assert.That(item.VersionIndex, Is.EqualTo(0));
        }

        [Test]
        public void SaveVersion_Updates_CurrentVersionIndex()
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
            persister.Save(item);

            var version = versioner.SaveVersion(item);

            Assert.That(version.VersionIndex, Is.EqualTo(0));
            Assert.That(item.VersionIndex, Is.EqualTo(1));
        }

        [Test]
        public void VersionIndex_IsNotCarriedOn_WhenReplacingVersion()
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
            persister.Save(item);

            var version = versioner.SaveVersion(item);
            persister.Save(item);

            versioner.ReplaceVersion(item, CreateOneItem<Definitions.PersistableItem1>(0, "root2", null));

            Assert.That(item.VersionIndex, Is.EqualTo(2));
		}

		[Test]
		public void Delete_PreviousVersions_AreDeleted()
		{
			ContentItem item, version;
			using (persister)
			{
				item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
				persister.Save(item);

				version = versioner.SaveVersion(item);
			}
			using (persister)
			{
				item = persister.Get(item.ID);
				persister.Delete(item);

				var loadedVersioin = persister.Get(version.ID);
				Assert.That(loadedVersioin, Is.Null);
			}
		}

		[Test]
		public void Delete_References_OnPreviousVersions_AreDeleted()
		{
			ContentItem item, referenced, version;
			using (persister)
			{
				referenced = CreateOneItem<Definitions.PersistableItem1>(0, "reference", null);
				persister.Save(referenced);

				item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
				item["Reference"] = referenced;
				persister.Save(item);

				version = versioner.SaveVersion(item);
			}
			using (persister)
			{
				referenced = persister.Get(referenced.ID);
				persister.Delete(referenced);

				var loadedVersioin = versioner.Repository.GetVersion(item, version.VersionIndex).Version;
				//persister.Get(version.ID);
				Assert.That(loadedVersioin["Reference"], Is.Null);
			}
		}

		[Test]
		public void DetailCollection_MovesToNextVersion_WhenReplacing()
		{
			// Create item to link in original version
			PersistableItem1 link1 = CreateOneItem<Definitions.PersistableItem1>(0, "link1", null);
			persister.Save(link1);

			// Create item to link in new version
			PersistableItem1 link2 = CreateOneItem<Definitions.PersistableItem1>(0, "link2", null);
			persister.Save(link2);

			PersistableItem1 master = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			master.ContentLinks = new[] { link1 };
			persister.Save(master);

			PersistableItem1 version1 = (PersistableItem1) versioner.SaveVersion(master);
			
			master.ContentLinks = new[] { link2 };
			persister.Save(master);

			var version2 = versioner.ReplaceVersion(master, version1);

			PersistableItem1 restoredItem = (PersistableItem1) persister.Get(master.ID);
			CollectionAssert.AreEqual(new[] { link1 }, restoredItem.ContentLinks);

			PersistableItem1 versionItem = (PersistableItem1)versioner.GetVersion(master, version2.VersionIndex);
			// persister.Get(version2.ID);
			CollectionAssert.AreEqual(new[] { link2 }, versionItem.ContentLinks);

		}

		[Test]
		public void CreatingVersion_IncreasesVersionIndex()
		{
			PersistableItem1 master = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			persister.Save(master);

			var version1 = versioner.SaveVersion(master);

			master.VersionIndex.ShouldBe(1);
			version1.VersionIndex.ShouldBe(0);
		}

		[Test]
		public void ReplacingVersions_IncreasesVersionIndex()
		{
			PersistableItem1 master = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			persister.Save(master);

			var version1 = versioner.SaveVersion(master);

			var version2 = versioner.ReplaceVersion(master, version1, storeCurrentVersion: true);

			version1.VersionIndex.ShouldBe(0);
			version2.VersionIndex.ShouldBe(1);
			master.VersionIndex.ShouldBe(2);
		}

		[Test]
		public void SavingVersion_CanGiveNewVersion_GreaterIndex()
		{
			PersistableItem1 master = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			persister.Save(master);

			var version1 = versioner.SaveVersion(master, createPreviousVersion: false);

			version1.VersionIndex.ShouldBe(1);
			master.VersionIndex.ShouldBe(0);
		}

		[Test]
		public void ReplacingVersion_SwitchesVersionIndex()
		{
			PersistableItem1 master = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			persister.Save(master);

			var version1 = versioner.SaveVersion(master, createPreviousVersion: false);
			var version2 = versioner.ReplaceVersion(master, version1, storeCurrentVersion: true);

			version1.VersionIndex.ShouldBe(1);
			version2.VersionIndex.ShouldBe(0);
			master.VersionIndex.ShouldBe(2);
		}

		[Test]
		public void GetGreatestVersionIndex_WhenNoVersions()
		{
			PersistableItem1 master = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			persister.Save(master);

			versioner.Repository.GetGreatestVersionIndex(master).ShouldBe(0);
		}

		[Test]
		public void SavingVersion_IncludesParts()
		{
			PersistableItem1 root = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part = CreateOneItem<Definitions.PersistablePart1>(0, "part", root);
			part.ZoneName = "TheZone";
			persister.Save(part);

			var version1 = versioner.SaveVersion(root, createPreviousVersion: false);

			var versionPart = version1.Children.Single();
			versionPart.Title.ShouldBe("part");
			versionPart.VersionOf.ID.ShouldBe(part.ID);
		}

		[Test]
		public void IncludedParts_InheritStateFromPage()
		{
			PersistableItem1 root = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part = CreateOneItem<Definitions.PersistablePart1>(0, "part", root);
			part.ZoneName = "TheZone";
			persister.Save(part);

			root.State = ContentState.Draft;
			var version1 = versioner.SaveVersion(root, createPreviousVersion: false);

			var versionPart = version1.Children.Single();
			versionPart.State.ShouldBe(ContentState.Draft);
		}
	}
}
