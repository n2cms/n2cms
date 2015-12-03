using System;
using N2.Persistence;
using N2.Tests.Fakes;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using Shouldly;
using System.Linq;
using N2.Edit.Versioning;
using N2.Tests.Persistence;
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
        public override void TestFixtureTearDown()
        {
            N2.Utility.CurrentTime = () => DateTime.Now;
            engine.SecurityManager.ScopeEnabled = true;

			base.TestFixtureTearDown();
		}

        // versioning

        [Test]
        public void SaveVersion()
        {
            ContentItem item = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(item);
            ContentItem version = versioner.AddVersion(item);

            Assert.AreEqual(item, version.VersionOf.Value);
        }

        [Test]
        public void RestoreVersion()
        {
            string key = "TheKey";

            ContentItem item = CreateOneItem<PersistableItem>(0, "root", null);
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

            ContentItem item = CreateOneItem<PersistableItem>(0, "root", null);
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
            PersistableItem item = CreateOneItem<PersistableItem>(0, "item", null);
            using (new TimeCapsule(N2.Utility.CurrentTime().AddSeconds(-10)))
            {
                persister.Save(item);
            }

            ContentItem version = versioner.AddVersion(item);
            item.VersionIndex++;
            persister.Save(item);

            var versions = versioner.GetVersionsOf(item).ToList();

            Assert.That(versions.Count, Is.EqualTo(2));
            Assert.That(versions[0].Content, Is.EqualTo(item));
            Assert.That(versions[1].Content, Is.EqualTo(version));
        }

        [Test]
        public void GetVersions_IncludesPublishedVersion_WhenItsTheOnlyVersion()
        {
            PersistableItem item = CreateOneItem<PersistableItem>(0, "item", null);
            persister.Save(item);

            var versions = versioner.GetVersionsOf(item).ToList();

            Assert.That(versions.Count, Is.EqualTo(1));
        }

        [Test]
        public void CanGet_LatestVersion_Only()
        {
            PersistableItem item = CreateOneItem<PersistableItem>(0, "item", null);
            using (new TimeCapsule(N2.Utility.CurrentTime().AddSeconds(-10)))
            {
                persister.Save(item);
            }

            ContentItem version = versioner.AddVersion(item);
            item.VersionIndex++;
            persister.Save(item);

            var versions = versioner.GetVersionsOf(item, 0, 1).ToList();

            Assert.That(versions.Count, Is.EqualTo(1));
            Assert.That(versions[0].Content, Is.EqualTo(item));
        }

        [Test]
        public void CanTrim_NumberOfVersions()
        {
            ContentItem item = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(item);
            versioner.AddVersion(item);
            versioner.AddVersion(item);

            versioner.TrimVersionCountTo(item, 2);

            var versions = versioner.GetVersionsOf(item).ToList();
            Assert.That(versions.Count, Is.EqualTo(2));
        }

        [Test]
        public void CanTrim_NumberOfVersions_FromLargeNumberOfVersions()
        {
            ContentItem item = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(item);
            for (int i = 0; i < 25; i++)
            {
                versioner.AddVersion(item);
            }

            versioner.TrimVersionCountTo(item, 2);

            var versions = versioner.GetVersionsOf(item).ToList();
            Assert.That(versions.Count, Is.EqualTo(2));
        }

        [Test]
        public void CannotRemove_PublishedVersion()
        {
            ContentItem item = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(item);
            ContentItem version = versioner.AddVersion(item);
            version.Updated = N2.Utility.CurrentTime().AddSeconds(10);
            engine.Persister.Repository.SaveOrUpdate(version);
            engine.Persister.Repository.Flush();

            versioner.TrimVersionCountTo(item, 1);

            var versions = versioner.GetVersionsOf(item).ToList();
            Assert.That(versions.Count, Is.EqualTo(1));
            Assert.That(versions[0].Content, Is.EqualTo(item));
        }

        // version index

        [Test]
        public void Index_IsZero_OnSavedItem()
        {
            ContentItem item = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(item);

            Assert.That(item.VersionIndex, Is.EqualTo(0));
        }

        [Test]
        public void SaveVersion_Updates_CurrentVersionIndex()
        {
            ContentItem item = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(item);

            var version = versioner.AddVersion(item);

            Assert.That(version.VersionIndex, Is.EqualTo(0));
            Assert.That(item.VersionIndex, Is.EqualTo(1));
        }

        [Test]
        public void VersionIndex_IsNotCarriedOn_WhenReplacingVersion()
        {
            ContentItem item = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(item);

            var version = versioner.AddVersion(item);
            persister.Save(item);

            versioner.ReplaceVersion(item, CreateOneItem<PersistableItem>(0, "root2", null));

            Assert.That(item.VersionIndex, Is.EqualTo(2));
        }

        [Test]
        public void Delete_PreviousVersions_AreDeleted()
        {
            ContentItem item, version;
            using (persister)
            {
                item = CreateOneItem<PersistableItem>(0, "root", null);
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
                referenced = CreateOneItem<PersistableItem>(0, "reference", null);
                persister.Save(referenced);

                item = CreateOneItem<PersistableItem>(0, "root", null);
                item["Reference"] = referenced;
                persister.Save(item);

                version = versioner.AddVersion(item);
            }
            using (persister)
            {
                referenced = persister.Get(referenced.ID);
                persister.Delete(referenced);

				var loadedVersion = versioner.Repository.DeserializeVersion(versioner.Repository.GetVersion(item, version.VersionIndex));
                //persister.Get(version.ID);
                Assert.That(loadedVersion["Reference"], Is.Null);
            }
        }

        [Test]
        public void DetailCollection_MovesToNextVersion_WhenReplacing()
        {
            // Create item to link in original version
            PersistableItem link1 = CreateOneItem<PersistableItem>(0, "link1", null);
            persister.Save(link1);

            // Create item to link in new version
            PersistableItem link2 = CreateOneItem<PersistableItem>(0, "link2", null);
            persister.Save(link2);

            PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
            master.ContentLinks = new[] { link1 };
            persister.Save(master);

            PersistableItem version1 = (PersistableItem) versioner.AddVersion(master);
            
            master.ContentLinks = new[] { link2 };
            persister.Save(master);

            var version2 = versioner.ReplaceVersion(master, version1);

            PersistableItem restoredItem = (PersistableItem) persister.Get(master.ID);
            CollectionAssert.AreEqual(new[] { link1 }, restoredItem.ContentLinks);

            PersistableItem versionItem = (PersistableItem)versioner.GetVersion(master, version2.VersionIndex);
            // persister.Get(version2.ID);
            CollectionAssert.AreEqual(new[] { link2 }, versionItem.ContentLinks);

        }

        [Test]
        public void CreatingVersion_IncreasesVersionIndex()
        {
            PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(master);

            var version1 = versioner.AddVersion(master);

            master.VersionIndex.ShouldBe(1);
            version1.VersionIndex.ShouldBe(0);
        }

        [Test]
        public void ReplacingVersions_IncreasesVersionIndex()
        {
            PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
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
            PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(master);

            var version1 = versioner.AddVersion(master, asPreviousVersion: false);

            version1.VersionIndex.ShouldBe(1);
            master.VersionIndex.ShouldBe(0);
        }

        [Test]
        public void Replacing_MasterVersion_WithDraft_RemovesDraftVersion()
        {
            PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
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
            PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
            master.State = ContentState.Published;
            persister.Save(master);

            var version1 = versioner.AddVersion(master, asPreviousVersion: isPrevious);

            version1.State.ShouldBe(expectedState);
        }

        [TestCase(true, ContentState.Unpublished)]
        [TestCase(false, ContentState.Draft)]
        public void AddingVersion_OfUnpublishedItem_SetsStateOfVersion_AccordingTo_PreviousOrNot(bool isPrevious, ContentState expectedState)
        {
            PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
            master.State = ContentState.Unpublished;
            persister.Save(master);

            var version1 = versioner.AddVersion(master, asPreviousVersion: isPrevious);

            version1.State.ShouldBe(expectedState);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void AddingVersion_OfDraftItem_SetsStateToDraft(bool isPrevious)
        {
            PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
            master.State = ContentState.Draft;
            persister.Save(master);

            var version1 = versioner.AddVersion(master, asPreviousVersion: isPrevious);

            version1.State.ShouldBe(ContentState.Draft);
        }

        [Test]
        public void GetGreatestVersionIndex_WhenNoVersions()
        {
            PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(master);

            versioner.Repository.GetGreatestVersionIndex(master).ShouldBe(0);
        }

        [Test]
        public void SavingVersion_IncludesParts()
        {
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            var part = CreateOneItem<PersistablePart>(0, "part", root);
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
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            var part = CreateOneItem<PersistablePart>(0, "part", root);
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
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);

            var draft = versioner.AddVersion(root, asPreviousVersion: false);
            var part = new PersistablePart { Title = "part", Name = "part" };
            part.ZoneName = "TheZone";
            part.AddTo(draft);

            var master = versioner.MakeMasterVersion(draft);

            var addedChild = master.Children.Single();
            addedChild.State.ShouldBe(ContentState.Published);
            N2.Utility.CurrentTime().ShouldBeGreaterThanOrEqualTo(addedChild.Published.Value);
            addedChild.ID.ShouldNotBe(0);
            addedChild.VersionOf.HasValue.ShouldBe(false);
            addedChild.Title.ShouldBe("part");
        }

        [Test]
        public void PublishingDraft_CreatesNewParts_WithHierarchy()
        {
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);

            var draft = versioner.AddVersion(root, asPreviousVersion: false);
            var part = CreateOneItem<PersistablePart>(0, "part", draft);
            part.ZoneName = "TheZone";
            part.AddTo(draft);
            var part2 = CreateOneItem<PersistablePart>(0, "part2", part);
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
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            var part = CreateOneItem<PersistablePart>(0, "part", root);
            part.ZoneName = "TheZone";
            part.AddTo(root);
            persister.Save(part);

            var draft = versioner.AddVersion(root, asPreviousVersion: false);
            var draftPart = CreateOneItem<PersistablePart>(0, "new part", null);
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
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            var part = CreateOneItem<PersistablePart>(0, "part", root);
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
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            var part = CreateOneItem<PersistablePart>(0, "part", root);
            part.ZoneName = "TheZone";
            part.AddTo(root);
            persister.Save(part);
            var part2 = CreateOneItem<PersistablePart>(0, "part", part);
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
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            var part = CreateOneItem<PersistablePart>(0, "part", root);
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
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            var part = CreateOneItem<PersistablePart>(0, "part", root);
            part.ZoneName = "TheZone";
            part.AddTo(root);
            persister.Save(part);
            var part2 = CreateOneItem<PersistablePart>(0, "part2", part);
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
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            var part = CreateOneItem<PersistablePart>(0, "part", root);
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
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            var part1 = CreateOneItem<PersistablePart>(0, "part1", root);
            part1.ZoneName = "TheZone";
            part1.SortOrder = 333;
            part1.AddTo(root);
            persister.Save(part1);
            var part1_1 = CreateOneItem<PersistablePart>(0, "part1_1", part1);
            part1_1.ZoneName = "TheZone";
            part1_1.SortOrder = 333;
            part1_1.AddTo(part1);
            persister.Save(part1_1);
            var part2 = CreateOneItem<PersistablePart>(0, "part2", root);
            part2.ZoneName = "TheZone";
            part2.SortOrder = 333;
            part2.AddTo(root);
            persister.Save(part2);
            //  root
            //      part1
            //          part1_1
            //      part2

            var draft = versioner.AddVersion(root, asPreviousVersion: false);
            // remove part1 and part1_1
            draft.Children.Remove(draft.Children["part1"]);
            // modify part2
            var versionOfPart2 = draft.Children["part2"];
            versionOfPart2.Title += " changed";
            versionOfPart2.SortOrder = 666;
            versionOfPart2.ZoneName = "TheOtherZone";
            // add new1
            var new1 = CreateOneItem<PersistablePart>(0, "new1", null);
            new1.ZoneName = "TheZone";
            new1.AddTo(draft);
            // add new2 below new1
            var new1_1 = CreateOneItem<PersistablePart>(0, "new1_1", null);
            new1_1.ZoneName = "TheZone";
            new1_1.AddTo(draft.Children.Single(c => c.Name == "new1"));
            // add new2 below part2
            var new2 = CreateOneItem<PersistablePart>(0, "new2", null);
            new2.ZoneName = "TheZone";
            new2.AddTo(draft.Children["part2"]);
            //  root
            //      part1 - (deleted)
            //          part1_1 - (cascade deleted)
            //      part2 - modfied
            //          new2
            //      new1
            //          new1_1

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
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            var part = CreateOneItem<PersistablePart>(0, "part", root);
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
            var root = CreateOneItem<PersistableItem>(0, "root", null);
            root.PersistableProperty = "Hello world";
            persister.Save(root);

            var draft = versioner.AddVersion(root, asPreviousVersion: false);

            var restoredMaster = (PersistableItem)versioner.ReplaceVersion(root, draft);
            restoredMaster.PersistableProperty.ShouldBe("Hello world");
        }

        [Test]
        public void ChangesTo_PersistableProperties_AreCarriedOn_FromVersion()
        {
            var root = CreateOneItem<PersistableItem>(0, "root", null);
            root.PersistableProperty = "Hello world";
            persister.Save(root);

            var draft = (PersistableItem)versioner.AddVersion(root, asPreviousVersion: false);
            draft.PersistableProperty = "Herro!";

            versioner.ReplaceVersion(root, draft);
            root.PersistableProperty.ShouldBe("Herro!");
        }

        [Test]
        public void PersistableProperties_OnParts_AreCarriedAlong_WhenReplacingVersions()
        {
            var root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            var part = CreateOneItem<PersistablePart>(0, "part", root);
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
            var root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            var part = CreateOneItem<PersistablePart>(0, "part", root);
            part.ZoneName = "TheZone";
            part.AddTo(root);
            part.PersistableProperty = "Hello world";
            persister.Save(part);

            var draft = (PersistableItem)versioner.AddVersion(root, asPreviousVersion: false);
            draft.Children.OfType<PersistablePart>().Single().PersistableProperty = "Herro!";

            versioner.ReplaceVersion(root, draft);
            part["PersistableProperty"].ShouldBe("Herro!");
        }

        [Test]
        public void ReplaceVersion_MaintainsPartState()
        {
            var root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            var part = CreateOneItem<PersistablePart>(0, "part", root);
            part.ZoneName = "TheZone";
            part.AddTo(root);
            part["Greeting"] = "Hello";
            persister.Save(part);

            var draft = (PersistableItem)versioner.AddVersion(root, asPreviousVersion: false);
            draft.Children.OfType<PersistablePart>().Single()["Greeting"] = "Herro!";

            versioner.ReplaceVersion(root, draft);
            part.State.ShouldBe(ContentState.Published);
        }

        [Test]
        public void NewParts_ArePublished()
        {
            var root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            
            var draft = (PersistableItem)versioner.AddVersion(root, asPreviousVersion: false);

            var part = CreateOneItem<PersistablePart>(0, "part", draft);
            part.ZoneName = "TheZone";

            var part2 = CreateOneItem<PersistablePart>(0, "part2", part);
            part2.ZoneName = "TheZone";

            versioner.ReplaceVersion(root, draft);

            root.Children.Single().State.ShouldBe(ContentState.Published);
            root.Children.Single().Children.Single().State.ShouldBe(ContentState.Published);
        }

        [Test]
        public void DeleteVersion()
        {
            ContentItem item = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(item);
            versioner.AddVersion(item);

            versioner.Repository.DeserializeVersion(versioner.Repository.GetVersions(item).Single()).VersionOf.Value.ShouldBe(item);
            versioner.Repository.DeleteVersionsOf(item);
            versioner.Repository.GetVersions(item).ShouldBeEmpty();
        }

        [Test]
        public void DeleteVersions()
        {
            ContentItem item = CreateOneItem<PersistableItem>(0, "root", null);
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

            ContentItem item = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(item);
            versioner.AddVersion(item);
            versioner.Repository.GetVersions(item).Count().ShouldBe(1);

            persister.Delete(item);
            versioner.Repository.GetVersions(item).ShouldBeEmpty();
        }

        [Test]
        public void SelfLinks_AreStraightened()
        {
            ContentItem item = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(item);

            var version = versioner.AddVersion(item);
            version["Hello"] = version;

            versioner.ReplaceVersion(item, version, storeCurrentVersion: false);

            item["Hello"].ShouldBe(item);
        }

        [Test]
        public void SelfLinks_InCollections_AreStraightened()
        {
            ContentItem item = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(item);

            var version = versioner.AddVersion(item);
            version.GetDetailCollection("Hello", true).Add(version);

            versioner.ReplaceVersion(item, version, storeCurrentVersion: false);

            item.GetDetailCollection("Hello", false)[0].ShouldBe(item);
        }

        [Test]
        public void ProxiedDetail_IsCarriedOn_ToMasterVersion_WhenPublishinig()
        {
            var item = CreateOneItem<PersistableItem>(0, "root", null);

            using (persister)
            {
                item.StringList = new List<string> { "one", "two" };
                persister.Save(item);
            }

            PersistableItem version;
            using (persister)
            {
                item = persister.Get<PersistableItem>(item.ID);
                version = (PersistableItem)versioner.AddVersion(item, asPreviousVersion: false);

                version.StringList.Add("three");
                versioner.UpdateVersion(version);

                item.StringList.Count.ShouldBe(2);
            }

            using (persister)
            {
                item = persister.Get<PersistableItem>(item.ID);
                version = (PersistableItem)versioner.GetVersion(item, version.VersionIndex);
                versioner.ReplaceVersion(item, version, storeCurrentVersion: false);

                item.StringList.Count.ShouldBe(3);
                persister.Save(item);
            }

            item = persister.Get<PersistableItem>(item.ID);
            item.StringList.Count.ShouldBe(3);
        }

        [Test]
        public void ProxiedDetail_OnPart_IsCarriedOn_ToMasterVersion_WhenPublishinig()
        {
            var page = CreateOneItem<PersistableItem>(0, "root", null);
            var part = CreateOneItem<PersistablePart>(0, "part", page);

            // create initial state
            using (persister)
            {
                persister.Save(page);

                part.StringList = new List<string> { "one", "two" };
                persister.Repository.SaveOrUpdate(part);
            }

            // creae and modify a version
            PersistableItem version;
            PersistablePart partVersion;
            using (persister)
            {
                page = persister.Get<PersistableItem>(page.ID);
                part = (PersistablePart)page.Children[0];
                part.StringList.Count.ShouldBe(2);
                ((List<string>)part.Details["StringList"].Value).Count.ShouldBe(2);
                
                version = (PersistableItem)versioner.AddVersion(page);
                partVersion = (PersistablePart)version.Children[0];

                partVersion.StringList.Add("three");
                versioner.UpdateVersion(version);

                part.StringList.Count.ShouldBe(2);
                partVersion.StringList.Count.ShouldBe(3);
                ((List<string>)partVersion.Details["StringList"].Value).Count.ShouldBe(3);
            }

            // publish the version
            using (persister)
            {
                page = persister.Get<PersistableItem>(page.ID);
                part = (PersistablePart)page.Children[0];

                version = (PersistableItem)versioner.GetVersion(page, version.VersionIndex);
                partVersion = (PersistablePart)version.Children[0];
                versioner.ReplaceVersion(page, version, storeCurrentVersion: true);

                part.StringList.Count.ShouldBe(3);
            }

            page = persister.Get<PersistableItem>(page.ID);
            part = (PersistablePart)page.Children[0];
            part.StringList.Count.ShouldBe(3);
        }

        [Test]
        public void CloneForVersioningRecursive_AlsoClones_CertainReferenceTypes()
        {
            var page = CreateOneItem<PersistableItem>(0, "root", null);
            page.StringList = new List<string> { "1", "2" };

            var clone = (PersistableItem)page.CloneForVersioningRecursive();
            object.ReferenceEquals(clone.StringList, page.StringList).ShouldBe(false);
        }

		[Test]
		public void CloneForVersioningRecursive_AlsoClones_MetaInformation()
		{
			var page = CreateOneItem<PersistableItem>(0, "root", null);

			new N2.Details.ContentDetail(page, "Hello", "World") { Meta = "Super!" }.AddTo(page.DetailCollections["Hello"]);

			var clone = (PersistableItem)page.CloneForVersioningRecursive();
			clone.DetailCollections["Hello"].Details.Single().Meta.ShouldBe("Super!");
		}

		[Test]
        public void Links_AreMaintained_WhenCreatinVersions()
        {
            var root = CreateOneItem<PersistableItem>(0, "root", null);
            var item = CreateOneItem<PersistableItem>(0, "item", root);
            item.EditableLink = root;
            using (persister)
            {
                persister.Save(root);
                persister.Save(item);
            }

            PersistableItem version;
            using (persister)
            {
                item = persister.Get<PersistableItem>(item.ID);
                version = (PersistableItem)versioner.AddVersion(item, asPreviousVersion: false);
                version.EditableLink.ShouldBe(root);
            }

            using (persister)
            {
                version = (PersistableItem)versioner.GetVersion(item, version.VersionIndex);
                version.EditableLink.ShouldBe(root);
            }
        }

        [Test]
        public void Links_AreMaintained_OnParts_WhenCreatingVersions()
        {
            var root = CreateOneItem<PersistableItem>(0, "root", null);
            var item = CreateOneItem<PersistableItem>(0, "item", root);
            var part = CreateOneItem<PersistablePart>(0, "part", item);
            part.EditableLink = root;
            
            using (persister)
            {
                persister.Save(root);
                persister.Save(item);
                persister.Save(part);
            }

            PersistableItem version;
            using (persister)
            {
                item = persister.Get<PersistableItem>(item.ID);
                version = (PersistableItem)versioner.AddVersion(item, asPreviousVersion: false);
                ((PersistablePart)version.Children[0]).EditableLink.ShouldBe(root);
            }

            using (persister)
            {
                version = (PersistableItem)versioner.GetVersion(item, version.VersionIndex);
                versioner.Publish(persister, version);

                ((PersistablePart)persister.Get(part.ID)).EditableLink.ShouldBe(root);
            }
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

        [Test]
        public void PublishingDraft_CreatesNewPart_WithoutRemovingExpiryDate()
        {
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            persister.Save(root);
            
            var draft = versioner.AddVersion(root, asPreviousVersion: false);
            var part = new PersistablePart { Title = "part", Name = "part" };
            part.ZoneName = "TheZone";
            var now = DateTime.Now.StripMilliseconds();
            part.Expires = now;
            part.AddTo(draft);

            var master = versioner.MakeMasterVersion(draft);

            var addedChild = master.Children.Single();
            addedChild.Expires.Value.ShouldBe(now);
        }

        [Test]
        public void AddPreviousVersion_MasterShouldKeep_OriginalPublishedExpiration()
        {
            var now = DateTime.Now;
            using (N2.Utility.TimeCapsule(now.AddMinutes(-1)))
            {
                PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
                persister.Save(master);

                using (N2.Utility.TimeCapsule(now))
                {
                    versioner.AddVersion(master, asPreviousVersion: true);
                }

                var versions = versioner.GetVersionsOf(master).ToList();
                var masterInfo = versions[0];
                masterInfo.Published.ShouldBe(now.AddMinutes(-1));
                masterInfo.Expires.ShouldBe(null);
            }
        }

        [Test]
        public void AddSinglePreviousVersion_VersionShouldBorrow_PublishedFromMaster_AndExpiresFromCurrentTime()
        {
            var now = DateTime.Now;
            using (N2.Utility.TimeCapsule(now.AddMinutes(-1)))
            {
                PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
                persister.Save(master);

                using (N2.Utility.TimeCapsule(now))
                {
                    versioner.AddVersion(master, asPreviousVersion: true);
                }

                var versions = versioner.GetVersionsOf(master).ToList();
                var previousVersion = versions[1];
                previousVersion.Published.ShouldBe(now.AddMinutes(-1));
                previousVersion.Expires.ShouldBe(now);
            }
        }

        [Test]
        public void AddMultiplePreviousVersions_VersionShouldBorrow_PublishedFromPreviousExpires_AndExpiresFromCurrentTime()
        {
            var now = DateTime.Now;
            using (N2.Utility.TimeCapsule(now.AddMinutes(-2)))
            {
                PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
                persister.Save(master);

                using (N2.Utility.TimeCapsule(now.AddMinutes(-1)))
                {
                    versioner.AddVersion(master, asPreviousVersion: true);
                }

                using (N2.Utility.TimeCapsule(now))
                {
                    versioner.AddVersion(master, asPreviousVersion: true);
                }

                var versions = versioner.GetVersionsOf(master).ToList();
                var previousVersion = versions[1];
                previousVersion.Published.ShouldBe(now.AddMinutes(-1));
                previousVersion.Expires.ShouldBe(now);
                
                var oldestVersion = versions[2];
                oldestVersion.Published.ShouldBe(now.AddMinutes(-2));
                oldestVersion.Expires.ShouldBe(now.AddMinutes(-1));
            }

        }

        [Test]
        public void AddDraftVersion_VersionShouldHave_NoPublishedDate()
        {
            var now = DateTime.Now;
            using (N2.Utility.TimeCapsule(now.AddMinutes(-1)))
            {
                PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
                persister.Save(master);

                using (N2.Utility.TimeCapsule(now))
                {
                    versioner.AddVersion(master, asPreviousVersion: false);
                }

                var versions = versioner.GetVersionsOf(master).ToList();
                var draft = versions[0];
                draft.Published.ShouldBe(null);
                draft.Expires.ShouldBe(null);
            }
        }

        [Test]
        public void AddDraftVersion_Master_ShouldNotBeAffected()
        {
            var now = DateTime.Now;
            using (N2.Utility.TimeCapsule(now.AddMinutes(-1)))
            {
                PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
                persister.Save(master);

                using (N2.Utility.TimeCapsule(now))
                {
                    versioner.AddVersion(master, asPreviousVersion: false);
                }

                var versions = versioner.GetVersionsOf(master).ToList();
                var masterInfo = versions[1];
                masterInfo.Published.ShouldBe(now.AddMinutes(-1));
                masterInfo.Expires.ShouldBe(null);
            }
        }

        [Test]
        public void PublishDraftVersion_DraftShouldBorrow_PublishedExpiresFromCurrentMaster()
        {
            var now = DateTime.Now;
            using (N2.Utility.TimeCapsule(now.AddMinutes(-1)))
            {
                PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
                persister.Save(master);

                var draft = versioner.AddVersion(master, asPreviousVersion: false);
                using (N2.Utility.TimeCapsule(now))
                {
                    versioner.ReplaceVersion(master, draft, storeCurrentVersion: true);
                }

                var versions = versioner.GetVersionsOf(master).ToList();
                var masterInfo = versions[0];
                masterInfo.Published.ShouldBe(now.AddMinutes(-1));
                masterInfo.Expires.ShouldBe(null);
            }
        }

        [Test]
        public void PublishDraftVersion_MasterShouldExpireNow_AndBorrowPublishedFromPreviousVersion()
        {
            var now = DateTime.Now;
            using (N2.Utility.TimeCapsule(now.AddMinutes(-2)))
            {
                PersistableItem master = CreateOneItem<PersistableItem>(0, "root", null);
                persister.Save(master);
                using (N2.Utility.TimeCapsule(now.AddMinutes(-1)))
                {
                    var version = versioner.AddVersion(master, asPreviousVersion: true);
                }

                var draft = versioner.AddVersion(master, asPreviousVersion: false);
                using (N2.Utility.TimeCapsule(now))
                {
                    versioner.ReplaceVersion(master, draft, storeCurrentVersion: true);
                }

                var versions = versioner.GetVersionsOf(master).ToList();
                var previousMaster = versions[1];
                previousMaster.Published.ShouldBe(now.AddMinutes(-1));
                previousMaster.Expires.ShouldBe(now);
            }
        }
    }
}
