using System;
using N2.Details;
using N2.Persistence;
using N2.Persistence.Proxying;
using NUnit.Framework;
using N2.Persistence.Finder;
using System.Linq;
using N2.Persistence.NH.Finder;

namespace N2.Tests.Persistence.NH
{
    #region class PropertyItemTypes
    public class PropertyItemInheritor1 : PropertyItemType
    {
    }
    public class PropertyItemType : ContentItem
    {
        [Persistable(Length = 50)]
        public virtual string ShortStringProperty { get; set; }

        [Persistable]
        public virtual string LongStringProperty { get; set; }

        [Persistable]
        public virtual int IntegerProperty { get; set; }
        [Persistable]
        public virtual int? NullableIntegerProperty { get; set; }

        [Persistable]
        public virtual bool BooleanProperty { get; set; }
        [Persistable]
        public virtual bool? NullableBooleanProperty { get; set; }

        [Persistable]
        public virtual DateTime DateTimeProperty { get; set; }
        [Persistable]
        public virtual DateTime? NullableDateTimeProperty { get; set; }

        [Persistable]
        public virtual double DoubleProperty { get; set; }
        [Persistable]
        public virtual double? NullableDoubleProperty { get; set; }

        [Persistable]
        public virtual ContentItem LinkProperty { get; set; }

        [Persistable, EditableText]
        public virtual string PersistableEditableProperty { get; set; }
    }
    #endregion

    [TestFixture]
    public class MappingGeneratorTests : PersisterTestsBase
    {
        [TestFixtureSetUp]
        public override void TestFixtureSetup()
        {
            InterceptingProxyFactory proxyFactory;
            ItemFinder finder;
            TestSupport.Setup(out definitions, out activator, out notifier, out sessionProvider, out finder, out schemaCreator, out proxyFactory, typeof(PropertyItemType), typeof(PropertyItemInheritor1));
        }

        // string

        [Test]
        public void StringProperty()
        {
            SaveLoadAndCompare<string>(
                (i) => i.LongStringProperty, 
                (i) => { i.LongStringProperty = "Will it blend? ABCDEFGHIJKLMNOPQRSTUVWXYZ ABCDEFGHIJKLMNOPQRSTUVWXYZ ABCDEFGHIJKLMNOPQRSTUVWXYZ ABCDEFGHIJKLMNOPQRSTUVWXYZ ABCDEFGHIJKLMNOPQRSTUVWXYZ"; });
        }

        [Test]
        public void StringProperty_WithMaxLength()
        {
            SaveLoadAndCompare<string>(
                (i) => i.ShortStringProperty, 
                (i) => { i.ShortStringProperty = "Will it blend?"; });
        }

        // integer

        [Test]
        public void IntegerProperty()
        {
            SaveLoadAndCompare<int>(
                (i) => i.IntegerProperty,
                (i) => { i.IntegerProperty = 432; });
        }

        [Test]
        public void IntegerProperty_SetNullable()
        {
            SaveLoadAndCompare<int?>(
                (i) => i.NullableIntegerProperty,
                (i) => { i.NullableIntegerProperty = 432; });
        }

        [Test]
        public void IntegerProperty_StoreNull()
        {
            SaveLoadAndCompare<int?>(
                (i) => i.NullableIntegerProperty,
                (i) => { i.NullableIntegerProperty = null; });
        }

        // boolean

        [Test]
        public void BooleanProperty()
        {
            SaveLoadAndCompare<bool>(
                (i) => i.BooleanProperty,
                (i) => { i.BooleanProperty = true; });
        }

        [Test]
        public void BooleanProperty_SetNullable()
        {
            SaveLoadAndCompare<bool?>(
                (i) => i.NullableBooleanProperty,
                (i) => { i.NullableBooleanProperty = true; });
        }

        [Test]
        public void BooleanProperty_StoreNull()
        {
            SaveLoadAndCompare<bool?>(
                (i) => i.NullableBooleanProperty,
                (i) => { i.NullableBooleanProperty = null; });
        }

        // date time

        [Test]
        public void DateTimeProperty()
        {
            SaveLoadAndCompare<DateTime>(
                (i) => i.DateTimeProperty,
                (i) => { i.DateTimeProperty = N2.Utility.CurrentTime().StripMilliseconds(); });
        }

        [Test]
        public void DateTimeProperty_SetNullable()
        {
            SaveLoadAndCompare<DateTime?>(
                (i) => i.NullableDateTimeProperty,
                (i) => { i.NullableDateTimeProperty = N2.Utility.CurrentTime().StripMilliseconds(); });
        }

