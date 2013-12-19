using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using N2.Details;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using Shouldly;

namespace N2.Tests.Persistence.NH
{
    [TestFixture, Category("Integration")]
    public class ItemRepositoryTests_Finds : DatabasePreparingBase
    {
        ContentItemRepository repository;
        new ISessionProvider sessionProvider;
        private PersistableItem root;
        private PersistableItem child1;
        private PersistableItem grandchild1;
        private PersistableItem child2;
        private PersistablePart part1;
        private PersistablePart part2;
        private PersistableItem version;
        private ContentItem[] all;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            CreateDatabaseSchema();

            sessionProvider = engine.Resolve<ISessionProvider>();
            repository = new ContentItemRepository(sessionProvider);

            all = new ContentItem []
            {
                root = CreateOneItem<Definitions.PersistableItem>(0, "page", null),
                child1 = CreateOneItem<Definitions.PersistableItem>(0, "page1", root),
                grandchild1 = CreateOneItem<Definitions.PersistableItem>(0, "page1_1", child1),
                part1 = CreateOneItem<Definitions.PersistablePart>(0, "part1", child1),
                part2 = CreateOneItem<Definitions.PersistablePart>(0, "part1", child1),
                child2 = CreateOneItem<Definitions.PersistableItem>(0, "page2", root),
                version = CreateOneItem<Definitions.PersistableItem>(0, "page1", null)
            };
            part1.ZoneName = "Left";
            part2.ZoneName = "RecursiveLeft";
            child1["Hello"] = "World";
            child2["Age"] = 2.7;
            grandchild1["Age"] = 1.7;

            version.VersionOf = child1;
            foreach (var item in all)
                repository.SaveOrUpdate(all);
        }

        [Test]
        public void Find_ByID_Equals()
        {
            repository.Find(Parameter.Equal("ID", grandchild1.ID)).Single().ShouldBe(grandchild1);
        }

        [Test]
        public void Find_ByID_NotEquals()
        {
            repository.Find(Parameter.NotEqual("ID", grandchild1.ID)).Count().ShouldBe(all.Length - 1);
        }

        [Test]
        public void Find_ByID_GreaterThan()
        {
            repository.Find(Parameter.GreaterThan("ID", grandchild1.ID)).SequenceEqual(all.Where(i => i.ID > grandchild1.ID));
        }

        [Test]
        public void Find_ByID_GreaterThanOrEqualTo()
        {
            repository.Find(Parameter.GreaterOrEqual("ID", grandchild1.ID)).SequenceEqual(all.Where(i => i.ID >= grandchild1.ID));
        }

        [Test]
        public void Find_ByID_LessThan()
        {
            repository.Find(Parameter.LessThan("ID", grandchild1.ID)).SequenceEqual(all.Where(i => i.ID < grandchild1.ID));
        }

        [Test]
        public void Find_ByID_LessThanOrEqualTo()
        {
            repository.Find(Parameter.LessOrEqual("ID", grandchild1.ID)).SequenceEqual(all.Where(i => i.ID <= grandchild1.ID));
        }

        [Test]
        public void Find_ByID_NotEqualTo()
        {
            repository.Find(Parameter.NotEqual("ID", grandchild1.ID)).SequenceEqual(all.Where(i => i.ID != grandchild1.ID));
        }

        [Test]
        public void Find_Zone_IsNull()
        {
            repository.Find(Parameter.IsNull("ZoneName")).SequenceEqual(all.Where(i => i.ZoneName == null));
        }

        [Test]
        public void Find_Zone_IsNotNull()
        {
            repository.Find(Parameter.IsNull("ZoneName")).SequenceEqual(all.Where(i => i.ZoneName != null));
        }

        [Test]
        public void Find_Zone_IsLike()
        {
            repository.Find(Parameter.StartsWith("ZoneName", "Recursive")).Single().ShouldBe(part2);
        }

        [Test]
        public void Find_Zone_IsNotLike()
        {
            repository.Find(Parameter.NotLike("ZoneName", "Recursive%")).Single().ShouldBe(part1);
        }

        [Test]
        public void Find_Or()
        {
            repository.Find(Parameter.Equal("ID", grandchild1.ID) | Parameter.Equal("ID", child1.ID)).Count().ShouldBe(2);
        }

        [Test]
        public void Find_And()
        {
            repository.Find(Parameter.Equal("ID", grandchild1.ID) & Parameter.Equal("Name", grandchild1.Name)).Single().ShouldBe(grandchild1);
        }

        [Test]
        public void Find_AndOr()
        {
            var results = repository.Find(
                (Parameter.Equal("Parent", root) & Parameter.StartsWith("Name", child1.Name))
                | (Parameter.Equal("Parent", root) & Parameter.StartsWith("Name", child2.Name))
                );

            results.Count().ShouldBe(2);
            results.ShouldNotContain(grandchild1);
        }

