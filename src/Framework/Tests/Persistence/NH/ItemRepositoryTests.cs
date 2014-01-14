using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using N2.Details;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using Shouldly;

namespace N2.Tests.Persistence.NH
{
    [TestFixture, Category("Integration")]
    public class ItemRepositoryTests : DatabasePreparingBase
    {
        ContentItemRepository repository;
        new ISessionProvider sessionProvider;

        protected override T CreateOneItem<T>(int id, string name, ContentItem parent)
        {
            var item = base.CreateOneItem<T>(id, name, parent);
            repository.Save(item);
            return item;
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            CreateDatabaseSchema();

            sessionProvider = engine.Resolve<ISessionProvider>();
            repository = new ContentItemRepository(sessionProvider);
        }

        [Test]
        public void CanSave()
        {
            int itemID = SaveAnItem("savedItem", null);
            Assert.AreNotEqual(0, itemID);

            using (repository)
            {
                ContentItem item = repository.Get(itemID);
                Assert.AreEqual(item.ID, itemID);
                repository.Delete(item);
                repository.Flush();
            }
        }

        [Test]
        public void CanUpdate()
        {
            int itemID = SaveAnItem("savedItem", null);

            using (repository)
            {
                ContentItem item = repository.Get(itemID);
                item.Title = "updated item";
                repository.SaveOrUpdate(item);
                repository.Flush();
            }

            using (repository)
            {
                ContentItem item = repository.Get(itemID);
                Assert.AreEqual("updated item", item.Title);
                repository.Delete(item);
                repository.Flush();
            }
        }

        [Test]
        public void CanDelete()
        {
            int itemID = SaveAnItem("itemToDelete", null);

            using (repository)
            {
                ContentItem item = repository.Get(itemID);
                Assert.IsNotNull(item, "There should be a saved item.");
                repository.Delete(item);
                repository.Flush();
            }

            using (repository)
            {
                ContentItem item = repository.Get(itemID);
                Assert.IsNull(item, "Item is supposed to be deleted");
                repository.Flush();
            }
        }

        [Test]
        public void CanFindAll()
        {
            int item1ID = SaveAnItem("first", null);
            int item2ID = SaveAnItem("second", null);
            int item3ID = SaveAnItem("third", null);

            using (repository)
            {
                ICollection<ContentItem> items = repository.FindAll();
                Assert.AreEqual(3, items.Count);
                repository.Flush();
            }
        }

        [Test]
        public void CanFindGreaterThanID()
        {
            int item1ID = SaveAnItem("first", null);
            int item2ID = SaveAnItem("second", null);
            int item3ID = SaveAnItem("third", null);

            using (repository)
            {
                ICollection<ContentItem> items = repository.FindAll(NHibernate.Criterion.Expression.Gt("ID", 1));
                Assert.AreEqual(2, items.Count);
                repository.Flush();
            }
        }

        [Test]
        public void CanSaveDetail()
        {
            IRepository<N2.Details.ContentDetail> detailRepository = new NHRepository<N2.Details.ContentDetail>(sessionProvider);

            using (repository)
            {
                ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "item", null);
                item["TheString"] = "the string";
                repository.Save(item);
                repository.Flush();
            }

            using (detailRepository)
            {
                Assert.AreEqual(1, detailRepository.Count());
            }
        }

