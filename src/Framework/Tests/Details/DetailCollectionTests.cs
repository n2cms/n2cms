using System;
using System.Collections.Generic;
using System.Linq;
using N2.Details;
using NUnit.Framework;
using Shouldly;

namespace N2.Tests.Details
{
    [TestFixture]
    public class DetailCollectionTests
    {
        [Test]
        public void CanAdd_IntDetail()
        {
            DetailCollection collection = new DetailCollection();
            collection.Add(1);

            Assert.That(collection[0], Is.EqualTo(1));
            Assert.That(collection.Details[0].ValueType, Is.EqualTo(typeof(int)));
            Assert.That(collection.Details[0].ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.IntType));
            Assert.That(collection.Details[0].Value, Is.EqualTo(1));
        }

        [Test]
        public void CanAdd_StringDetail()
        {
            DetailCollection collection = new DetailCollection();
            collection.Add("hello");

            Assert.That(collection[0], Is.EqualTo("hello"));
            Assert.That(collection.Details[0].ValueType, Is.EqualTo(typeof(string)));
            Assert.That(collection.Details[0].ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.StringType));
            Assert.That(collection.Details[0].Value, Is.EqualTo("hello"));
        }

        [Test]
        public void CanAddRange()
        {
            DetailCollection collection = new DetailCollection();
            collection.AddRange(new[] {"hello", "world"});

            Assert.That(collection[0], Is.EqualTo("hello"));
            Assert.That(collection[1], Is.EqualTo("world"));
        }

        [Test]
        public void CanFind_IndexOfInteger()
        {
            DetailCollection collection = new DetailCollection();
            collection.Add(1);
            collection.Add(2);

            Assert.That(collection.IndexOf(1), Is.EqualTo(0));
            Assert.That(collection.IndexOf(2), Is.EqualTo(1));
        }

        [Test]
        public void CanInsert_IntoCollection()
        {
            DetailCollection collection = new DetailCollection();
            collection.Insert(0, "hello");
            collection.Insert(0, "world");

            Assert.That(collection.IndexOf("hello"), Is.EqualTo(1));
            Assert.That(collection.IndexOf("world"), Is.EqualTo(0));
        }

        [Test]
        public void CanRemove_FromCollection()
        {
            DetailCollection collection = new DetailCollection(null, null, "hello", "world");
            
            collection.Remove("hello");

            Assert.That(collection.Count, Is.EqualTo(1));
            Assert.That(collection[0], Is.EqualTo("world"));
        }

        [Test]
        public void CanRemove_AtIndex()
        {
            DetailCollection collection = new DetailCollection(null, null, "hello", "world");

            collection.RemoveAt(0);

            Assert.That(collection.Count, Is.EqualTo(1));
            Assert.That(collection[0], Is.EqualTo("world"));
        }

        [Test]
        public void CanFind_IndexOfString()
        {
            DetailCollection collection = new DetailCollection();
            collection.Add("hello");
            collection.Add("world");

            Assert.That(collection.IndexOf("hello"), Is.EqualTo(0));
            Assert.That(collection.IndexOf("world"), Is.EqualTo(1));
        }

        [Test]
        public void IndexOf_NotFoundItem_IsNegativeOne()
        {
            DetailCollection collection = new DetailCollection();
            collection.Add("hello");
            collection.Add("world");

            Assert.That(collection.IndexOf("sweden"), Is.EqualTo(-1));
        }

        [Test]
        public void DoesntThrow_WhenCollection_ContainsNullValue()
        {
            DetailCollection collection = new DetailCollection();
            collection.Details.Add(new ContentDetail());

            Assert.That(collection.IndexOf("something"), Is.EqualTo(-1));
        }

        [Test]
        public void Contains_IsTrue_ForContainedString()
        {
            DetailCollection collection = new DetailCollection();
            collection.Add("hello");

            Assert.That(collection.Contains("hello"));
        }

        [Test]
        public void CanClear_DetailCollection()
        {
            DetailCollection collection = new DetailCollection();
            collection.Add("hello");

            collection.Clear();

            Assert.That(collection.Count, Is.EqualTo(0));
        }

        [Test]
        public void CanClone_Collection()
        {
            DetailCollection collection = new DetailCollection();
            collection.Add("hello");

            DetailCollection cloned = collection.Clone();
            Assert.That(cloned.Contains("hello"));
        }

        [Test]
        public void ClonedCollection_IdentifierIsCleared()
        {
            DetailCollection collection = new DetailCollection();
            collection.Add("hello");

            DetailCollection cloned = collection.Clone();
            Assert.That(cloned.ID, Is.EqualTo(0));
        }

        [Test, Ignore]
        public void UpdatedEnclosingItem_UpdatesDetails()
        {
            DetailCollection collection = new DetailCollection();
            collection.Add("hello");

            var item = new Content.AnItem();
            collection.EnclosingItem = item;

            Assert.That(collection.EnclosingItem, Is.EqualTo(item));
            Assert.That(collection.Details[0].EnclosingItem, Is.EqualTo(item));
        }

        [Test]
        public void CanCopyToArray()
        {
            DetailCollection collection = new DetailCollection();
            collection.Add("hello");

            string[] array = new string[1];
            collection.CopyTo(array, 0);

            Assert.That(array[0], Is.EqualTo("hello"));
        }

        [Test]
        public void CanReplace_ValuesInCollection()
        {
            DetailCollection collection = new DetailCollection(null, null, "hello", "world");

            collection.Replace(new[] {"hi", "world"});

            Assert.That(collection.Contains("hi"));
            Assert.That(collection.Contains("world"));
            Assert.That(!collection.Contains("hello"));
        }

        [Test]
        public void CanReplace_ValuesInCollection_WithoutRemoving_ExistingDetails()
        {
            DetailCollection collection = new DetailCollection(null, null, "hello", "world");
            collection.Details[0].ID = 666;
            collection.Details[1].ID = 777;

            collection.Replace(new[] { "hi", "world" });

            Assert.That(collection.Details.Where(d => d.ID == 777).Single().Value, Is.EqualTo("world"));
        }

        [Test]
        public void Collection_CanCombine_MultipleTypes()
        {
            object[] objects = new object[] {"hello", 1, 3.1415, true, N2.Utility.CurrentTime(), new Content.AnItem(), new object[0]};
            DetailCollection collection = new DetailCollection();
            collection.AddRange(objects);

            foreach(object o in objects)
                Assert.That(collection.Contains(o));
        }

        [Test]
        public void UsesBusinessEquality_ToCompare_OnCollections()
        {
            DetailCollection collection = new DetailCollection();
            collection.ID = 123;

            DetailCollection collection2 = new DetailCollection();
            collection2.ID = 123;

            Assert.That(collection.Equals(collection2));
        }

        [Test]
        public void HashCode_RemainsTheSame_AfterUpdatingID()
        {
            DetailCollection collection = new DetailCollection();
            int originalHashCode = collection.GetHashCode();

            collection.ID = 123;

            Assert.That(collection.GetHashCode(), Is.EqualTo(originalHashCode));
        }

        [Test]
        public void CanEnumerate_ValuesInCollection()
        {
            DetailCollection collection = new DetailCollection();
            collection.AddRange(new[] {1, 2, 3});

            int i = 0;
            foreach (var o in collection)
            {
                Assert.That(o, Is.EqualTo(++i));
            }
        }

        [Test]
        public void CanConvert_ToArray()
        {
            DetailCollection collection = new DetailCollection();
            collection.AddRange(new[] { 1, 2, 3 });

            int[] array = collection.ToArray<int>();

            Assert.That(array.Length, Is.EqualTo(3));
            Assert.That(array[0], Is.EqualTo(1));
            Assert.That(array[2], Is.EqualTo(3));
        }

        [Test]
        public void CanConvert_ToList()
        {
            DetailCollection collection = new DetailCollection();
            collection.AddRange(new[] { 1, 2, 3 });

            IList<int> list = collection.ToList<int>();

            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list[0], Is.EqualTo(1));
            Assert.That(list[2], Is.EqualTo(3));
        }

        [Test]
        public void CanGet_StronglyTypeEnumerator()
        {
            DetailCollection collection = new DetailCollection();
            collection.AddRange(new object[] { 1, 2, 3 });

            foreach (int number in collection.Enumerate<int>())
            {
                Assert.That(number >= 1 && number <= 3);
            }
        }

        [Test]
        public void CanWorkAsDictionary()
        {
            DetailCollection collection = new DetailCollection();
            collection.Replace(new Dictionary<string, object> { { "One", 1 }, { "Two", 2 }, { "Three", 3 } });
            var dict = collection.AsDictionary();

            dict.Count.ShouldBe(3);
            dict["One"].ShouldBe(1);
            dict["Two"].ShouldBe(2);
            dict["Three"].ShouldBe(3);
        }

        [Test]
        public void CanAddToDictionary()
        {
            DetailCollection collection = new DetailCollection();
            collection.Replace(new Dictionary<string, object> { { "One", 1 }, { "Two", 2 } });
            collection.Replace(new Dictionary<string, object> { { "One", 1 }, { "Two", 2 }, { "Three", 3 } });
            var dict = collection.AsDictionary();

            dict.Count.ShouldBe(3);
            dict["One"].ShouldBe(1);
            dict["Two"].ShouldBe(2);
            dict["Three"].ShouldBe(3);
        }

        [Test]
        public void CanRemoveFromDictionary()
        {
            DetailCollection collection = new DetailCollection();
            collection.Replace(new Dictionary<string, object> { { "One", 1 }, { "Two", 2 }, { "Three", 3 } });
            collection.Replace(new Dictionary<string, object> { { "One", 1 }, { "Two", 2 } });
            var dict = collection.AsDictionary();

            dict.Count.ShouldBe(2);
            dict["One"].ShouldBe(1);
            dict["Two"].ShouldBe(2);
        }

        [Test]
        public void CanReplaceInDictionary()
        {
            DetailCollection collection = new DetailCollection();
            collection.Replace(new Dictionary<string, object> { { "One", 1 }, { "Two", 2 } });
            collection.Replace(new Dictionary<string, object> { { "One", 1 }, { "Three", 3 } });
            var dict = collection.AsDictionary();

            dict.Count.ShouldBe(2);
            dict["One"].ShouldBe(1);
            dict["Three"].ShouldBe(3);
        }

    }
}
