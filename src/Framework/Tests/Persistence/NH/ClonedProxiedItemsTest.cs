using N2.Tests.Persistence.Proxying;
using N2.Persistence;
using NUnit.Framework;

namespace N2.Tests.Persistence.NH
{
    [TestFixture]
    public class ClonedProxiedItemsTest : PersisterTestsBase
    {
        [TestFixtureSetUp]
        public override void TestFixtureSetup()
        {
            base.persistedTypes = new[] { typeof(Definitions.PersistableItem), typeof(InterceptableInheritorItem) };
            base.TestFixtureSetup();
        }

        [Test]
        public void CanPersistDetails_OnClonedItem()
        {
            ContentItem item1, item2;
            using (persister)
            {
                item1 = CreateOneItem<Definitions.PersistableItem>(0, "item", null);
                item1["Hello"] = "World";
                persister.Save(item1);

                item2 = item1.Clone(false);
                persister.Save(item2);
            }
            using (persister)
            {
                item2 = persister.Get(item2.ID);
                Assert.That(item2.ID, Is.Not.EqualTo(item1.ID));
                Assert.That(item2["Hello"], Is.EqualTo("World"));
            }
        }

        [Test]
        public void CanPersistDetails_OnClonedItems_ThatAreProxied()
        {
            InterceptableInheritorItem item1, item2;
            using (persister)
            {
                item1 = proxyFactory.Create(typeof(InterceptableInheritorItem).FullName, 0) as InterceptableInheritorItem;
                item1.Name = "Hello";
                item1.StringProperty = "Howdy";
                item1["Hello"] = "World";

                persister.Save(item1);

                item2 = (InterceptableInheritorItem)item1.Clone(false);
                persister.Save(item2);
                var details = item2.Details;
                var sp = details["StringProperty"];
                var id = sp.ID;
                var value = sp.Value;
            }
            using (persister)
            {
                item2 = persister.Get<InterceptableInheritorItem>(item2.ID);
                Assert.That(item2.ID, Is.Not.EqualTo(item1.ID));
                Assert.That(item2["Hello"], Is.EqualTo("World"));
                Assert.That(item2.StringProperty, Is.EqualTo("Howdy"));
                Assert.That(item2.Details["StringProperty"].StringValue, Is.EqualTo("Howdy"));
            }
        }

        [Test]
        public void CanPersistChildren_OnClonedItems_ThatAreProxied()
        {
            InterceptableInheritorItem item1, item2, clone, clonedChild;
            using (persister)
            {
                item1 = proxyFactory.Create(typeof(InterceptableInheritorItem).FullName, 0) as InterceptableInheritorItem;
                item2 = proxyFactory.Create(typeof(InterceptableInheritorItem).FullName, 0) as InterceptableInheritorItem;
                item2.Name = "Hello";
                item2.StringProperty = "Howdy";
                item2["Hello"] = "World";
                item2.AddTo(item1);

                persister.Repository.SaveOrUpdate(item1, item2);

                clone = (InterceptableInheritorItem)item1.Clone(true);
                clonedChild = (InterceptableInheritorItem)clone.Children[0];
                persister.Repository.SaveOrUpdate(clone, clonedChild);
            }
            using (persister)
            {
                var child = persister.Get<InterceptableInheritorItem>(clonedChild.ID);
                Assert.That(child.ID, Is.Not.EqualTo(item1.ID));
                Assert.That(child["Hello"], Is.EqualTo("World"));
                Assert.That(child.StringProperty, Is.EqualTo("Howdy"));
                Assert.That(child.Details["StringProperty"].StringValue, Is.EqualTo("Howdy"));
            }
        }
    }
}