        [Test]
        public void DateTimeProperty_StoreNull()
        {
            SaveLoadAndCompare<DateTime?>(
                (i) => i.NullableDateTimeProperty,
                (i) => { i.NullableDateTimeProperty = N2.Utility.CurrentTime().StripMilliseconds(); });
        }

        // double

        [Test]
        public void DoubleProperty()
        {
            SaveLoadAndCompare<double>(
                (i) => i.DoubleProperty,
                (i) => { i.DoubleProperty = 432.1; });
        }

        [Test]
        public void DoubleProperty_SetNullable()
        {
            SaveLoadAndCompare<double?>(
                (i) => i.NullableDoubleProperty,
                (i) => { i.NullableDoubleProperty = 432.1; });
        }

        [Test]
        public void DoubleProperty_StoreNull()
        {
            SaveLoadAndCompare<double?>(
                (i) => i.NullableDoubleProperty,
                (i) => { i.NullableDoubleProperty = null; });
        }

        [Test]
        public void Assigning_AttachedEntity_DoesntCreateDetails()
        {
            var item = CreateOneItem<PropertyItemType>(0, "item2", null);
            using (persister)
            {
                persister.Save(item);
            }

            using (persister)
            {
                var fromDb = persister.Get<PropertyItemType>(item.ID);
                fromDb.PersistableEditableProperty = "hello world";

                Assert.That(fromDb.Details.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void Saving_AttachedEntity_DoesntCreateDetails()
        {
            var item = CreateOneItem<PropertyItemType>(0, "item2", null);
            using (persister)
            {
                persister.Save(item);
            }

            using (persister)
            {
                var fromDb = persister.Get<PropertyItemType>(item.ID);
                fromDb.PersistableEditableProperty = "hello world";
                persister.Save(fromDb);
            }

            using (persister)
            {
                var fromDb = persister.Get<PropertyItemType>(item.ID);

                Assert.That(fromDb.Details.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void AllOfEm()
        {
            var linked = CreateOneItem<PropertyItemType>(0, "item1", null);
            var item = CreateOneItem<PropertyItemType>(0, "item2", null);
            item.BooleanProperty = true;
            item.DateTimeProperty = new DateTime(2010, 06, 18, 14, 30, 00);
            item.DoubleProperty = 555.555;
            item.IntegerProperty = 555;
            item.LinkProperty = linked;
            item.ShortStringProperty = "in table text";
            item.LongStringProperty = "long table text";
            item.PersistableEditableProperty = "hello world";

            using (persister)
            {
                persister.Save(linked);
                persister.Save(item);
            }

            using (persister)
            {
                var fromDb = persister.Get<PropertyItemType>(item.ID);
                Assert.That(fromDb.BooleanProperty, Is.EqualTo(item.BooleanProperty));
                Assert.That(fromDb.DateTimeProperty, Is.EqualTo(item.DateTimeProperty));
                Assert.That(fromDb.DoubleProperty, Is.EqualTo(item.DoubleProperty));
                Assert.That(fromDb.IntegerProperty, Is.EqualTo(item.IntegerProperty));
                Assert.That(fromDb.LinkProperty, Is.EqualTo(item.LinkProperty));
                Assert.That(fromDb.ShortStringProperty, Is.EqualTo(item.ShortStringProperty));
                Assert.That(fromDb.LongStringProperty, Is.EqualTo(item.LongStringProperty));
                Assert.That(fromDb.PersistableEditableProperty, Is.EqualTo(item.PersistableEditableProperty));
                Assert.That(fromDb.Details.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void QueryFor_MultipleTypes_WithSame_PersistableProperties()
        {
            var item1 = CreateOneItem<PropertyItemInheritor1>(0, "item1", null);
            persister.Save(item1);
            var items = persister.Repository.Find().ToList();
            
            Assert.That(items.Count, Is.EqualTo(1));
        }

        void SaveLoadAndCompare<T>(Func<PropertyItemType, T> get, Action<PropertyItemType> set)
        {
            PropertyItemType original = new PropertyItemType();
            set(original);
            
            using (persister)
            {
                persister.Save(original);
            }

            using (persister)
            {
                var stored = persister.Get<PropertyItemType>(original.ID);
                Assert.That(get(stored), Is.EqualTo(get(original)));
            }

        }
    }
}
