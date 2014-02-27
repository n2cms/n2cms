using N2.Tests.Persistence;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Tests.Details
{
    [TestFixture]
    public class PersistentDetailCollecetionTests : DatabasePreparingBase
    {
        [Test]
        public void Add()
        {
            var root = CreateRoot("root", "root");
            using(engine.Persister)
            {
                root.DetailCollections["Test"].Add("hello");
                engine.Persister.Save(root);
            }

            engine.Persister.Get(root.ID).DetailCollections["Test"].Single().ShouldBe("hello");
        }

        [Test]
        public void Update()
        {
            var root = CreateRoot("root", "root");
            using (engine.Persister)
            {
                root.DetailCollections["Test"].Add("hello");
                engine.Persister.Save(root);
            }

            using (engine.Persister)
            {
                var storedRoot = engine.Persister.Get(root.ID);
                storedRoot.DetailCollections["Test"][0] = "Hej";
                engine.Persister.Save(storedRoot);
            }

            engine.Persister.Get(root.ID).DetailCollections["Test"].Single().ShouldBe("Hej");
        }

        [Test]
        public void Remove()
        {
            var root = CreateRoot("root", "root");
            using (engine.Persister)
            {
                root.DetailCollections["Test"].Add("hello");
                engine.Persister.Save(root);
            }

            using (engine.Persister)
            {
                var storedRoot = engine.Persister.Get(root.ID);
                storedRoot.DetailCollections["Test"].Remove("hello");
                engine.Persister.Save(storedRoot);
            }

            engine.Persister.Get(root.ID).DetailCollections["Test"].Count.ShouldBe(0);
        }

        [Test]
        public void AddDictionary()
        {
            var root = CreateRoot("root", "root");
            using (engine.Persister)
            {
                root.DetailCollections["Test"].Replace(new Dictionary<string, object> { { "hello", "world" }, { "snick", "snack" } });
                engine.Persister.Save(root);
            }

            var storedRoot = engine.Persister.Get(root.ID);
            storedRoot.DetailCollections["Test"].Details.Single(d => d.Meta == "hello").Value.ShouldBe("world");
            storedRoot.DetailCollections["Test"].Details.Single(d => d.Meta == "snick").Value.ShouldBe("snack");
        }

        [Test]
        public void UpdateDictionary()
        {
            var root = CreateRoot("root", "root");
            using (engine.Persister)
            {
                root.DetailCollections["Test"].Replace(new Dictionary<string, object> { { "hello", "world" }, { "snick", "snack" } });
                engine.Persister.Save(root);
            }
            using (engine.Persister)
            {
                var storedRoot = engine.Persister.Get(root.ID);
                storedRoot.DetailCollections["Test"].Replace(new Dictionary<string, object> { { "hello", "universe" }, { "wow", "factor" } });
                engine.Persister.Save(storedRoot);
            }

            {
                var storedRoot = engine.Persister.Get(root.ID);
                storedRoot.DetailCollections["Test"].Details.Single(d => d.Meta == "hello").Value.ShouldBe("universe");
                storedRoot.DetailCollections["Test"].Details.Single(d => d.Meta == "wow").Value.ShouldBe("factor");
            }
        }

        [Test]
        public void RemoveDictionary()
        {
            var root = CreateRoot("root", "root");
            using (engine.Persister)
            {
                root.DetailCollections["Test"].Replace(new Dictionary<string, object> { { "hello", "world" }, { "snick", "snack" } });
                engine.Persister.Save(root);
            }
            using (engine.Persister)
            {
                var storedRoot = engine.Persister.Get(root.ID);
                storedRoot.DetailCollections["Test"].Replace(new Dictionary<string, object> { { "hello", "universe" } });
                engine.Persister.Save(storedRoot);
            }

            {
                var storedRoot = engine.Persister.Get(root.ID);
                var d = storedRoot.DetailCollections["Test"].Details.Single();
                d.Meta.ShouldBe("hello");
                d.Value.ShouldBe("universe");
            }
        }
    }
}
