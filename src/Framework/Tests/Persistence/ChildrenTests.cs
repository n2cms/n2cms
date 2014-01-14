using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Tests.Persistence.Definitions;
using N2.Collections;
using NUnit.Framework;
using Shouldly;
using N2.Security;

namespace N2.Tests.Persistence
{
    [TestFixture]
    public class ChildrenTests : PersistenceAwareBase
    {
        private N2.Persistence.IPersister persister;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            CreateDatabaseSchema();

            persister = engine.Persister;
        }

        [Test]
        public void Save_ChildState()
        {
            var item = new PersistableItem();
            persister.Save(item);
            item.ChildState.ShouldBe(CollectionState.IsEmpty);
        }

        [Test]
        public void Save_WithChildren_ChildState()
        {
            var parent = new PersistableItem();
            persister.Save(parent);

            var child = new PersistableItem { Parent = parent };
            persister.Save(child);

            parent.ChildState.ShouldBe(CollectionState.ContainsVisiblePublicPages);
        }

        [Test]
        public void Save_WithChildren_DoubleSave()
        {
            var parent = new PersistableItem();
            persister.Save(parent);

            var child = new PersistableItem { Parent = parent };
            persister.Save(child);

            parent.Children.Count.ShouldBe(1);

            persister.Save(parent);

            parent.Children.Count.ShouldBe(1);
        }

        [Test]
        public void Save_WithDescendants()
        {
            var parent = new PersistableItem();
            persister.Save(parent);

            var child1 = new PersistableItem { Parent = parent };
            persister.Save(child1);

            var child1_1 = new PersistableItem { Parent = child1 };
            persister.Save(child1_1);

            var child2 = new PersistableItem { Parent = parent };
            persister.Save(child2);

            var child2_1 = new PersistableItem { Parent = child2 };
            persister.Save(child2_1);

            persister.Save(parent);

            Find.EnumerateChildren(parent).Count().ShouldBe(4);
        }

        [TestCase(1)]
        //[TestCase(10)]
        //[TestCase(100)]
        //[TestCase(1000)]
        //[TestCase(10000)]
        public void Save_ManyAssociations_InSession(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                var parent = new PersistableItem();
                persister.Save(parent);

                var child1 = new PersistableItem { Parent = parent };
                child1["parent"] = parent;
                persister.Save(child1);

                var child1_1 = new PersistableItem { Parent = child1 };
                child1_1["parent"] = child1_1;
                persister.Save(child1_1);

                var child2 = new PersistableItem { Parent = parent };
                child2["sibling"] = child1;
                persister.Save(child2);

                var child2_1 = new PersistableItem { Parent = child2 };
                child2_1["cousin"] = child1_1;
                persister.Save(child2_1);

                child1["child"] = child1_1;
                child1["sibling"] = child2;
                persister.Save(child1);

                child1_1["cousin"] = child2_1;
                persister.Save(child1_1);

                child2["child"] = child2_1;
                persister.Save(child2);

                Find.EnumerateChildren(parent).Count().ShouldBe(4);
            }
        }

        [Test]
        public void MultipleSessions()
        {
            for (int i = 0; i < 1; i++)
            {
                var parent = new PersistableItem();
                persister.Save(parent);

                persister.Dispose();
                parent = persister.Get<PersistableItem>(parent.ID);

                var child1 = new PersistableItem { Parent = parent };
                child1["parent"] = parent;
                persister.Save(child1);

                var child1_1 = new PersistableItem { Parent = child1 };
                child1_1["parent"] = child1_1;
                persister.Save(child1_1);

                persister.Dispose();

                parent = persister.Get<PersistableItem>(parent.ID);
                child1 = persister.Get<PersistableItem>(child1.ID);
                child1_1 = persister.Get<PersistableItem>(child1_1.ID);

                var child2 = new PersistableItem { Parent = parent };
                child2.ZoneName = "TheZone";
                child2["sibling"] = child1;
                persister.Save(child2);

                var child2_1 = new PersistableItem { Parent = child2 };
                child2_1.ZoneName = "TheZone";
                child2_1["cousin"] = child1_1;
                persister.Save(child2_1);

                persister.Dispose();

                parent = persister.Get<PersistableItem>(parent.ID);
                child1 = persister.Get<PersistableItem>(child1.ID);
                child1_1 = persister.Get<PersistableItem>(child1_1.ID);
                child2 = persister.Get<PersistableItem>(child2.ID);
                child2_1 = persister.Get<PersistableItem>(child2_1.ID);

                child1["child"] = child1_1;
                child1["sibling"] = child2;
                persister.Save(child1);

                child1_1["cousin"] = child2_1;
                persister.Save(child1_1);

                child2["child"] = child2_1;
                persister.Save(child2);

                persister.Dispose();

                persister.Get(parent.ID).Children.FindPages().Count().ShouldBe(1);
                persister.Get(parent.ID).Children.FindParts().Count().ShouldBe(1);

                persister.Get(parent.ID).Children.Select(c => c.ID).Contains(child1.ID).ShouldBe(true);
                persister.Get(parent.ID).Children.Select(c => c.ID).Contains(child2.ID).ShouldBe(true);

                persister.Get(child1.ID)["child"].ShouldBe(persister.Get(child1_1.ID));
            }
        }
    }
}
