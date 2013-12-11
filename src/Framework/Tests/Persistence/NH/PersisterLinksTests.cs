using NUnit.Framework;
using N2.Persistence;

namespace N2.Tests.Persistence.NH
{
    [TestFixture]
    public class PersisterLinksTests : PersisterTestsBase
    {
        [Test]
        public void Delete_ReferencedItem_ReferencesAreCleared()
        {
            ContentItem to, from;
            using (persister)
            {
                to = CreateOneItem<Definitions.PersistableItem>(0, "to", null);
                persister.Save(to);
                from = CreateOneItem<Definitions.PersistableItem>(0, "from", null);
                from["Reference"] = to;
                persister.Save(from);
            }
            using (persister)
            {
                to = persister.Get(to.ID);
                persister.Delete(to);
            }
            using (persister)
            {
                from = persister.Get(from.ID);
                Assert.That(from["Reference"], Is.Null);
            }
        }

        [Test]
        public void Delete_ParentOf_ReferencedItem_ReferencesAreCleared()
        {
            ContentItem parent, to, from;
            using (persister)
            {
                parent = CreateOneItem<Definitions.PersistableItem>(0, "parent", null);
                to = CreateOneItem<Definitions.PersistableItem>(0, "to", parent);
                persister.Repository.SaveOrUpdate(parent, to);
                from = CreateOneItem<Definitions.PersistableItem>(0, "from", null);
                from["Reference"] = to;
                persister.Save(from);
            }
            using (persister)
            {
                parent = persister.Get(parent.ID);
                persister.Delete(parent);
            }
            using (persister)
            {
                from = persister.Get(from.ID);
                Assert.That(from["Reference"], Is.Null);
            }
        }

        [Test]
        public void DeleteItem_ReferencedByVersion_ReferencesAreCleared()
        {
            ContentItem version, to, from;
            using (persister)
            {
                to = CreateOneItem<Definitions.PersistableItem>(0, "to", null);
                persister.Save(to);
                from = CreateOneItem<Definitions.PersistableItem>(0, "from", null);
                from["Reference"] = to;
                persister.Save(from);
                version = CreateOneItem<Definitions.PersistableItem>(0, "version", null);
                version.VersionOf = from;
                version["Reference"] = to;
                persister.Save(version);
            }
            using (persister)
            {
                to = persister.Get(to.ID);
                persister.Delete(to);
            }
            using (persister)
            {
                version = persister.Get(version.ID);
                Assert.That(version["Reference"], Is.Null);
            }
        }

        [Test]
        public void Delete_ReferencedItem_ReferencedByChild()
        {
            ContentItem to, from;
            using (persister)
            {
                to = CreateOneItem<Definitions.PersistableItem>(0, "to", null);
                persister.Save(to);
                from = CreateOneItem<Definitions.PersistableItem>(0, "from", to);
                from["Reference"] = to;
                persister.Save(from);
            }
            using (persister)
            {
                to = persister.Get(to.ID);
                persister.Delete(to);
            }
        }

        [Test]
        public void Delete_ReferencedItem_ReferencedByItself()
        {
            ContentItem to;
            using (persister)
            {
                to = CreateOneItem<Definitions.PersistableItem>(0, "to", null);
                to["Reference"] = to;
                persister.Save(to);
            }
            using (persister)
            {
                to = persister.Get(to.ID);
                persister.Delete(to);
            }
        }

        [Test]
        public void Delete_ReferencedItem_ReferencedByDetailCollection_DeletesReference()
        {
            ContentItem to, from;
            using (persister)
            {
                to = CreateOneItem<Definitions.PersistableItem>(0, "to", null);
                persister.Save(to);
                from = CreateOneItem<Definitions.PersistableItem>(0, "from", null);
                from.GetDetailCollection("References", true).Add(to);
                persister.Save(from);
            }
            using (persister)
            {
                to = persister.Get(to.ID);
                persister.Delete(to);
            }
            using (persister)
            {
                from = persister.Get(from.ID);
                Assert.That(from.GetDetailCollection("References", true).Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void Delete_ParentOf_ReferencedItem_ReferencedByDetailCollection_DeletesReference()
        {
            ContentItem parent, to, from;
            using (persister)
            {
                parent = CreateOneItem<Definitions.PersistableItem>(0, "parent", null);
                to = CreateOneItem<Definitions.PersistableItem>(0, "to", parent);
                persister.Repository.SaveOrUpdate(parent, to);
                from = CreateOneItem<Definitions.PersistableItem>(0, "from", null);
                from.GetDetailCollection("References", true).Add(to);
                persister.Save(from);
            }
            using (persister)
            {
                parent = persister.Get(parent.ID);
                persister.Delete(parent);
            }
            using (persister)
            {
                from = persister.Get(from.ID);
                Assert.That(from.GetDetailCollection("References", true).Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void SaveItem_WithReferences_DoesNotThrowSerializationException()
        {
            Definitions.PersistableItem parent, to, from;
            using (persister)
            {
                parent = CreateOneItem<Definitions.PersistableItem>(0, "parent", null);
                persister.Save(parent);
                to = CreateOneItem<Definitions.PersistableItem>(0, "to", parent);
                persister.Save(to);
                from = CreateOneItem<Definitions.PersistableItem>(0, "from", null);
                from.ContentLinks = new[] {to};
                persister.Save(from);
            }
        }
    }
}
