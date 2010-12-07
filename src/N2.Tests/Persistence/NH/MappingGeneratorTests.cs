using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Persistence;
using N2.Persistence.Proxying;

namespace N2.Tests.Persistence.NH
{
	#region class PropertyItemType
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
    }
	#endregion

	[TestFixture]
    public class MappingGeneratorTests : PersisterTestsBase
    {
        [TestFixtureSetUp]
        public override void TestFixtureSetup()
        {
			InterceptingProxyFactory proxyFactory;
			TestSupport.Setup(out definitions, out notifier, out sessionProvider, out finder, out schemaCreator, out proxyFactory, typeof(PropertyItemType));
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
                (i) => { i.DateTimeProperty = DateTime.Now.StripMilliseconds(); });
        }

        [Test]
        public void DateTimeProperty_SetNullable()
        {
            SaveLoadAndCompare<DateTime?>(
                (i) => i.NullableDateTimeProperty,
                (i) => { i.NullableDateTimeProperty = DateTime.Now.StripMilliseconds(); });
        }

        [Test]
        public void DateTimeProperty_StoreNull()
        {
            SaveLoadAndCompare<DateTime?>(
                (i) => i.NullableDateTimeProperty,
                (i) => { i.NullableDateTimeProperty = DateTime.Now.StripMilliseconds(); });
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
				var linked = CreateOneItem<PropertyItemType>(0, "item1", null);
				persister.Save(linked);

				var fromDb = persister.Get<PropertyItemType>(item.ID);
				fromDb.BooleanProperty = true;
				fromDb.DateTimeProperty = new DateTime(2010, 06, 18, 14, 30, 00);
				fromDb.DoubleProperty = 555.555;
				fromDb.IntegerProperty = 555;
				fromDb.LinkProperty = linked;
				fromDb.ShortStringProperty = "in table text";
				fromDb.LongStringProperty = "long table text";

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
			using (persister)
			{
				persister.Save(linked);
				persister.Save(item);
			}

			using (persister)
			{
				var fromDb = persister.Get<PropertyItemType>(item.ID);
				Assert.That(fromDb.BooleanProperty, Is.EqualTo(fromDb.BooleanProperty));
				Assert.That(fromDb.DateTimeProperty, Is.EqualTo(fromDb.DateTimeProperty));
				Assert.That(fromDb.DoubleProperty, Is.EqualTo(fromDb.DoubleProperty));
				Assert.That(fromDb.IntegerProperty, Is.EqualTo(fromDb.IntegerProperty));
				Assert.That(fromDb.LinkProperty, Is.EqualTo(fromDb.LinkProperty));
				Assert.That(fromDb.ShortStringProperty, Is.EqualTo(fromDb.ShortStringProperty));
				Assert.That(fromDb.LongStringProperty, Is.EqualTo(fromDb.LongStringProperty));
				Assert.That(fromDb.Details.Count, Is.EqualTo(0));
			}
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
