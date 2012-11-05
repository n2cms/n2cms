using System;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Tests.Fakes;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using Shouldly;
using System.Linq;
using N2.Edit.Versioning;
using N2.Tests.Persistence;
using N2.Persistence.Proxying;
using N2.Definitions;
using System.Collections.Generic;

namespace N2.Tests.Edit.Versioning
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
			engine.SecurityManager.ScopeEnabled = false;
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			engine.SecurityManager.ScopeEnabled = true;
		}

        // versioning

		[Test]
		public void SaveVersion()
		{
			ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(item);
			ContentItem version = versioner.AddVersion(item);

			Assert.AreEqual(item, version.VersionOf.Value);
		}

		[Test]
		public void RestoreVersion()
		{
            string key = "TheKey";

			ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
			item[key] = 1;
			persister.Save(item);
			ContentItem version = versioner.AddVersion(item);
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
			ContentItem version = versioner.AddVersion(item);
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

			ContentItem version = versioner.AddVersion(item);
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

			ContentItem version = versioner.AddVersion(item);
            item.VersionIndex++;
            persister.Save(item);

			var versions = versioner.GetVersionsOf(item, 0, 1);

			Assert.That(versions.Count, Is.EqualTo(1));
            Assert.That(versions[0], Is.EqualTo(item));
        }

		[Test]
		public void CanTrim_NumberOfVersions()
		{
			ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(item);
			versioner.AddVersion(item);
			versioner.AddVersion(item);

			versioner.TrimVersionCountTo(item, 2);

			var versions = versioner.GetVersionsOf(item);
			Assert.That(versions.Count, Is.EqualTo(2));
		}

		[Test]
		public void CanTrim_NumberOfVersions_FromLargeNumberOfVersions()
		{
			ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(item);
			for (int i = 0; i < 25; i++)
			{
				versioner.AddVersion(item);
			}

			versioner.TrimVersionCountTo(item, 2);

			var versions = versioner.GetVersionsOf(item);
			Assert.That(versions.Count, Is.EqualTo(2));
		}

		[Test]
		public void CannotRemove_PublishedVersion()
		{
			ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(item);
			ContentItem version = versioner.AddVersion(item);
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
            ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
            persister.Save(item);

            Assert.That(item.VersionIndex, Is.EqualTo(0));
        }

        [Test]
        public void SaveVersion_Updates_CurrentVersionIndex()
        {
            ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
            persister.Save(item);

            var version = versioner.AddVersion(item);

            Assert.That(version.VersionIndex, Is.EqualTo(0));
            Assert.That(item.VersionIndex, Is.EqualTo(1));
        }

        [Test]
        public void VersionIndex_IsNotCarriedOn_WhenReplacingVersion()
        {
            ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
            persister.Save(item);

            var version = versioner.AddVersion(item);
            persister.Save(item);

            versioner.ReplaceVersion(item, CreateOneItem<PersistableItem1>(0, "root2", null));

            Assert.That(item.VersionIndex, Is.EqualTo(2));
		}

		[Test]
		public void Delete_PreviousVersions_AreDeleted()
		{
			ContentItem item, version;
			using (persister)
			{
				item = CreateOneItem<PersistableItem1>(0, "root", null);
				persister.Save(item);

				version = versioner.AddVersion(item);
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
				referenced = CreateOneItem<PersistableItem1>(0, "reference", null);
				persister.Save(referenced);

				item = CreateOneItem<PersistableItem1>(0, "root", null);
				item["Reference"] = referenced;
				persister.Save(item);

				version = versioner.AddVersion(item);
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
			PersistableItem1 link1 = CreateOneItem<PersistableItem1>(0, "link1", null);
			persister.Save(link1);

			// Create item to link in new version
			PersistableItem1 link2 = CreateOneItem<PersistableItem1>(0, "link2", null);
			persister.Save(link2);

			PersistableItem1 master = CreateOneItem<PersistableItem1>(0, "root", null);
			master.ContentLinks = new[] { link1 };
			persister.Save(master);

			PersistableItem1 version1 = (PersistableItem1) versioner.AddVersion(master);
			
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
			PersistableItem1 master = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(master);

			var version1 = versioner.AddVersion(master);

			master.VersionIndex.ShouldBe(1);
			version1.VersionIndex.ShouldBe(0);
		}

		[Test]
		public void ReplacingVersions_IncreasesVersionIndex()
		{
			PersistableItem1 master = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(master);

			var version1 = versioner.AddVersion(master);

			var version2 = versioner.ReplaceVersion(master, version1, storeCurrentVersion: true);

			version1.VersionIndex.ShouldBe(0);
			version2.VersionIndex.ShouldBe(1);
			master.VersionIndex.ShouldBe(2);
		}

		[Test]
		public void SavingVersion_CanGiveNewVersion_GreaterIndex()
		{
			PersistableItem1 master = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(master);

			var version1 = versioner.AddVersion(master, asPreviousVersion: false);

			version1.VersionIndex.ShouldBe(1);
			master.VersionIndex.ShouldBe(0);
		}

		[Test]
		public void Replacing_MasterVersion_WithDraft_RemovesDraftVersion()
		{
			PersistableItem1 master = CreateOneItem<PersistableItem1>(0, "root", null);
			master.State = ContentState.Published;
			persister.Save(master);

			var version1 = versioner.AddVersion(master, asPreviousVersion: false);
			version1.Title += " changes";
			var version2 = versioner.ReplaceVersion(master, version1, storeCurrentVersion: true);

			version2.VersionIndex.ShouldBe(0);
			version2.Title.ShouldBe("root");
			master.VersionIndex.ShouldBe(1);
			master.Title.ShouldBe("root changes");
			versioner.GetVersionsOf(master).Count().ShouldBe(2);
		}

		[TestCase(true, ContentState.Unpublished)]
		[TestCase(false, ContentState.Draft)]
		public void AddingVersion_OfPublishedItem_SetsStateOfVersion_AccordingTo_PreviousOrNot(bool isPrevious, ContentState expectedState)
		{
			PersistableItem1 master = CreateOneItem<PersistableItem1>(0, "root", null);
			master.State = ContentState.Published;
			persister.Save(master);

			var version1 = versioner.AddVersion(master, asPreviousVersion: isPrevious);

			version1.State.ShouldBe(expectedState);
		}

		[TestCase(true, ContentState.Unpublished)]
		[TestCase(false, ContentState.Draft)]
		public void AddingVersion_OfUnpublishedItem_SetsStateOfVersion_AccordingTo_PreviousOrNot(bool isPrevious, ContentState expectedState)
		{
			PersistableItem1 master = CreateOneItem<PersistableItem1>(0, "root", null);
			master.State = ContentState.Unpublished;
			persister.Save(master);

			var version1 = versioner.AddVersion(master, asPreviousVersion: isPrevious);

			version1.State.ShouldBe(expectedState);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void AddingVersion_OfDraftItem_SetsStateToDraft(bool isPrevious)
		{
			PersistableItem1 master = CreateOneItem<PersistableItem1>(0, "root", null);
			master.State = ContentState.Draft;
			persister.Save(master);

			var version1 = versioner.AddVersion(master, asPreviousVersion: isPrevious);

			version1.State.ShouldBe(ContentState.Draft);
		}

		[Test]
		public void GetGreatestVersionIndex_WhenNoVersions()
		{
			PersistableItem1 master = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(master);

			versioner.Repository.GetGreatestVersionIndex(master).ShouldBe(0);
		}

		[Test]
		public void SavingVersion_IncludesParts()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part = CreateOneItem<PersistablePart1>(0, "part", root);
			part.ZoneName = "TheZone";
			persister.Save(part);

			var version1 = versioner.AddVersion(root, asPreviousVersion: false);

			var versionPart = version1.Children.Single();
			versionPart.Title.ShouldBe("part");
			versionPart.VersionOf.ID.ShouldBe(part.ID);
		}

		[Test]
		public void IncludedParts_InheritStateFromPage()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part = CreateOneItem<PersistablePart1>(0, "part", root);
			part.ZoneName = "TheZone";
			persister.Save(part);

			root.State = ContentState.Draft;
			var version1 = versioner.AddVersion(root, asPreviousVersion: false);

			var versionPart = version1.Children.Single();
			versionPart.State.ShouldBe(ContentState.Draft);
		}

		[Test]
		public void PublishingDraft_CreatesNewPart()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);

			var draft = versioner.AddVersion(root, asPreviousVersion: false);
			var part = new PersistablePart1 { Title = "part", Name = "part" };
			part.ZoneName = "TheZone";
			part.AddTo(draft);

			var master = versioner.MakeMasterVersion(draft);

			var addedChild = master.Children.Single();
			addedChild.State.ShouldBe(ContentState.Published);
			DateTime.Now.ShouldBeGreaterThanOrEqualTo(addedChild.Published.Value);
			addedChild.ID.ShouldNotBe(0);
			addedChild.VersionOf.HasValue.ShouldBe(false);
			addedChild.Title.ShouldBe("part");
		}

		[Test]
		public void PublishingDraft_CreatesNewParts_WithHierarchy()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);

			var draft = versioner.AddVersion(root, asPreviousVersion: false);
			var part = CreateOneItem<PersistablePart1>(0, "part", draft);
			part.ZoneName = "TheZone";
			part.AddTo(draft);
			var part2 = CreateOneItem<PersistablePart1>(0, "part2", part);
			part2.ZoneName = "TheZone";
			part2.AddTo(part);

			var master = versioner.MakeMasterVersion(draft);

			var addedGrandChild = master.Children.Single().Children.Single();
			addedGrandChild.State.ShouldBe(ContentState.Published);
			addedGrandChild.ID.ShouldNotBe(0);
			addedGrandChild.VersionOf.HasValue.ShouldBe(false);
			addedGrandChild.Title.ShouldBe("part2");
		}

		[Test]
		public void PublishingDraft_CreatesNewParts_BelowExistingParts()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part = CreateOneItem<PersistablePart1>(0, "part", root);
			part.ZoneName = "TheZone";
			part.AddTo(root);
			persister.Save(part);

			var draft = versioner.AddVersion(root, asPreviousVersion: false);
			var draftPart = CreateOneItem<PersistablePart1>(0, "new part", null);
			draftPart.ZoneName = "TheZone";
			draftPart.AddTo(draft.Children.Single());

			var master = versioner.MakeMasterVersion(draft);

			var addedChild = master.Children.Single().Children.Single();
			addedChild.State.ShouldBe(ContentState.Published);
			addedChild.ID.ShouldNotBe(0);
			addedChild.VersionOf.HasValue.ShouldBe(false);
			addedChild.Title.ShouldBe("new part");
		}

		[Test]
		public void PublishingDraft_RemovesRemovedParts()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part = CreateOneItem<PersistablePart1>(0, "part", root);
			part.ZoneName = "TheZone";
			part.AddTo(root);
			persister.Save(part);
			
			var draft = versioner.AddVersion(root, asPreviousVersion: false);
			draft.Children.Clear();

			var master = versioner.MakeMasterVersion(draft);

			master.Children.Count.ShouldBe(0);
			persister.Get(part.ID).ShouldBe(null);
		}

		[Test]
		public void PublishingDraft_RemovesRemovedParts_InHierarchy()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part = CreateOneItem<PersistablePart1>(0, "part", root);
			part.ZoneName = "TheZone";
			part.AddTo(root);
			persister.Save(part);
			var part2 = CreateOneItem<PersistablePart1>(0, "part", part);
			part2.ZoneName = "TheZone";
			part2.AddTo(part);
			persister.Save(part2);

			var draft = versioner.AddVersion(root, asPreviousVersion: false);
			draft.Children.Clear();

			var master = versioner.MakeMasterVersion(draft);

			master.Children.Count.ShouldBe(0);
			persister.Get(part.ID).ShouldBe(null);
			persister.Get(part2.ID).ShouldBe(null);
		}

		[Test]
		public void PublishingDraft_UpdatesParts()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part = CreateOneItem<PersistablePart1>(0, "part", root);
			part.ZoneName = "TheZone";
			part.AddTo(root);
			persister.Save(part);

			var draft = versioner.AddVersion(root, asPreviousVersion: false);
			draft.Children.Single().Title += " modified";

			var master = versioner.MakeMasterVersion(draft);

			master.Children.Single().Title.ShouldBe("part modified");
		}

		[Test]
		public void PublishingDraft_Updates_DescendantParts()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part = CreateOneItem<PersistablePart1>(0, "part", root);
			part.ZoneName = "TheZone";
			part.AddTo(root);
			persister.Save(part);
			var part2 = CreateOneItem<PersistablePart1>(0, "part2", part);
			part2.ZoneName = "TheZone";
			part2.AddTo(part);
			persister.Save(part2);

			var draft = versioner.AddVersion(root, asPreviousVersion: false);
			draft.Children.Single().Children.Single().Title += " modified";

			var master = versioner.MakeMasterVersion(draft);

			master.Children.Single().Children.Single().Title.ShouldBe("part2 modified");
		}

		[Test]
		public void PublishingDraft_ZoneName_And_SortOrder_IsUpdatedFromDraft()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part = CreateOneItem<PersistablePart1>(0, "part", root);
			part.ZoneName = "TheZone";
			part.SortOrder = 333;
			part.AddTo(root);
			persister.Save(part);

			var draft = versioner.AddVersion(root, asPreviousVersion: false);
			var draftChild = draft.Children.Single();
			draftChild.ZoneName = "TheOtherZone";
			draftChild.SortOrder = 666;

			var master = versioner.MakeMasterVersion(draft);

			var updatedPart = master.Children.Single();
			updatedPart.ZoneName.ShouldBe("TheOtherZone");
			updatedPart.SortOrder.ShouldBe(666);
		}

		[Test]
		public void PublishingDraft_PuttingItAllTogether()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part1 = CreateOneItem<PersistablePart1>(0, "part1", root);
			part1.ZoneName = "TheZone";
			part1.SortOrder = 333;
			part1.AddTo(root);
			persister.Save(part1);
			var part1_1 = CreateOneItem<PersistablePart1>(0, "part1_1", part1);
			part1_1.ZoneName = "TheZone";
			part1_1.SortOrder = 333;
			part1_1.AddTo(part1);
			persister.Save(part1_1);
			var part2 = CreateOneItem<PersistablePart1>(0, "part2", root);
			part2.ZoneName = "TheZone";
			part2.SortOrder = 333;
			part2.AddTo(root);
			persister.Save(part2);
			//	root
			//		part1
			//			part1_1
			//		part2

			var draft = versioner.AddVersion(root, asPreviousVersion: false);
			// remove part1 and part1_1
			draft.Children.Remove(draft.Children["part1"]);
			// modify part2
			var versionOfPart2 = draft.Children["part2"];
			versionOfPart2.Title += " changed";
			versionOfPart2.SortOrder = 666;
			versionOfPart2.ZoneName = "TheOtherZone";
			// add new1
			var new1 = CreateOneItem<PersistablePart1>(0, "new1", null);
			new1.ZoneName = "TheZone";
			new1.AddTo(draft);
			// add new2 below new1
			var new1_1 = CreateOneItem<PersistablePart1>(0, "new1_1", null);
			new1_1.ZoneName = "TheZone";
			new1_1.AddTo(draft.Children.Single(c => c.Name == "new1"));
			// add new2 below part2
			var new2 = CreateOneItem<PersistablePart1>(0, "new2", null);
			new2.ZoneName = "TheZone";
			new2.AddTo(draft.Children["part2"]);
			//	root
			//		part1 - (deleted)
			//			part1_1 - (cascade deleted)
			//		part2 - modfied
			//			new2
			//		new1
			//			new1_1

			var master = versioner.MakeMasterVersion(draft);
			master.Children["part1"].ShouldBe(null);
			persister.Get(part1.ID).ShouldBe(null);
			persister.Get(part1_1.ID).ShouldBe(null);
			var updatedPart2 = master.Children["part2"];
			updatedPart2.Title.ShouldBe("part2 changed");
			updatedPart2.ZoneName.ShouldBe("TheOtherZone");
			updatedPart2.SortOrder.ShouldBe(666);
			updatedPart2.Children.Single().Title.ShouldBe("new2");
			var publishedNew1 = master.Children["new1"];
			publishedNew1.Title.ShouldBe("new1");
			publishedNew1.ID.ShouldBeGreaterThan(0);
			var publishedNew1_1 = publishedNew1.Children.Single();
			publishedNew1_1.Title.ShouldBe("new1_1");
			publishedNew1_1.ID.ShouldBeGreaterThan(0);
		}

		[Test]
		public void AddingVersionOfPart_AddsVersionOfPage_AndReturnsPart()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part = CreateOneItem<PersistablePart1>(0, "part", root);
			part.ZoneName = "TheZone";
			part.AddTo(root);
			persister.Save(part);

			var draft = versioner.AddVersion(part, asPreviousVersion: false);

			draft.Name.ShouldBe("part");
			draft.ID.ShouldBe(0);
			part.Parent.Name.ShouldBe("root");
			draft.Parent.ID.ShouldBe(0);
		}

		[Test]
		public void PersistableProperties_AreCarriedAlong_WhenReplacingVersions()
		{
			var root = CreateOneItem<PersistableItem1>(0, "root", null);
			root.PersistableProperty = "Hello world";
			persister.Save(root);

			var draft = versioner.AddVersion(root, asPreviousVersion: false);

			var restoredMaster = (PersistableItem1)versioner.ReplaceVersion(root, draft);
			restoredMaster.PersistableProperty.ShouldBe("Hello world");
		}

		[Test]
		public void ChangesTo_PersistableProperties_AreCarriedOn_FromVersion()
		{
			var root = CreateOneItem<PersistableItem1>(0, "root", null);
			root.PersistableProperty = "Hello world";
			persister.Save(root);

			var draft = (PersistableItem1)versioner.AddVersion(root, asPreviousVersion: false);
			draft.PersistableProperty = "Herro!";

			versioner.ReplaceVersion(root, draft);
			root.PersistableProperty.ShouldBe("Herro!");
		}

		[Test]
		public void PersistableProperties_OnParts_AreCarriedAlong_WhenReplacingVersions()
		{
			var root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part = CreateOneItem<PersistablePart1>(0, "part", root);
			part.ZoneName = "TheZone";
			part.AddTo(root);
			part.PersistableProperty = "Hello world";
			persister.Save(part);

			var draft = versioner.AddVersion(root, asPreviousVersion: false);

			versioner.ReplaceVersion(root, draft);
			part["PersistableProperty"].ShouldBe("Hello world");
		}

		[Test]
		public void ChangesTo_PersistableProperties_OnParts_AreCarriedOn_FromVersion()
		{
			var root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part = CreateOneItem<PersistablePart1>(0, "part", root);
			part.ZoneName = "TheZone";
			part.AddTo(root);
			part.PersistableProperty = "Hello world";
			persister.Save(part);

			var draft = (PersistableItem1)versioner.AddVersion(root, asPreviousVersion: false);
			draft.Children.OfType<PersistablePart1>().Single().PersistableProperty = "Herro!";

			versioner.ReplaceVersion(root, draft);
			part["PersistableProperty"].ShouldBe("Herro!");
		}

		[Test]
		public void ReplaceVersion_MaintainsPartState()
		{
			var root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);
			var part = CreateOneItem<PersistablePart1>(0, "part", root);
			part.ZoneName = "TheZone";
			part.AddTo(root);
			part["Greeting"] = "Hello";
			persister.Save(part);

			var draft = (PersistableItem1)versioner.AddVersion(root, asPreviousVersion: false);
			draft.Children.OfType<PersistablePart1>().Single()["Greeting"] = "Herro!";

			versioner.ReplaceVersion(root, draft);
			part.State.ShouldBe(ContentState.Published);
		}

		[Test]
		public void NewParts_ArePublished()
		{
			var root = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(root);
			
			var draft = (PersistableItem1)versioner.AddVersion(root, asPreviousVersion: false);

			var part = CreateOneItem<PersistablePart1>(0, "part", draft);
			part.ZoneName = "TheZone";

			var part2 = CreateOneItem<PersistablePart1>(0, "part2", part);
			part2.ZoneName = "TheZone";

			versioner.ReplaceVersion(root, draft);

			root.Children.Single().State.ShouldBe(ContentState.Published);
			root.Children.Single().Children.Single().State.ShouldBe(ContentState.Published);
		}

		[Test]
		public void DeleteVersion()
		{
			ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(item);
			versioner.AddVersion(item);

			versioner.Repository.GetVersions(item).Single().Version.VersionOf.Value.ShouldBe(item);
			versioner.Repository.DeleteVersionsOf(item);
			versioner.Repository.GetVersions(item).ShouldBeEmpty();
		}

		[Test]
		public void DeleteVersions()
		{
			ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(item);
			versioner.AddVersion(item);
			versioner.AddVersion(item);
			versioner.AddVersion(item);

			versioner.Repository.GetVersions(item).Count().ShouldBe(3);
			versioner.Repository.DeleteVersionsOf(item);
			versioner.Repository.GetVersions(item).ShouldBeEmpty();
		}

		[Test]
		public void DeleteVersions_ViaPersister()
		{
			engine.Resolve<ContentVersionCleanup>().Start();

			ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(item);
			versioner.AddVersion(item);
			versioner.Repository.GetVersions(item).Count().ShouldBe(1);

			persister.Delete(item);
			versioner.Repository.GetVersions(item).ShouldBeEmpty();
		}

		[Test]
		public void SelfLinks_AreStraightened()
		{
			ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(item);

			var version = versioner.AddVersion(item);
			version["Hello"] = version;

			versioner.ReplaceVersion(item, version, storeCurrentVersion: false);

			item["Hello"].ShouldBe(item);
		}

		[Test]
		public void SelfLinks_InCollections_AreStraightened()
		{
			ContentItem item = CreateOneItem<PersistableItem1>(0, "root", null);
			persister.Save(item);

			var version = versioner.AddVersion(item);
			version.GetDetailCollection("Hello", true).Add(version);

			versioner.ReplaceVersion(item, version, storeCurrentVersion: false);

			item.GetDetailCollection("Hello", false)[0].ShouldBe(item);
		}

		[Test]
		public void ProxiedDetails_AreRestored_WhenRetrieving()
		{
			var item = CreateOneItem<PersistableItem1>(0, "root", null);
			item.StringList = new List<string> { "one", "two" };
			persister.Save(item);

			persister.Dispose();

			item = persister.Get<PersistableItem1>(item.ID);
			item.StringList.Count.ShouldBe(2);

			var version = (PersistableItem1)versioner.AddVersion(item);
			
			version.StringList.Add("three");
			versioner.UpdateVersion(version);

			persister.Dispose();

			item = persister.Get<PersistableItem1>(item.ID);
			version = (PersistableItem1)versioner.GetVersion(item, version.VersionIndex);
			versioner.ReplaceVersion(item, version, storeCurrentVersion: false);

			item.StringList.Count.ShouldBe(3);
			persister.Save(item);

			persister.Dispose();

			item = persister.Get<PersistableItem1>(item.ID);
			item.StringList.Count.ShouldBe(3);
		}

		[Test, Ignore("TODO")]
		public void ExistingDetails_MaintainTheirIdentity()
		{
		}

		[Test, Ignore("TODO")]
		public void NewDetails_AreAdded()
		{
		}

		[Test, Ignore("TODO")]
		public void RemovedDetails_AreDeleted()
		{
		}

		[Test, Ignore("TODO")]
		public void RemovedDetailCollections_AreDeleted()
		{
		}

		[Test, Ignore("TODO")]
		public void ExistingDetailCollections_MaintainTheirIdentity()
		{
		}

		[Test, Ignore("TODO")]
		public void NewDetailCollections_AreAdded()
		{
		}
	}
}