        [Test]
        public void Find_Skip()
        {
            var results = repository.Find(new ParameterCollection().Skip(2));

            results.Count().ShouldBe(all.Length - 2);
        }

        [Test]
        public void Find_Take()
        {
            var results = repository.Find(new ParameterCollection().Take(3));

            results.Count().ShouldBe(3);
        }

        [Test]
        public void Find_OrderBy_DESC()
        {
            var results = repository.Find(new ParameterCollection().OrderBy("ID"));

            results.First().ID.ShouldBe(all.Select(i => i.ID).Min());
            results.Last().ID.ShouldBe(all.Select(i => i.ID).Max());
        }

        [Test]
        public void Find_OrderBy()
        {
            var results = repository.Find(new ParameterCollection().OrderBy("ID Desc"));

            results.First().ID.ShouldBe(all.Select(i => i.ID).Max());
            results.Last().ID.ShouldBe(all.Select(i => i.ID).Min());
        }

        [Test]
        public void Find_ExcludeVersions()
        {
            var results = repository.Find(Parameter.IsNull("VersionOf.ID"));

            results.Count().ShouldBe(all.Length - 1);
        }

        [Test]
        public void Find_DetailEqual()
        {
            var results = repository.Find(Parameter.Equal("Hello", "World").Detail());

            results.Single().ShouldBe(child1);
        }

        [Test]
        public void Find_DetailLike()
        {
            var results = repository.Find(Parameter.Like("Hello", "Wor%").Detail());

            results.Single().ShouldBe(child1);
        }

        [Test]
        public void Find_UnknownDetailLike()
        {
            var results = repository.Find(Parameter.Like(null, "Wor%").Detail());

            results.Single().ShouldBe(child1);
        }

        [Test]
        public void Find_DetailLessThan()
        {
            var results = repository.Find(Parameter.LessThan("Age", 2.0).Detail());

            results.Single().ShouldBe(grandchild1);
        }

        [Test]
        public void Find_In()
        {
            var results = repository.Find(Parameter.In("ID", child1.ID, child2.ID)).ToList();

            results.Count.ShouldBe(2);
            results.ShouldContain(child1);
            results.ShouldContain(child2);
        }

        [Test]
        public void Find_NotIn()
        {
            var results = repository.Find(Parameter.NotIn("ID", child1.ID, child2.ID)).ToList();

            results.Count.ShouldBe(all.Length - 2);
            results.ShouldNotContain(child1);
            results.ShouldNotContain(child2);
        }

        [Test]
        public void Find_DetailIn()
        {
            var results = repository.Find(Parameter.In("Hello", "World", "Universe").Detail()).ToList();

            results.Count.ShouldBe(1);
            results.ShouldContain(child1);
        }

        [Test]
        public void Find_DetailNotIn()
        {
            var results = repository.Find(Parameter.NotIn("Age", 1.7, 1.9, 2.1).Detail()).ToList();

            results.Single().ShouldBe(child2); // other items are excluded since they don't have a detail named "Hello", might be unexpected though
        }

        [Test]
        public void Select_SingleProperty()
        {
            var results = repository.Select(Parameter.LessThan("Age", 2.0).Detail(), "ID");

            results.Single()["ID"].ShouldBe(grandchild1.ID);
        }

        [Test]
        public void Select_MultipleProperties()
        {
            var result = repository.Select(Parameter.LessThan("Age", 2.0).Detail(), "ID", "Title").Single();

            result["ID"].ShouldBe(grandchild1.ID);
            result["Title"].ShouldBe(grandchild1.Title);
        }

        [Test]
        public void Select_SingleProperty_MultipleResults()
        {
            var results = repository.Select(Parameter.Equal("ID", grandchild1.ID) | Parameter.Equal("ID", child1.ID), "ID");

            results.Count().ShouldBe(2);
            results.Any(r => (int)r["ID"] == grandchild1.ID).ShouldBe(true);
            results.Any(r => (int)r["ID"] == child1.ID).ShouldBe(true);
        }

        [Test]
        public void Select_MultipleProperties_MultipleResults()
        {
            var results = repository.Select(Parameter.Equal("ID", grandchild1.ID) | Parameter.Equal("ID", child1.ID), "ID", "Title");

            results.Count().ShouldBe(2);
            var gc = results.Single(r => (int)r["ID"] == grandchild1.ID);
            gc["ID"].ShouldBe(grandchild1.ID);
            gc["Title"].ShouldBe(grandchild1.Title);

            var c = results.Single(r => (int)r["ID"] == child1.ID);
            c["ID"].ShouldBe(child1.ID);
            c["Title"].ShouldBe(child1.Title);
        }

        [Test]
        public void Count_SingleResult()
        {
            repository.Count(Parameter.LessThan("Age", 2.0).Detail()).ShouldBe(1);
        }

        [Test]
        public void Count_MultipleResults()
        {
            repository.Count(Parameter.Equal("ID", grandchild1.ID) | Parameter.Equal("ID", child1.ID)).ShouldBe(2);
        }
    }
}
