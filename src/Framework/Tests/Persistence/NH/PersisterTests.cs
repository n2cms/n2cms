using System;
using System.Linq;
using System.Diagnostics;
using N2.Tests.Persistence.Definitions;
using N2.Persistence;
using NUnit.Framework;
using Shouldly;

namespace N2.Tests.Persistence.NH
{
    [TestFixture]
    public class PersisterTests : PersisterTestsBase
    {
        [Test]
        public void VersionOf_MayBeAccessed()
        {
            var item = CreateOneItem<Definitions.PersistableItem>(0, "item1", null);
            var item2 = CreateOneItem<Definitions.PersistableItem>(0, "item2", null);
            persister.Save(item);
            item2.VersionOf = item;
            persister.Save(item2);

            persister.Dispose();

            var loaded = persister.Get(item2.ID);
            loaded.VersionOf.Value.ID.ShouldBe(item.ID);
        }

        [Test]
        public void Save_AssignsID()
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "saveableRoot", null);
            persister.Save(item);
            Assert.AreNotEqual(0, item.ID);
        }

        [Test]
        public void Properties_ArePersisted()
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "anitem", null);
            var props = typeof(ContentItem).GetProperties()
                .Where(p => p.CanWrite)
                .Where(p => GetExpectedValue(p.PropertyType) != null)
                .Where(p => p.Name != "ID")
                .Where(p => p.Name != "Updated")
                .Where(p => p.Name != "AncestralTrail")
                .ToArray();
            
            foreach (var pi in props)
            {
                pi.SetValue(item, GetExpectedValue(pi.PropertyType), null);
            }

            using (persister)
            {
                persister.Save(item);
            }

            using (persister)
            {
                var persistedItem = persister.Get(item.ID);
                foreach (var pi in props)
                {
                    object value = pi.GetValue(persistedItem, null);
                    object expected = GetExpectedValue(pi.PropertyType);
                    Assert.That(value, Is.EqualTo(expected), "Expected " + pi.Name + " to be " + expected + " but was " + value);
                }
            }
        }

        private static object GetExpectedValue(Type propertyType)
        {
            if (propertyType == typeof(string))
                return "Hello";
            else if (propertyType == typeof(int))
                return 11;
            else if (propertyType == typeof(int?))
                return 11;
            else if (propertyType == typeof(DateTime))
                return new DateTime(2010, 06, 16);
            else if (propertyType == typeof(DateTime?))
                return new DateTime(2010, 06, 16);
            else if (propertyType == typeof(bool))
                return true;
            else if (propertyType == typeof(N2.Collections.CollectionState))
                return N2.Collections.CollectionState.IsEmpty;
            else if (propertyType.IsEnum)
                return Enum.Parse(propertyType, Enum.GetNames(propertyType).First());
            
            return null;
        }

        [Test, Ignore]
        public void Get_Children_AreEagerlyFetched()
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "gettableRoot", null);
            ContentItem child = CreateOneItem<Definitions.PersistableItem>(0, "gettableChild", item);
            using (persister)
            {
                persister.Save(item);
            }

            ContentItem storedItem = persister.Get(item.ID);
            persister.Dispose();

            Assert.That(storedItem.Children.Count, Is.EqualTo(1));
        }

        [Test]
        public void SavingItemWithEmptyName_NameIsSetToNull()
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "", null);

            persister.Save(item);

            Assert.AreEqual(item.ID.ToString(), item.Name);
        }

        [Test]
        public void CanUpdate()
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "updatableRoot", null);

            using (persister)
            {
                item["someproperty"] = "hello";
                persister.Save(item);

                item["someproperty"] = "world";
                persister.Save(item);
            }
            using (persister)
            {
                item = persister.Get(item.ID);
                Assert.AreEqual("world", item["someproperty"]);
            }
        }

        [Test]
        public void CanDelete()
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "root", null);

            using (persister)
            {
                persister.Save(item);
                persister.Delete(item);
            }
            using (persister)
            {
                item = persister.Get(item.ID);
                Assert.IsNull(item, "Item should have been null.");
            }
        }

        [Test]
        public void CanMove()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "root", null);
            ContentItem item1 = CreateOneItem<Definitions.PersistableItem>(0, "item1", root);
            ContentItem item2 = CreateOneItem<Definitions.PersistableItem>(0, "item2", root);

            using (persister)
            {
                persister.Save(root);
                persister.Save(item1);
                persister.Save(item2);
            }

            using (persister)
            {
                root = persister.Get(root.ID);
                item1 = persister.Get(item1.ID);
                item2 = persister.Get(item2.ID);

                persister.Move(item2, item1);
            }

            using (persister)
            {
                root = persister.Get(root.ID);
                item1 = persister.Get(item1.ID);
                item2 = persister.Get(item2.ID);

                Assert.AreEqual(1, root.Children.Count);
                Assert.AreEqual(1, item1.Children.Count);
                Assert.AreEqual(item1, item2.Parent);
            }
        }

        [Test]
        public void Copy_ShouldCreate_CopyOfCopiedItem()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "root", null);
            ContentItem item1 = CreateOneItem<Definitions.PersistableItem>(0, "item1", root);
            ContentItem item2 = CreateOneItem<Definitions.PersistableItem>(0, "item2", root);

            using (persister)
            {
                persister.Save(root);
                persister.Save(item1);
                persister.Save(item2);
            }

            using (persister)
            {
                root = persister.Get(root.ID);
                item1 = persister.Get(item1.ID);
                item2 = persister.Get(item2.ID);

                persister.Copy(item2, item1);
            }

            using (persister)
            {
                root = persister.Get(root.ID);
                item1 = persister.Get(item1.ID);
                item2 = persister.Get(item2.ID);

                Assert.AreEqual(2, root.Children.Count);
                Assert.AreEqual(1, item1.Children.Count);
                Assert.AreNotEqual(root, item1.Children[0]);
                Assert.AreNotEqual(item1, item1.Children[0]);
                Assert.AreNotEqual(item2, item1.Children[0]);
            }
        }

        [Test]
        public void Copy_OfDeepHierarchy_ShouldCopyDescendants()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "root", null);
            ContentItem item1 = CreateOneItem<Definitions.PersistableItem>(0, "item1", root);
            ContentItem item2 = CreateOneItem<Definitions.PersistableItem>(0, "item2", item1);
            ContentItem item3 = CreateOneItem<Definitions.PersistableItem>(0, "item3", item2);
            ContentItem item4 = CreateOneItem<Definitions.PersistableItem>(0, "item4", item3);
            ContentItem copy = null;

            using (persister)
            {
                persister.Repository.SaveOrUpdate(root, item1, item2, item3, item4);
            }

            using (persister)
            {
                item1 = persister.Get(item1.ID);

                copy = persister.Copy(item1, item1);
            }

            using (persister)
            {
                copy = persister.Get(copy.ID);
                
                Assert.That(copy.Name, Is.EqualTo(item1.Name));
                Assert.That(copy.Parent, Is.EqualTo(item1));
                Assert.That(copy.Children[0].Name, Is.EqualTo(item2.Name));
                Assert.That(copy.Children[0].ID, Is.Not.EqualTo(item2.ID));
                Assert.That(copy.Children[0].Children[0].Name, Is.EqualTo(item3.Name));
                Assert.That(copy.Children[0].Children[0].ID, Is.Not.EqualTo(item3.ID));
                Assert.That(copy.Children[0].Children[0].Children[0].Name, Is.EqualTo(item4.Name));
                Assert.That(copy.Children[0].Children[0].Children[0].ID, Is.Not.EqualTo(item4.ID));
            }
        }

        [Test]
        public void Copy_IgnoringChildren_DoesntCopyChildren()
        {
            ContentItem root = CreateOneItem<Definitions.PersistableItem>(0, "root", null);
            ContentItem item1 = CreateOneItem<Definitions.PersistableItem>(0, "item1", root);
            ContentItem item2 = CreateOneItem<Definitions.PersistableItem>(0, "item2", item1);
            ContentItem copy = null;

            using (persister)
            {
                persister.Repository.SaveOrUpdate(root, item1, item2);
            }

            using (persister)
            {
                item1 = persister.Get(item1.ID);

                copy = persister.Copy(item1, item1, false);
            }

            using (persister)
            {
                copy = persister.Get(copy.ID);

                Assert.That(copy.Name, Is.EqualTo(item1.Name));
                Assert.That(copy.Parent, Is.EqualTo(item1));
                Assert.That(copy.Children.Count, Is.EqualTo(0));
            }
        }



        [Test]
        public void CanChange_SaveAction()
        {
            ContentItem itemToSave = CreateOneItem<Definitions.PersistableItem>(0, "root", null);

            using (persister)
            {
                ContentItem invokedItem = null;
                EventHandler<CancellableItemEventArgs> handler = delegate(object sender, CancellableItemEventArgs e)
                {
                    e.FinalAction = delegate(ContentItem item) { invokedItem = item; };
                };
                persister.ItemSaving += handler;
                persister.Save(itemToSave);
                persister.ItemSaving -= handler;

                Assert.That(invokedItem, Is.EqualTo(itemToSave));
            }
        }

        [Test]
        public void CanChange_DeleteAction()
        {
            ContentItem itemToDelete = CreateOneItem<Definitions.PersistableItem>(0, "root", null);

            using (persister)
            {
                ContentItem invokedItem = null;
                EventHandler<CancellableItemEventArgs> handler = delegate(object sender, CancellableItemEventArgs e)
                {
                    e.FinalAction = delegate(ContentItem item) { invokedItem = item; };
                };
                persister.ItemDeleting += handler;
                persister.Delete(itemToDelete);
                persister.ItemDeleting -= handler;

                Assert.That(invokedItem, Is.EqualTo(itemToDelete));
            }
        }

        [Test]
        public void CanChange_MoveAction()
        {
            ContentItem source = CreateOneItem<Definitions.PersistableItem>(0, "source", null);
            ContentItem destination = CreateOneItem<Definitions.PersistableItem>(0, "destination", null);

            using (persister)
            {
                ContentItem invokedFrom = null;
                ContentItem invokedTo = null;
                EventHandler<CancellableDestinationEventArgs> handler = delegate(object sender, CancellableDestinationEventArgs e)
                {
                    e.FinalAction = delegate(ContentItem from, ContentItem to)
                    {
                        invokedFrom = from;
                        invokedTo = to;
                        return null;
                    };
                };
                persister.ItemMoving += handler;
                persister.Move(source, destination);
                persister.ItemMoving -= handler;

                Assert.That(invokedFrom, Is.EqualTo(source));
                Assert.That(invokedTo, Is.EqualTo(destination));
            }
        }

        [Test]
        public void CanChange_CopyAction()
        {
            ContentItem source = CreateOneItem<Definitions.PersistableItem>(0, "source", null);
            ContentItem destination = CreateOneItem<Definitions.PersistableItem>(0, "destination", null);

            using (persister)
            {
                ContentItem invokedFrom = null;
                ContentItem invokedTo = null;
                ContentItem copyToReturn = CreateOneItem<Definitions.PersistableItem>(0, "copied", null);
                EventHandler<CancellableDestinationEventArgs> handler = delegate(object sender, CancellableDestinationEventArgs e)
                {
                    e.FinalAction = delegate(ContentItem from, ContentItem to)
                    {
                        invokedFrom = from;
                        invokedTo = to;
                        return copyToReturn;
                    };
                };
                persister.ItemCopying += handler;
                ContentItem copy = persister.Copy(source, destination);
                persister.ItemCopying -= handler;

                Assert.That(copy, Is.SameAs(copyToReturn));
                Assert.That(invokedFrom, Is.EqualTo(source));
                Assert.That(invokedTo, Is.EqualTo(destination));
            }
        }

        [Test]
        public void CanSave_Guid()
        {
            PersistableItem item = CreateOneItem<PersistableItem>(0, "root", null);
            PersistableItem fromDB = null;

            item.GuidProperty = Guid.NewGuid();
            using (persister)
            {
                persister.Save(item);
            }

            fromDB = persister.Get<PersistableItem>(item.ID);

            Assert.That(fromDB.GuidProperty, Is.EqualTo(item.GuidProperty));
        }

        [Test]
        public void CanSave_ReadOnlyGuid()
        {
            PersistableItem item = CreateOneItem<PersistableItem>(0, "root", null);
            PersistableItem fromDB = null;
            string guid = item.ReadOnlyGuid;

            using (persister)
            {
                persister.Save(item);
            }

            fromDB = persister.Get<PersistableItem>(item.ID);

            Assert.That(fromDB.ReadOnlyGuid, Is.EqualTo(guid));
        }

        [Test]
        public void CanSave_WritableGuid()
        {
            PersistableItem item = CreateOneItem<PersistableItem>(0, "root", null);
            PersistableItem fromDB = null;

            string guid = item.WritableGuid;
            item.WritableGuid = guid;
            using (persister)
            {
                persister.Save(item);
            }

            fromDB = persister.Get<PersistableItem>(item.ID);

            Assert.That(fromDB.WritableGuid, Is.EqualTo(guid));
        }

        [Test]
        public void Laziness()
        {
            ContentItem root = CreateOneItem<PersistableItem>(0, "root", null);
            ContentItem root2 = CreateOneItem<PersistableItem>(0, "root2", null);
            for (int i = 0; i < 30; i++)
            {
                PersistableItem item = CreateOneItem<PersistableItem>(0, "item", root);
            }
            using (persister)
            {
                persister.Save(root);
                persister.Save(root2);
            }
            using (persister)
            {
                root = persister.Get(root.ID);
                Debug.WriteLine("Got: " + root + " with Children.Count: " + root.Children.Count);
                foreach (var child in root.Children)
                {
                }
                root2 = persister.Get(root2.ID);
                Debug.WriteLine("Got: " + root2 + " with Children.Count: " + root2.Children.Count);
                foreach (var child in root2.Children)
                {
                }
            }
        }

        [Test]
        public void Save_CausesSortOrder_ToBeUpdated()
        {
            ContentItem parent = CreateOneItem<Definitions.PersistableItem>(0, "parent", null);
            persister.Save(parent);

            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "child1", parent);
            persister.Save(child1);
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "child2", parent);
            persister.Save(child2);
            ContentItem child3 = CreateOneItem<Definitions.PersistableItem>(0, "child3", parent);
            persister.Save(child3);

            Assert.That(child1.SortOrder, Is.LessThan(child2.SortOrder));
            Assert.That(child2.SortOrder, Is.LessThan(child3.SortOrder));
        }

        [Test]
        public void Save_OnParentWith_SortChildrenByUnordered_CausesSortOrder_NotToBeUpdated()
        {
            ContentItem parent = CreateOneItem<Definitions.NonVirtualItem>(0, "parent", null);
            persister.Save(parent);

            ContentItem child1 = CreateOneItem<Definitions.NonVirtualItem>(0, "child1", parent);
            persister.Save(child1);
            ContentItem child2 = CreateOneItem<Definitions.NonVirtualItem>(0, "child2", parent);
            persister.Save(child2);
            ContentItem child3 = CreateOneItem<Definitions.NonVirtualItem>(0, "child3", parent);
            persister.Save(child3);

            Assert.That(child1.SortOrder, Is.EqualTo(0));
            Assert.That(child2.SortOrder, Is.EqualTo(0));
            Assert.That(child3.SortOrder, Is.EqualTo(0));
        }

        [Test]
        public void Save_OnParentWith_SortChildren_ByExpression_NameDesc_CausesChildrenToBeReordered()
        {
            ContentItem parent = CreateOneItem<Definitions.PersistableItem2>(0, "parent", null);
            persister.Save(parent);

            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "child1", parent);
            persister.Save(child1);
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "child2", parent);
            persister.Save(child2);
            ContentItem child3 = CreateOneItem<Definitions.PersistableItem>(0, "child3", parent);
            persister.Save(child3);

            Assert.That(child1.SortOrder, Is.GreaterThan(child2.SortOrder));
            Assert.That(child2.SortOrder, Is.GreaterThan(child3.SortOrder));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Children_CanBePaged(bool forceInitialize)
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "gettableRoot", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "one", item);
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "two", item);
            ContentItem child3 = CreateOneItem<Definitions.PersistableItem>(0, "three", item);
            using (persister)
            {
                persister.Repository.SaveOrUpdate(item, child1, child2, child3);
            }

            using (persister)
            {
                item = persister.Get(item.ID);

                if (forceInitialize)
                {
                    var temp = item.Children[0]; // initilze
                }

                var first = item.Children.FindRange(0, 1).ToList();
                var second = item.Children.FindRange(1, 1).ToList();
                var third = item.Children.FindRange(2, 1).ToList();
                var none = item.Children.FindRange(3, 1).ToList();
                var beginning = item.Children.FindRange(0, 2).ToList();
                var ending = item.Children.FindRange(2, 2).ToList();

                Assert.That(first.Single(), Is.EqualTo(child1));
                Assert.That(second.Single(), Is.EqualTo(child2));
                Assert.That(third.Single(), Is.EqualTo(child3));
                Assert.That(none.Any(), Is.False);
                Assert.That(beginning.Count(), Is.EqualTo(2));
                Assert.That(beginning.First(), Is.EqualTo(child1));
                Assert.That(beginning.Last(), Is.EqualTo(child2));
                Assert.That(ending.Count(), Is.EqualTo(1));
                Assert.That(ending.First(), Is.EqualTo(child3));

                Assert.That(item.Children.Count, Is.EqualTo(3));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Children_CanBe_FoundByZone(bool forceInitialize)
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "gettableRoot", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "one", item);
            child1.ZoneName = "First";
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "two", item);
            child2.ZoneName = "Second";
            ContentItem child3 = CreateOneItem<Definitions.PersistableItem>(0, "three", item);
            using (persister)
            {
                persister.Repository.SaveOrUpdate(item, child1, child2, child3);
            }

            using (persister)
            {
                item = persister.Get(item.ID);

                if (forceInitialize)
                {
                    var temp = item.Children[0]; // initilze
                }

                var nozone = item.Children.FindParts(null);
                var emptyzone = item.Children.FindParts("");
                var first = item.Children.FindParts("First");
                var second = item.Children.FindParts("Second");
                var third = item.Children.FindParts("Third");

                Assert.That(nozone.Single(), Is.EqualTo(child3));
                Assert.That(emptyzone.Count(), Is.EqualTo(0));
                Assert.That(first.Single(), Is.EqualTo(child1));
                Assert.That(second.Single(), Is.EqualTo(child2));
                Assert.That(third.Count(), Is.EqualTo(0));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ZoneNames_CanBeFound(bool forceInitialize)
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "gettableRoot", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "one", item);
            child1.ZoneName = "TheZone";
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "two", item);
            child2.ZoneName = "TheZone";
            ContentItem child3 = CreateOneItem<Definitions.PersistableItem>(0, "three", item);
            using (persister)
            {
                persister.Repository.SaveOrUpdate(item, child1, child2, child3);
            }

            using (persister)
            {
                item = persister.Get(item.ID);

                if (forceInitialize)
                {
                    var temp = item.Children[0]; // initilze
                }

                var zones = item.Children.FindZoneNames();

                Assert.That(zones.Count(), Is.EqualTo(2));
                Assert.That(zones.Contains(null));
                Assert.That(zones.Contains("TheZone"));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void FindPages_ReturnsPages_NotInZone(bool forceInitialize)
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "gettableRoot", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "one", item);
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "two", item);
            child2.ZoneName = "Zone";
            using (persister)
            {
                persister.Repository.SaveOrUpdate(item, child1, child2);
            }

            using (persister)
            {
                item = persister.Get(item.ID);

                if (forceInitialize)
                {
                    var temp = item.Children[0]; // initialize
                }

                var pages = item.Children.FindPages();

                Assert.That(pages.Single(), Is.EqualTo(child1));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void FindNavigatablePages_ReturnsPages_NotInZone(bool forceInitialize)
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "gettableRoot", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "one", item);
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "two", item);
            child2.ZoneName = "Zone";
            using (persister)
            {
                persister.Repository.SaveOrUpdate(item, child1, child2);
            }

            using (persister)
            {
                item = persister.Get(item.ID);
                if (forceInitialize)
                {
                    var temp = item.Children[0]; // initialize
                }

                var pages = item.Children.FindNavigatablePages();

                Assert.That(pages.Single(), Is.EqualTo(child1));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void FindNavigatablePages_ReturnsPages_ThatAreVisible(bool forceInitialize)
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "gettableRoot", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "one", item);
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "two", item);
            child1.Visible = false;
            using (persister)
            {
                persister.Repository.SaveOrUpdate(item, child1, child2);
            }

            using (persister)
            {
                item = persister.Get(item.ID);
                if (forceInitialize)
                {
                    var temp = item.Children[0]; // initialize
                }

                var pages = item.Children.FindNavigatablePages();

                Assert.That(pages.Single(), Is.EqualTo(child2));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void FindNavigatablePages_ReturnsPages_ThatArePublished(bool forceInitialize)
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "gettableRoot", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "one", item);
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "two", item);
            child2.Expires = N2.Utility.CurrentTime().AddSeconds(-10);
            child2.State = ContentState.Unpublished;
            using (persister)
            {
                persister.Repository.SaveOrUpdate(item, child1, child2);
            }

            using (persister)
            {
                item = persister.Get(item.ID);
                if (forceInitialize)
                {
                    var temp = item.Children[0]; // initialize
                }

                var pages = item.Children.FindNavigatablePages();

                Assert.That(pages.Single(), Is.EqualTo(child1));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Children_WhichAreParts_CanBeFound(bool forceInitialize)
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "gettableRoot", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "one", item);
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "two", item);
            child2.ZoneName = "Zone";
            using (persister)
            {
                persister.Repository.SaveOrUpdate(item, child1, child2);
            }

            using (persister)
            {
                item = persister.Get(item.ID);

                if (forceInitialize)
                {
                    var temp = item.Children[0]; // initilze
                }

                var pages = item.Children.FindParts();

                Assert.That(pages.Single(), Is.EqualTo(child2));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Children_CanBe_FoundByName(bool forceInitialize)
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "root", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "one", item);
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "two", item);
            ContentItem child3 = CreateOneItem<Definitions.PersistableItem>(0, "three", item);
            using (persister)
            {
                persister.Repository.SaveOrUpdate(item, child1, child2, child3);
            }

            using (persister)
            {
                item = persister.Get(item.ID);

                if (forceInitialize)
                {
                    var temp = item.Children[0]; // initilze
                }

                var nullname = item.Children.FindNamed(null);
                var emptyname = item.Children.FindNamed("");
                var rootname = item.Children.FindNamed("root");
                var first = item.Children.FindNamed("one");
                var second = item.Children.FindNamed("two");
                var third = item.Children.FindNamed("three");
                
                Assert.That(nullname, Is.Null);
                Assert.That(emptyname, Is.Null);
                Assert.That(rootname, Is.Null);
                Assert.That(first, Is.EqualTo(child1));
                Assert.That(second, Is.EqualTo(child2));
                Assert.That(third, Is.EqualTo(child3));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Children_CanBeQueried(bool forceInitialize)
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "root", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "one", item);
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "two", item);
            ContentItem child3 = CreateOneItem<Definitions.PersistableItem>(0, "three", item);
            using (persister)
            {
                persister.Repository.SaveOrUpdate(item, child1, child2, child3);
            }

            using (persister)
            {
                item = persister.Get(item.ID);

                if (forceInitialize)
                {
                    var temp = item.Children[0]; // initilze
                }

                var one = item.Children.Query().Where(i => i.Name == "one").ToList();
                var notone = item.Children.Query().Where(i => i.Name != "one").ToList();
                var containso = item.Children.Query().Where(i => i.Name.Contains("o")).ToList();

                Assert.That(one.Single(), Is.EqualTo(child1));
                Assert.That(notone.Count(), Is.EqualTo(2));
                Assert.That(notone.Any(i => i == child1), Is.False);
                Assert.That(containso.Count(), Is.EqualTo(2));
                Assert.That(containso.Contains(child1), Is.True);
                Assert.That(containso.Contains(child2), Is.True);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Children_Find(bool forceInitialize)
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "root", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "one", item);
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "two", item);
            ContentItem child3 = CreateOneItem<Definitions.PersistableItem>(0, "three", item);
            using (persister)
            {
                persister.Repository.SaveOrUpdate(item, child1, child2, child3);
            }

            using (persister)
            {
                item = persister.Get(item.ID);

                if (forceInitialize)
                {
                    var temp = item.Children[0]; // initilze
                }

                var one = item.Children.Find(Parameter.Equal("Name", "one")).ToList();
                var notone = item.Children.Find(Parameter.NotEqual("Name", "one")).ToList();
                var containso = item.Children.Find(Parameter.Like("Name", "%o%")).ToList();

                Assert.That(one.Single(), Is.EqualTo(child1));
                Assert.That(notone.Count(), Is.EqualTo(2));
                Assert.That(notone.Any(i => i == child1), Is.False);
                Assert.That(containso.Count(), Is.EqualTo(2));
                Assert.That(containso.Contains(child1), Is.True);
                Assert.That(containso.Contains(child2), Is.True);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Children_Select(bool forceInitialize)
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "root", null);
            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "one", item);
            ContentItem child2 = CreateOneItem<Definitions.PersistableItem>(0, "two", item);
            ContentItem child3 = CreateOneItem<Definitions.PersistableItem>(0, "three", item);
            using (persister)
            {
                persister.Repository.SaveOrUpdate(item, child1, child2, child3);
            }

            using (persister)
            {
                item = persister.Get(item.ID);

                if (forceInitialize)
                {
                    var temp = item.Children[0]; // initilze
                }

                var one = item.Children.Select(Parameter.Equal("Name", "one"), "Name").ToList();
                var notone = item.Children.Select(Parameter.NotEqual("Name", "one"), "Name", "Title").ToList();
                var containso = item.Children.Select(Parameter.Like("Name", "%o%"), "ID").ToList();

                one.Single()["Name"].ShouldBe(child1.Name);
                notone.Count().ShouldBe(2);
                notone.Any(i => i["Title"].ToString() == child1.Title).ShouldBe(false);
                notone.Any(i => i["Name"].ToString() == child1.Name).ShouldBe(false);
                containso.Count().ShouldBe(2);
                containso.Select(i => i.Values.Single()).Contains(child1.ID);
                containso.Select(i => i.Values.Single()).Contains(child2.ID);
            }
        }

        [Test]
        public void AddSingleChild()
        {
            ContentItem item = CreateOneItem<Definitions.PersistableItem>(0, "root", null);
            persister.Save(item);

            ContentItem child1 = CreateOneItem<Definitions.PersistableItem>(0, "one", item);

            new N2.Definitions.SortChildrenAttribute(N2.Definitions.SortBy.CurrentOrder).OnSavingChild(new N2.Persistence.Behaviors.BehaviorContext { Action = "Saving", AffectedItem = child1, Parent = item });

            persister.Save(child1);

            persister.Dispose();

            persister.Get(child1.ID);
        }
    }
}
