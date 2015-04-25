using N2.Definitions;
using N2.Management.Content.Navigation;
using N2.Persistence.Sources;
using N2.Tests;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Management.Tests.Sources
{
    [TestFixture]
    public class DirectDb_GroupedChildrenSourceTest : GroupedChildrenSourceTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            GetAttribute().AllowDirectQuery = true;
        }
    }

    [TestFixture]
    public class DirectDb_UnInitializedChildren_GroupedChildrenSourceTest : GroupedChildrenSourceTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            GetAttribute().AllowDirectQuery = true;

            engine.Persister.Dispose();

            root = engine.Persister.Get<GroupedPage>(root.ID);
            item1 = engine.Persister.Get<GroupedPage>(item1.ID);
            item2 = engine.Persister.Get<GroupedPage>(item2.ID);
            rootQuery = Query.From(root);
        }
    }

    [TestFixture]
    public class SourceAnalyzing_GroupedChildrenSourceTest : GroupedChildrenSourceTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            GetAttribute().AllowDirectQuery = false;
        }
    }

    public abstract class GroupedChildrenSourceTestBase : PersistenceAwareBase
    {
        [GroupChildren(GroupChildrenMode.Ungrouped)]
        public class GroupedPage : ContentItem
        {
        }

        private ContentSource source;
        private N2.Definitions.Static.DefinitionMap map;
        protected GroupedPage root;
        protected GroupedPage item1;
        protected GroupedPage item2;
        protected Query rootQuery;

        protected override T CreateItem<T>(string name, ContentItem parent = null, string zoneName = null)
        {
            var item = base.CreateItem<T>(name, parent, zoneName);
            engine.Persister.Save(item);
            return item;
        }
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            CreateDatabaseSchema();

            map = new N2.Definitions.Static.DefinitionMap();
            source = new ContentSource(new N2.Tests.Fakes.FakeSecurityManager(), new SourceBase[] { new DatabaseSource(null, null), new ActiveContentSource(), new ChildGroupSource(map) });

            root = CreateItem<GroupedPage>("root", null);
            item1 = CreateItem<GroupedPage>("item1", root);
            item2 = CreateItem<GroupedPage>("item2", root);

            rootQuery = new Query { Parent = root };
        }

        [Test]
        public void GroupByPages_VirtualGroup_IsCreated()
        {
            GetAttribute(GroupChildrenMode.Pages);

            var children = source.GetChildren(rootQuery).ToList();

            children.Single().ShouldBeOfType<ChildGroupContainer>();
            children.Single().Name.ShouldBe("virtual-grouping/0");
        }

        [Test]
        public void GroupByPages_VirtualGroup_ReturnsOriginalPages()
        {
            GetAttribute(GroupChildrenMode.Pages);

            var group = source.GetChildren(rootQuery).Single();

            var children = source.GetChildren(new Query { Parent = group }).ToList();
            children.Count.ShouldBe(2);
            children.ShouldContain(item1);
            children.ShouldContain(item2);
        }

        [Test]
        public void GroupByPages_VirtualGroup_MultiplePages()
        {
            var item3 = CreateItem<GroupedPage>("item3", root);
            var item4 = CreateItem<GroupedPage>("item4", root);
            var item5 = CreateItem<GroupedPage>("item5", root);

            var attribute = GetAttribute(GroupChildrenMode.Pages);
            attribute.PageSize = 2;

            var groups = source.GetChildren(rootQuery).ToList();

            groups.Count.ShouldBe(3);
            groups[0].Name.ShouldBe("virtual-grouping/0");
            groups[1].Name.ShouldBe("virtual-grouping/1");
            groups[2].Name.ShouldBe("virtual-grouping/2");

            source.GetChildren(new Query { Parent = groups[2] }).Single().ShouldBe(item5);
        }

        [Test]
        public void GroupByPages_VirtualGroup_FilledUpPage()
        {
            var attribute = GetAttribute(GroupChildrenMode.Pages);
            attribute.PageSize = 2;

            var groups = source.GetChildren(rootQuery).ToList();

            groups.Count.ShouldBe(1);
        }

        [Test]
        public void GroupByPagesAfterTreshold_VirtualGroup_ReturnsOriginalPages()
        {
            GetAttribute(GroupChildrenMode.PagesAfterTreshold);

            var children = source.GetChildren(rootQuery).ToList();

            children.Count.ShouldBe(2);
            children.ShouldContain(item1);
            children.ShouldContain(item2);
        }

        [Test]
        public void GroupByPagesAfterTreshold_VirtualGroup_ReturnsPagesUntilTreshold_AndGroupForRemaining()
        {
            GetAttribute(GroupChildrenMode.PagesAfterTreshold).StartPagingTreshold = 1;

            var children = source.GetChildren(rootQuery).ToList();

            children.Count.ShouldBe(2);
            children.ShouldContain(item1);
            source.GetChildren(Query.From(children[1])).ShouldContain(item2);
        }

        [Test]
        public void GroupByAlphabeticalIndex_VirtualGroup_IsCreated()
        {
            GetAttribute(GroupChildrenMode.AlphabeticalIndex);
            var xyz = CreateItem<GroupedPage>("xyz", root);

            var children = source.GetChildren(rootQuery).ToList();

            var i = children.Single(c => c.Title == "I");
            var x = children.Single(c => c.Title == "X");

            source.GetChildren(Query.From(i)).Count().ShouldBe(2);
            source.GetChildren(Query.From(x)).Count().ShouldBe(1);
        }

        [Test]
        public void GroupByYear_VirtualGroup_IsCreated()
        {
            GetAttribute(GroupChildrenMode.PublishedYear);

            var children = source.GetChildren(rootQuery).ToList();

            var year = children.Single(c => c.Title == DateTime.Today.Year.ToString());

            source.GetChildren(Query.From(year)).Count().ShouldBe(2);
        }

        [Test]
        public void GroupByYearMonth_VirtualGroup_IsCreated()
        {
            GetAttribute(GroupChildrenMode.PublishedYearMonth);

            var children = source.GetChildren(rootQuery).ToList();

            var month = children.Single(c => c.Title == DateTime.Today.ToString("yyyy-MM"));

            source.GetChildren(Query.From(month)).Count().ShouldBe(2);
        }

        [Test]
        public void GroupByYearMonthDay_VirtualGroup_IsCreated()
        {
            GetAttribute(GroupChildrenMode.PublishedYearMonthDay);

            var children = source.GetChildren(rootQuery).ToList();

            var month = children.Single(c => c.Title == DateTime.Today.ToShortDateString());

            source.GetChildren(Query.From(month)).Count().ShouldBe(2);
        }

        [Test]
        public void RecentWithArchive_VirtualGroup_IsCreated()
        {
            item1.Published = DateTime.Today.AddYears(-2);
            engine.Persister.Save(item1);
            GetAttribute(GroupChildrenMode.RecentWithArchive);

            var children = source.GetChildren(rootQuery).ToList();

            children.Count.ShouldBe(2);
            children.ShouldContain(item2);

            var archive = children.Single(c => c.Title == "Archive");
            source.GetChildren(Query.From(archive)).Single().ShouldBe(item1);
        }

        [Test]
        public void Type_VirtualGroup_IsCreated()
        {
            GetAttribute(GroupChildrenMode.Type);

            var children = source.GetChildren(rootQuery).ToList();

            var group = children.Single();
            group.Title.ShouldBe(typeof(GroupedPage).Name);

            source.GetChildren(Query.From(group)).Count().ShouldBe(2);
        }

        [Test]
        public void ZoneName_VirtualGroup_IsCreated()
        {
            item1.ZoneName = "Hello";
            engine.Persister.Save(item1);

            GetAttribute(GroupChildrenMode.ZoneName);

            var children = source.GetChildren(rootQuery).ToList();

            children.ShouldContain(item2);
            var group = children.Single(c => c.Title == "Hello");

            source.GetChildren(Query.From(group)).Single().ShouldBe(item1);
        }

        protected GroupChildrenAttribute GetAttribute(GroupChildrenMode? mode = null)
        {
            var attribute = map.GetOrCreateDefinition(root).GetCustomAttributes<GroupChildrenAttribute>().Single();
            if (mode.HasValue)
                attribute.GroupBy = mode.Value;
            return attribute;
        }
    }
}
