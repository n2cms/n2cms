using System;
using N2.Tests.Fakes;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using N2.Persistence;
using N2.Persistence.NH;

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

			Assert.AreEqual(item, version.VersionOf);
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

			var versions = versioner.GetVersionsOf(item, 1);

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
			engine.Persister.Repository.Save(version);
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
        public void SaveVersion_DoesntAffect_CurrentVersionIndex()
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
            persister.Save(item);

            var version = versioner.SaveVersion(item);

            Assert.That(version.VersionIndex, Is.EqualTo(0));
            Assert.That(item.VersionIndex, Is.EqualTo(0));
        }

        [Test]
        public void VersionIndex_IsCarriedOn_WhenReplacingVersion()
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
            persister.Save(item);

            var version = versioner.SaveVersion(item);
            item.VersionIndex++;
            persister.Save(item);

            versioner.ReplaceVersion(item, CreateOneItem<Definitions.PersistableItem1>(0, "root2", null));

            Assert.That(item.VersionIndex, Is.EqualTo(0));
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

				var loadedVersioin = persister.Get(version.ID);
				Assert.That(loadedVersioin["Reference"], Is.Null);
			}
		}
	}
}
