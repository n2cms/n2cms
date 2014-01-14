using System;
using System.Collections.Generic;
using System.Linq;
using N2.Details;
using NUnit.Framework;
using Shouldly;

namespace N2.Tests.Details
{
    [TestFixture]
    public class TemporaryDetailCollectionTests_PersistentContentList_Initialized : TemporaryDetailCollectionTests
    {

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            foreach (var d in item.DetailCollections)
            {
                // do nothing
            }
        }
    }

    [TestFixture]
    public class TemporaryDetailCollectionTests_PersistentContentList : TemporaryDetailCollectionTests
    {
        private PersistenceTestHelper helper;

        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
            helper = new PersistenceTestHelper();
            helper.TestFixtureSetUp();
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            helper.CreateDatabaseSchema();

            using (helper.Engine.Persister)
            {
                helper.Engine.Persister.Save(item);
            }
            item = helper.Engine.Persister.Get<Content.AnItem>(item.ID);
        }

        [TearDown]
        public virtual void TearDown()
        {
            helper.TearDown();
        }

        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown()
        {
            helper.TestFixtureTearDown();
        }
    }

    [TestFixture]
    public class TemporaryDetailCollectionTests_ContentList : TemporaryDetailCollectionTests
    {
    }

    public abstract class TemporaryDetailCollectionTests
    {
        protected Content.AnItem item;

        [SetUp]
        public virtual void SetUp()
        {
            item = new Content.AnItem();
        }

        [Test]
        public void CanBeAccssed()
        {
            var collection = item.DetailCollections["Temp"];
            collection.ShouldNotBe(null);
        }

        [Test]
        public void IsNotAdded_ToItemsCollection()
        {
            var collection = item.DetailCollections["Temp"];

            item.DetailCollections.Count.ShouldBe(0);
        }

        [Test]
        public void IsAddedToItem_WhenAddingValues()
        {
            var collection = item.DetailCollections["Temp"];

            collection.Add(1);
            item.DetailCollections.Count.ShouldBe(1);
            item.DetailCollections["Temp"].Count.ShouldBe(1);
        }

        [Test]
        public void IsAddedToItem_WhenReplacingValues()
        {
            var collection = item.DetailCollections["Temp"];

            collection.Replace(new[] { 1, 1 });
            item.DetailCollections.Count.ShouldBe(1);
            item.DetailCollections["Temp"].Count.ShouldBe(2);
        }

        [Test]
        public void IsAddedToItem_WhenAddingRangesOfValues()
        {
            var collection = item.DetailCollections["Temp"];

            collection.AddRange(new[] { 2, 3 });
            item.DetailCollections.Count.ShouldBe(1);
            item.DetailCollections["Temp"].Count.ShouldBe(2);
        }

        [Test]
        public void IsAddedToItem_WhenInsertingValues()
        {
            var collection = item.DetailCollections["Temp"];

            collection.Insert(0, 1);
            item.DetailCollections.Count.ShouldBe(1);
            item.DetailCollections["Temp"].Count.ShouldBe(1);
        }

        [Test]
        public void IsAddedToItem_WhenReplacingDictionary()
        {
            var collection = item.DetailCollections["Temp"];

            collection.Replace(new Dictionary<string, object> { { "One", 1 }, { "Two", 2 }, { "Three", 3 } });
            item.DetailCollections.Count.ShouldBe(1);
            item.DetailCollections["Temp"].Count.ShouldBe(3);
        }
    }
}
