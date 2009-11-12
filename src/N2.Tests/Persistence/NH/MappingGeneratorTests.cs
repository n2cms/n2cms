using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Persistence;

namespace N2.Tests.Persistence.NH
{
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
    }

    [NUnit.Framework.TestFixture]
    public class MappingGeneratorTests : PersisterTestsBase
    {
        [NUnit.Framework.TestFixtureSetUp]
        public override void TestFixtureSetup()
        {
            SetUpEngineWithTypes(typeof(PropertyItemType));
        }

        // string

        [NUnit.Framework.Test]
        public void StringProperty()
        {
            SaveLoadAndCompare<string>(
                (i) => i.LongStringProperty, 
                (i) => { i.LongStringProperty = "Will it blend? ABCDEFGHIJKLMNOPQRSTUVWXYZ ABCDEFGHIJKLMNOPQRSTUVWXYZ ABCDEFGHIJKLMNOPQRSTUVWXYZ ABCDEFGHIJKLMNOPQRSTUVWXYZ ABCDEFGHIJKLMNOPQRSTUVWXYZ"; });
        }

        [NUnit.Framework.Test]
        public void StringProperty_WithMaxLength()
        {
            SaveLoadAndCompare<string>(
                (i) => i.ShortStringProperty, 
                (i) => { i.ShortStringProperty = "Will it blend?"; });
        }

        // integer

        [NUnit.Framework.Test]
        public void IntegerProperty()
        {
            SaveLoadAndCompare<int>(
                (i) => i.IntegerProperty,
                (i) => { i.IntegerProperty = 432; });
        }

        [NUnit.Framework.Test]
        public void IntegerProperty_SetNullable()
        {
            SaveLoadAndCompare<int?>(
                (i) => i.NullableIntegerProperty,
                (i) => { i.NullableIntegerProperty = 432; });
        }

        [NUnit.Framework.Test]
        public void IntegerProperty_StoreNull()
        {
            SaveLoadAndCompare<int?>(
                (i) => i.NullableIntegerProperty,
                (i) => { i.NullableIntegerProperty = null; });
        }

        // boolean

        [NUnit.Framework.Test]
        public void BooleanProperty()
        {
            SaveLoadAndCompare<bool>(
                (i) => i.BooleanProperty,
                (i) => { i.BooleanProperty = true; });
        }

        [NUnit.Framework.Test]
        public void BooleanProperty_SetNullable()
        {
            SaveLoadAndCompare<bool?>(
                (i) => i.NullableBooleanProperty,
                (i) => { i.NullableBooleanProperty = true; });
        }

        [NUnit.Framework.Test]
        public void BooleanProperty_StoreNull()
        {
            SaveLoadAndCompare<bool?>(
                (i) => i.NullableBooleanProperty,
                (i) => { i.NullableBooleanProperty = null; });
        }

        // date time

        [NUnit.Framework.Test]
        public void DateTimeProperty()
        {
            SaveLoadAndCompare<DateTime>(
                (i) => i.DateTimeProperty,
                (i) => { i.DateTimeProperty = DateTime.Now.StripMilliseconds(); });
        }

        [NUnit.Framework.Test]
        public void DateTimeProperty_SetNullable()
        {
            SaveLoadAndCompare<DateTime?>(
                (i) => i.NullableDateTimeProperty,
                (i) => { i.NullableDateTimeProperty = DateTime.Now.StripMilliseconds(); });
        }

        [NUnit.Framework.Test]
        public void DateTimeProperty_StoreNull()
        {
            SaveLoadAndCompare<DateTime?>(
                (i) => i.NullableDateTimeProperty,
                (i) => { i.NullableDateTimeProperty = DateTime.Now.StripMilliseconds(); });
        }

        // double

        [NUnit.Framework.Test]
        public void DoubleProperty()
        {
            SaveLoadAndCompare<double>(
                (i) => i.DoubleProperty,
                (i) => { i.DoubleProperty = 432.1; });
        }

        [NUnit.Framework.Test]
        public void DoubleProperty_SetNullable()
        {
            SaveLoadAndCompare<double?>(
                (i) => i.NullableDoubleProperty,
                (i) => { i.NullableDoubleProperty = 432.1; });
        }

        [NUnit.Framework.Test]
        public void DoubleProperty_StoreNull()
        {
            SaveLoadAndCompare<double?>(
                (i) => i.NullableDoubleProperty,
                (i) => { i.NullableDoubleProperty = null; });
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