        [Test]
        public void CanDeleteDetail()
        {
            IRepository<ContentDetail> detailRepository = new NHRepository<ContentDetail>(sessionProvider);

            using (repository)
            {
                ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "item", null);
                item["TheString"] = "the string";
                repository.Save(item);
                repository.Flush();

                Assert.AreEqual(1, detailRepository.Count());

                item["TheString"] = null;
                repository.Save(item);
                repository.Flush();

                Assert.AreEqual(0, detailRepository.Count());
            }
        }

        [Test]
        public void DeleteItemCascadesDetails()
        {
            IRepository<ContentDetail> detailRepository = new NHRepository<ContentDetail>(sessionProvider);
            int itemID = 0;

            using (repository)
            {
                ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "item", null);
                item["TheString"] = "the string";
                repository.Save(item);
                repository.Flush();
                itemID = item.ID;
            }
            using(detailRepository)
            {
                Assert.AreEqual(1, detailRepository.Count());
            }
            using(repository)
            {
                ContentItem item = repository.Get(itemID);
                repository.Delete(item);
                repository.Flush();
            }
            using(detailRepository)
            {
                Assert.AreEqual(0, detailRepository.Count());
            }
        }

        [Test]
        public void ItemClasses_MayHaveNonVirtualProperties()
        {
            using (repository)
            {
                ContentItem item = CreateOneItem<Definitions.NonVirtualItem>(0, "item", null);
                repository.Save(item);
                repository.Flush();

                repository.Delete(item);
                repository.Flush();
            }
        }

        [Test]
        public void ItemClasses_MayHaveNonVirtualProperties_LazyLoading()
        {
            NonVirtualItem item;
            NonVirtualItem item2;
            NonVirtualItem item3;
            NonVirtualItem item4;
            NonVirtualItem item5;

            using (repository)
            {
                item = CreateOneItem<NonVirtualItem>(0, "item", null);
                item2 = CreateOneItem<NonVirtualItem>(0, "item2", item);
                item3 = CreateOneItem<NonVirtualItem>(0, "item3", item2);
                item4 = CreateOneItem<NonVirtualItem>(0, "item4", item3);
                item5 = CreateOneItem<NonVirtualItem>(0, "item5", item4);

                item.IntProperty = 1;
                item2.IntProperty = 2;
                item3.IntProperty = 3;
                item4.IntProperty = 4;
                item5.IntProperty = 5;

                repository.SaveOrUpdate(item, item2, item3, item4, item5);
            }

            using (repository)
            {
                Debug.WriteLine("A");
                
                Debug.WriteLine("one");
                item = repository.Get<NonVirtualItem>(item.ID);
                Debug.WriteLine("one.2");
                Assert.That(item.Name, Is.EqualTo("item"));
                Debug.WriteLine("one.3");
                Assert.That(item["IntProperty"], Is.EqualTo(1));

                Debug.WriteLine("two");
                Assert.That(item.Children.Count, Is.EqualTo(1));
                Debug.WriteLine("two.2");
                Assert.That(item.Children[0].Name, Is.EqualTo("item2"));
                Debug.WriteLine("two.3");
                Assert.That(item.Children[0]["IntProperty"], Is.EqualTo(2));

                Debug.WriteLine("three");
                Assert.That(item.Children[0].Children.Count, Is.EqualTo(1));
                Debug.WriteLine("three.2");
                Assert.That(item.Children[0].Children[0].Name, Is.EqualTo("item3"));
                Debug.WriteLine("three.3");
                Assert.That(item.Children[0].Children[0]["IntProperty"], Is.EqualTo(3));

                Debug.WriteLine("four");
                Assert.That(item.Children[0].Children[0].Children.Count, Is.EqualTo(1));
                Debug.WriteLine("four.2");
                Assert.That(item.Children[0].Children[0].Children[0].Name, Is.EqualTo("item4"));
                Debug.WriteLine("four.3");
                Assert.That(item.Children[0].Children[0].Children[0]["IntProperty"], Is.EqualTo(4));

                Debug.WriteLine("five");
                Assert.That(item.Children[0].Children[0].Children[0].Children.Count, Is.EqualTo(1));
                Debug.WriteLine("four.2");
                Assert.That(item.Children[0].Children[0].Children[0].Children[0].Name, Is.EqualTo("item5"));
                Debug.WriteLine("four.3");
                Assert.That(item.Children[0].Children[0].Children[0].Children[0]["IntProperty"], Is.EqualTo(5));
            }

            using (repository)
            {
                Debug.WriteLine("B");
                item4 = repository.Get<NonVirtualItem>(item4.ID);
                item = repository.Get<NonVirtualItem>(item.ID);
                item.LinkProperty = item4;
                repository.Save(item);
                repository.Flush();
            }

            using (repository)
            {
                Debug.WriteLine("C");
                item = repository.Get<NonVirtualItem>(item.ID);
                Assert.That(item.IntProperty, Is.EqualTo(1));
                Assert.That(item.LinkProperty, Is.EqualTo(item4));
                Assert.That(item.LinkProperty.Parent, Is.EqualTo(item3));
            }

            using (repository)
            {
                Debug.WriteLine("deleting");
                repository.DeleteRecursive(repository.Get(item.ID));
                repository.Flush();
            }
        }

        [Test, Ignore("Any way to do this?")]
        public void OtherSideOfReferenceIsRemoved()
        {
            int itemID = 0;
            int item2ID = 0;

            using (repository)
            {
                ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "item", null);
                ContentItem item2 = CreateOneItem<Definitions.PersistableItem>(0, "item2", null);
                item["reference"] = item2;
                repository.Save(item);
                repository.Save(item2);
                repository.Flush();
                itemID = item.ID;
                item2ID = item2.ID;
            }
            using (repository)
            {
                ContentItem item2 = repository.Get(item2ID);
                repository.Delete(item2);
                repository.Flush();
            }
            using (repository)
            {
                ContentItem item = repository.Get(itemID);
                Assert.IsNull(item["reference"], "Other side of reference has been deleted, detail should have been removed.");
                repository.Delete(item);
            }
        }

        [Test, Ignore("Any way to do this?")]
        public void CascadeDeletesWithReferences()
        {
            int itemID = 0;
            int item2ID = 0;
            int item3ID = 0;

            using (repository)
            {
                ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "item", null);
                ContentItem item2 = CreateOneItem<Definitions.PersistableItem>(0, "item2", item);
                ContentItem item3 = CreateOneItem<Definitions.PersistableItem>(0, "item3", item2);
                item["reference"] = item2;
                item3["reference"] = item2;
                repository.Save(item);
                repository.Save(item2);
                repository.Save(item3);
                repository.Flush();
                itemID = item.ID;
                item2ID = item2.ID;
                item3ID = item3.ID;
            }
            using (repository)
            {
                ContentItem item2 = repository.Get(item2ID);
                repository.Delete(item2);
                repository.Flush();
            }
            using (repository)
            {
                ContentItem item = repository.Get(itemID);
                Assert.IsNull(item["reference"], "other side of reference has been deleted, detail should have been removed.");

                ContentItem item3 = repository.Get(item3ID);
                Assert.IsNull(item3, "item3 should have been deleted by cascade");

                repository.Delete(item);
            }
        }

        [Test]
        public void FindDiscriminatorsBelow_FindsDistinctDiscriminators()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "root", null);
            ContentItem child = CreateOneItem<Definitions.PersistableItem>(0, "item", root);
            repository.Save(root);

            repository.FindDescendantDiscriminators(root).Single().Discriminator
                .ShouldBe("PersistableItem");
        }

        [Test]
        public void FindDiscriminators_FindsDistinctDiscriminators_Without()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "root", null);
            repository.Save(root);
            ContentItem child = CreateOneItem<Definitions.PersistableItem>(0, "item", null);
            repository.Save(child);

            var d = repository.FindDescendantDiscriminators(null).Single();
            d.Discriminator.ShouldBe("PersistableItem");
            d.Count.ShouldBe(2);
        }

        [Test]
        public void FindDiscriminatorsBelow_FindsAncestorDiscriminator()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            ContentItem child = CreateOneItem<Definitions.PersistablePart>(0, "part", root);
            repository.SaveOrUpdate(root, child);

            var discriminators = repository.FindDescendantDiscriminators(root).ToList();
            var discriminator = discriminators.First(d => d.Discriminator == "PersistablePart");
            discriminator.Count.ShouldBe(1);
        }

        [Test]
        public void FindDiscriminatorsBelow_FindsRootDiscriminator()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            ContentItem child = CreateOneItem<Definitions.PersistablePart>(0, "part", root);
            repository.SaveOrUpdate(root, child);

            var discriminators = repository.FindDescendantDiscriminators(root).ToList();
            var discriminator = discriminators.First(d => d.Discriminator == "PersistableItem");
            discriminator.Count.ShouldBe(1);
        }

        [Test]
        public void FindDiscriminator_Count()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "item1", root);
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "item2", root);
            repository.SaveOrUpdate(root, child1, child2);

            var discriminators = repository.FindDescendantDiscriminators(root).ToList();
            discriminators.Single().Count.ShouldBe(3);
        }

        [Test]
        public void FindDescendantDiscriminators_OnMultipleLevels()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "item1", root);
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "item2", child1);
            ContentItem child3 = CreateOneItem<Definitions.PersistableItem>(0, "item3", child2);
            repository.SaveOrUpdate(root, child1, child2, child3);

            var discriminators = repository.FindDescendantDiscriminators(root).ToList();
            discriminators.Single().Count.ShouldBe(4);
        }

        [Test]
        public void FindDescendantDiscriminators_IsSortedByNumberOfItmes()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistablePart>(0, "item1", root);
            ContentItem child2 = CreateOneItem<Definitions.PersistablePart>(0, "item2", child1);
            ContentItem child3 = CreateOneItem<Definitions.PersistablePart>(0, "item3", child2);
            repository.SaveOrUpdate(root, child1, child2, child3);

            var discriminators = repository.FindDescendantDiscriminators(root).ToList();
            discriminators[0].Discriminator.ShouldBe("PersistablePart");
            discriminators[1].Discriminator.ShouldBe("PersistableItem");
        }

        [Test]
        public void FindDescends_FindsAncestorOfType()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistablePart>(0, "item1", root);
            ContentItem child2 = CreateOneItem<Definitions.PersistablePart>(0, "item2", child1);
            ContentItem child3 = CreateOneItem<Definitions.PersistablePart>(0, "item3", child2);
            repository.SaveOrUpdate(root, child1, child2, child3);

            var discriminators = repository.FindDescendants(root, "PersistablePart");
            discriminators.Count().ShouldBe(3);
        }
        
        [Test]
        public void FindDescends_FindsDescendantsOfType()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistablePart>(0, "item1", root);
            ContentItem child2 = CreateOneItem<Definitions.PersistablePart>(0, "item2", child1);
            repository.SaveOrUpdate(root, child1, child2);

            var discriminators = repository.FindDescendants(root, "PersistableItem");
            discriminators.Count().ShouldBe(1);
        }

        [Test]
        public void FindDescends_WithNull_FinsAllInDb()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "item1", null);
            repository.SaveOrUpdate(root, child1);

            var discriminators = repository.FindDescendants(null, "PersistableItem");
            discriminators.Count().ShouldBe(2);
        }

        [Test]
        public void Find_TypeAndParent_ShouldOnlyInclude_ItemWithSpecified_TypeAndParent()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "page1", root);
            ContentItem child2 = CreateOneItem<Definitions.PersistablePart>(0, "part2", root);

            var results = repository.Find(new Parameter("class", "PersistableItem"), new Parameter("Parent", root));

            results.Single().ShouldBe(child1);
        }

        [Test]
        public void FindReferencing_ShouldReturn_ItemsThatLinkToTarget()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            var child1 = CreateOneItem<Definitions.PersistableItem>(0, "page1", root);
            var child2 = CreateOneItem<Definitions.PersistableItem>(0, "page2", root);

            child1["Link"] = child2;
            child2["Link"] = child1;

            repository.Save(root);
            repository.Flush();

            var results = repository.FindReferencing(child2);

            results.Single().ShouldBe(child1);
        }

        [Test]
        public void FindReferencing_ShouldReturn_ItemsThatLinkToTarget_InDetailCollection()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            var child1 = CreateOneItem<Definitions.PersistableItem>(0, "page1", root);
            var child2 = CreateOneItem<Definitions.PersistableItem>(0, "page2", root);

            child1.GetDetailCollection("Links", true).Add(child2);
            child2.GetDetailCollection("Links", true).Add(child1);

            repository.Save(root);
            repository.Flush();

            var results = repository.FindReferencing(child2);

            results.Single().ShouldBe(child1);
        }

        [Test]
        public void RemoveReferencesTo_ShouldRemove_LinkFromOtherItem()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            var child1 = CreateOneItem<Definitions.PersistableItem>(0, "page1", root);
            var child2 = CreateOneItem<Definitions.PersistableItem>(0, "page2", root);
            child1["Link"] = child2;
            child2["Link"] = child1;
            repository.Save(root);
            repository.Flush();

            repository.RemoveReferencesToRecursive(child2);

            child1["Link"].ShouldBe(null);
        }

        [Test]
        public void RemoveReferencesTo_ShouldRemove_LinkToDescendantItem_FromOtherItem()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            var child1 = CreateOneItem<Definitions.PersistableItem>(0, "page1", root);
            var grandchild1 = CreateOneItem<Definitions.PersistableItem>(0, "page1", child1);
            var child2 = CreateOneItem<Definitions.PersistableItem>(0, "page2", root);
            child1["Link"] = grandchild1;
            grandchild1["Link"] = grandchild1;
            child2["Link"] = grandchild1;
            repository.Save(root);
            repository.Flush();

            repository.RemoveReferencesToRecursive(child1);

            child2["Link"].ShouldBe(null);
        }

        [Test]
        public void RemoveReferencesTo_ShouldRemove_LinkToDescendantItem_FromItself()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            var child1 = CreateOneItem<Definitions.PersistableItem>(0, "page1", root);
            var grandchild1 = CreateOneItem<Definitions.PersistableItem>(0, "page1", child1);
            var child2 = CreateOneItem<Definitions.PersistableItem>(0, "page2", root);
            child1["Link"] = grandchild1;
            grandchild1["Link"] = grandchild1;
            child2["Link"] = grandchild1;
            repository.Save(root);
            repository.Flush();

            repository.RemoveReferencesToRecursive(child1);

            grandchild1["Link"].ShouldBe(null);
        }

        [Test]
        public void RemoveReferencesTo_ShouldRemove_LinkToDescendantItem_FromParent()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            var child1 = CreateOneItem<Definitions.PersistableItem>(0, "page1", root);
            var grandchild1 = CreateOneItem<Definitions.PersistableItem>(0, "page1", child1);
            var child2 = CreateOneItem<Definitions.PersistableItem>(0, "page2", root);
            child1["Link"] = grandchild1;
            grandchild1["Link"] = grandchild1;
            child2["Link"] = grandchild1;
            repository.Save(root);
            repository.Flush();

            repository.RemoveReferencesToRecursive(child1);

            child1["Link"].ShouldBe(null);
        }

        [Test]
        public void RemoveReferencesTo_ShouldShouldReturn_NumberOfRemovedReferences()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "page", null);
            var child1 = CreateOneItem<Definitions.PersistableItem>(0, "page1", root);
            var grandchild1 = CreateOneItem<Definitions.PersistableItem>(0, "page1", child1);
            var child2 = CreateOneItem<Definitions.PersistableItem>(0, "page2", root);
            child1["Link"] = grandchild1;
            grandchild1["Link"] = grandchild1;
            child2["Link"] = grandchild1;
            repository.Save(root);
            repository.Flush();

            int count = repository.RemoveReferencesToRecursive(child1);

            count.ShouldBe(3);
        }

        private int SaveAnItem(string name, ContentItem parent)
        {
            using (repository)
            {
                ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, name, parent);
                repository.Save(item);
                repository.Flush();
                return item.ID;
            }
        }
    }
}
