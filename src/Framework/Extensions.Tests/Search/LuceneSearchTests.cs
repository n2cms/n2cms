using System;
using System.Configuration;
using System.Linq;
using N2.Configuration;
using N2.Persistence.NH;
using N2.Tests.Fakes;
using NUnit.Framework;
using N2.Web;
using N2.Persistence.Search;
using N2.Persistence;
using N2.Tests.Persistence.Definitions;
using System.Diagnostics;
using System.IO;
using N2.Definitions;
using Shouldly;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using N2.Tests;
using N2.Engine;
using N2.Search.Remote.Server;
using N2.Search.Remote.Client;

namespace N2.Extensions.Tests.Search
{
    [TestFixture]
    public class LocalLuceneSearchTests : LuceneSearchTests
    {
        LuceneAccesor accessor;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            accessor = new LuceneAccesor(new ThreadContext(), new DatabaseSection());
            indexer = new ContentIndexer(new LuceneIndexer(accessor), new TextExtractor(new IndexableDefinitionExtractor(definitions)));
            searcher = new LuceneContentSearcher(accessor, persister);

            indexer.Clear();
        }

        [TearDown]
        public override void TearDown()
        {
            accessor.Dispose();
            base.TearDown();
        }

    }

    [TestFixture]
    public class RemoteLuceneSearchTests : LuceneSearchTests
    {
        private IndexerServer server;

        [TestFixtureSetUp]
        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            definitions = TestSupport.SetupDefinitions(typeof(PersistableItem), typeof(PersistableItem2), typeof(PersistablePart));
            server = new IndexerServer();
            server.Start();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            server.Stop();
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            root = CreateOneItem<PersistableItem>(1, "The Root Page", null);
            indexer = new ContentIndexer(new IndexerClient(new Configuration.DatabaseSection()), new TextExtractor(new IndexableDefinitionExtractor(definitions)));
            searcher = new SearcherClient(persister, new Configuration.DatabaseSection());
            
            indexer.Clear();
        }

    }


    public abstract class LuceneSearchTests : ItemPersistenceMockingBase
    {
        protected IContentIndexer indexer;
        protected IContentSearcher searcher;
        protected IDefinitionManager definitions;
        protected PersistableItem root;

        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
            definitions = TestSupport.SetupDefinitions(typeof(PersistableItem), typeof(PersistableItem2), typeof(PersistablePart));
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            root = CreateOneItem<PersistableItem>(1, "The Root Page", null);
        }

        [Test]
        public void Title()
        {
            var item = CreateOneItem<PersistableItem>(2, "Hello world", root);
            indexer.Update(item);

            var hits = searcher.Search(Query.For("hello"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Skip()
        {
            indexer.Update(CreateOneItem<PersistableItem>(3, "Hello country", root));
            indexer.Update(CreateOneItem<PersistableItem>(2, "Hello world", root));
            indexer.Update(CreateOneItem<PersistableItem>(4, "Hello universe", root));

            
            var hits1 = searcher.Search(Query.For("hello").Take(1));
            var hits2 = searcher.Search(Query.For("hello").Skip(1).Take(1));
            var hits3 = searcher.Search(Query.For("hello").Skip(2).Take(1));

            Assert.That(hits1.Single(), Is.Not.EqualTo(hits2.Single()));
            Assert.That(hits2.Single(), Is.Not.EqualTo(hits3.Single()));
            Assert.That(hits3.Single(), Is.Not.EqualTo(hits1.Single()));
        }

        [Test]
        public void Take()
        {
            indexer.Update(CreateOneItem<PersistableItem>(3, "Hello country", root));
            indexer.Update(CreateOneItem<PersistableItem>(2, "Hello world", root));
            indexer.Update(CreateOneItem<PersistableItem>(4, "Hello universe", root));

            
            var hits1 = searcher.Search(Query.For("hello").Take(1));

            Assert.That(hits1.Count, Is.EqualTo(1));
        }

        [Test]
        public void Count_IsNumberOfItemsInHits()
        {
            indexer.Update(CreateOneItem<PersistableItem>(3, "Hello country", root));
            indexer.Update(CreateOneItem<PersistableItem>(2, "Hello world", root));
            indexer.Update(CreateOneItem<PersistableItem>(4, "Hello universe", root));

            
            var hits = searcher.Search(Query.For("hello").Take(2));

            Assert.That(hits.Hits.Count(), Is.EqualTo(2));
            Assert.That(hits.Count, Is.EqualTo(2));
        }

        [Test]
        public void Total_IsNumberOfItemsInIndex()
        {
            indexer.Update(CreateOneItem<PersistableItem>(3, "Hello country", root));
            indexer.Update(CreateOneItem<PersistableItem>(2, "Hello world", root));
            indexer.Update(CreateOneItem<PersistableItem>(4, "Hello universe", root));

            
            var hits = searcher.Search(Query.For("hello").Take(2));

            Assert.That(hits.Total, Is.EqualTo(3));
        }

        [Test]
        public void IndexableProperty()
        {
            var item = CreateOneItem<PersistableItem>(2, "Hello world", root);
            item.StringProperty = "Hej Världen";
            indexer.Update(item);

            
            var hits = searcher.Search(Query.For("världen"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
        }

        [Test]
        public void EditableProperty()
        {
            var item = CreateOneItem<PersistableItem>(2, "Hello world", root);
            item.IntProperty = 444;
            indexer.Update(item);

            
            var hits = searcher.Search(Query.For("444"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
        }

        [Test]
        public void MultipleKeywords()
        {
            indexer.Update(root);
            var item = CreateOneItem<PersistableItem>(2, "Hello world", root);
            indexer.Update(item);

            
            var hits = searcher.Search(Query.For("hello root"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(2));
        }

        [Test]
        public void TitleBoost()
        {
            root.StringProperty = "Hello world";
            indexer.Update(root);

            var item = CreateOneItem<PersistableItem>(2, root.StringProperty, root);
            item.StringProperty = root.Title;
            indexer.Update(item);

            

            var hits1 = searcher.Search(Query.For("hello"));
            var hits2 = searcher.Search(Query.For("root"));

            Assert.That(hits1.Hits.Select(h => h.Content).First(), Is.EqualTo(item));
            Assert.That(hits2.Hits.Select(h => h.Content).First(), Is.EqualTo(root));
        }

        [Test]
        public void Below()
        {
            var item1 = CreateOneItem<PersistableItem>(2, "first item", root);
            indexer.Update(item1);
            var item1b = CreateOneItem<PersistableItem>(3, "first item B", item1);
            indexer.Update(item1b);
            var item2 = CreateOneItem<PersistableItem>(4, "second item", root);
            indexer.Update(item2);
            var item2b = CreateOneItem<PersistableItem>(5, "second item B", item2);
            indexer.Update(item2b);

            
            var hits = searcher.Search(Query.For("item").Below(item1));

            Assert.That(hits.Hits.Count(), Is.EqualTo(2));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(item1));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(item1b));
        }

        [Test]
        public void FindOnlyPages()
        {
            var page1 = CreateOneItem<PersistableItem>(2, "first page", root);
            indexer.Update(page1);
            var part1 = CreateOneItem<PersistablePart>(3, "first part", page1);
            indexer.Update(part1);

            
            var hits = searcher.Search(Query.For("first").Pages(true));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(page1));
        }

        [Test]
        public void FindOnlyParts()
        {
            var page1 = CreateOneItem<PersistableItem>(2, "first page", root);
            indexer.Update(page1);
            var part1 = CreateOneItem<PersistablePart>(3, "first part", page1);
            indexer.Update(part1);

            
            var hits = searcher.Search(Query.For("first").Pages(false));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(part1));
        }

        [Test]
        public void FindPage_ByText_OnPart_BelowPage()
        {
            var page1 = CreateOneItem<PersistableItem>(2, "first page", root);
            var part1 = CreateOneItem<PersistablePart>(3, "first part", page1);
            part1.ZoneName = "Zone";
            indexer.Update(page1);
            indexer.Update(part1);

            
            var hits = searcher.Search(Query.For("part").Pages(true));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(page1));
        }

        [Test]
        public void DeletedItems_DoesntShowUp_InSearch()
        {
            var item = CreateOneItem<PersistableItem>(2, "Hello world", root);
            indexer.Update(item);
            indexer.Delete(item.ID);

            
            var hits = searcher.Search(Query.For("hello"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ChildrenOf_DeletedItems_DoesntShowUp_InSearch()
        {
            var item = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var child = CreateOneItem<PersistableItem>(3, "Child world", item);
            indexer.Update(item);
            indexer.Update(child);

            indexer.Delete(item.ID);

            
            var hits = searcher.Search(Query.For("world"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(0));
        }

        [Test]
        public void PagesWithoutMatchingRoles_AreNotDisplayed_WhenSearching_WithRoles()
        {
            var item = CreateOneItem<PersistableItem>(2, "Hello world", root);
            item.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(item, "Members"));
            indexer.Update(item);

            
            var hits = searcher.Search(Query.For("hello").ReadableBy("Everyone"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(0));
        }

        [Test]
        public void AuthorizedPages_AreDisplayed_WhenSearching_WithoutSpecifyingRoles()
        {
            var item = CreateOneItem<PersistableItem>(2, "Hello world", root);
            item.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(item, "Members"));
            indexer.Update(item);

            
            var hits = searcher.Search(Query.For("hello"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
        }

        [Test]
        public void PagesWithMatchingRole_AreDisplayed_WhenSearching_WithRoles()
        {
            var item = CreateOneItem<PersistableItem>(2, "Hello world", root);
            item.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(item, "Members"));
            indexer.Update(item);

            
            var hits = searcher.Search(Query.For("hello").ReadableBy("Members"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
        }

        [Test]
        public void PagesWithMatching_RoleIntersection_AreDisplayed_WhenSearching_WithRoles()
        {
            var item = CreateOneItem<PersistableItem>(2, "Hello world", root);
            item.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(item, "Members"));
            item.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(item, "Writers"));
            indexer.Update(item);

            
            var hits = searcher.Search(Query.For("hello").ReadableBy("Writers", "Editors"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
        }

        [Test]
        public void UnSecuredPages_AreDisplayed_WhenSearching_WithRoles()
        {
            var item = CreateOneItem<PersistableItem>(2, "Hello world", root);
            indexer.Update(item);

            
            var hits = searcher.Search(Query.For("hello").ReadableBy("Members"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Optimize()
        {
            indexer.Update(CreateOneItem<PersistableItem>(2, "Hello stockholm", root));
            indexer.Update(CreateOneItem<PersistableItem>(3, "Hello world", root));
            indexer.Update(CreateOneItem<PersistableItem>(4, "Hello universe", root));

            indexer.Optimize();

            Assert.IsTrue(true);
        }

        [Test]
        public void PagesCanBeFoundByType()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var part = CreateOneItem<PersistablePart>(3, "Hello region", root);
            indexer.Update(page);
            indexer.Update(part);

            
            var hits = searcher.Search(Query.For("hello").OfType(typeof(PersistablePart)));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
        }

        [Test]
        public void PagesCanBeFound_ByContentType()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var part = CreateOneItem<PersistablePart>(3, "Hello region", root);
            indexer.Update(page);
            indexer.Update(part);

            
            var hits = searcher.Search(Query.For("hello").OfType(typeof(PersistablePart)));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
        }

        [Test]
        public void PagesCanBeFound_ByInterfaceType()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var part = CreateOneItem<PersistablePart>(3, "Hello region", root);
            indexer.Update(page);
            indexer.Update(part);

            
            var hits = searcher.Search(Query.For("hello").OfType(typeof(IPart)));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
        }

        [Test]
        public void PagesCanBeFound_ByMultipleTypes()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var part = CreateOneItem<PersistablePart>(3, "Hello region", root);
            indexer.Update(page);
            indexer.Update(part);

            
            var hits = searcher.Search(Query.For("hello").OfType(typeof(IPart), typeof(IPage)));

            Assert.That(hits.Hits.Count(), Is.EqualTo(2));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(page));
        }

        [Test]
        public void PagesCanBeFound_ByObjectType()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var part = CreateOneItem<PersistablePart>(3, "Hello region", root);
            indexer.Update(page);
            indexer.Update(part);

            
            var hits = searcher.Search(Query.For("hello").OfType(typeof(object)));

            Assert.That(hits.Hits.Count(), Is.EqualTo(2));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(page));
        }

        [Test]
        public void Except_Text()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var part = CreateOneItem<PersistablePart>(3, "Hello region", root);
            indexer.Update(page);
            indexer.Update(part);

            
            var hits = searcher.Search(Query.For("hello").Except("world"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
        }

        [Test]
        public void Except_Type()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var part = CreateOneItem<PersistablePart>(3, "Hello region", root);
            indexer.Update(page);
            indexer.Update(part);

            
            var hits = searcher.Search(Query.For("hello").Except(typeof(IPage)));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
        }

        [Test]
        public void Except_Operator()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var part = CreateOneItem<PersistablePart>(3, "Hello region", root);
            indexer.Update(page);
            indexer.Update(part);

            
            var hits = searcher.Search(Query.For("hello") - Query.For(typeof(IPage)));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
        }

        [Test]
        public void Except_Operator_Multiple()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var part = CreateOneItem<PersistablePart>(3, "Hello world", root);
            var part2 = CreateOneItem<PersistablePart>(4, "Hello region", root);
            indexer.Update(page);
            indexer.Update(part);
            indexer.Update(part2);

            
            var hits = searcher.Search(Query.For("hello") - Query.For(typeof(IPage)) - Query.For("region"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
            Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
        }

        [Test]
        public void And()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var page2 = CreateOneItem<PersistableItem>(3, "Hello region", root);
            indexer.Update(page);
            indexer.Update(page2);

            
            var hits = searcher.Search(Query.For("hello").And(Query.For("world")));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
            Assert.That(hits.Contains(page));
        }

        [Test]
        public void And_Operator()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var page2 = CreateOneItem<PersistableItem>(3, "Hello region", root);
            indexer.Update(page);
            indexer.Update(page2);

            
            var hits = searcher.Search(Query.For("hello") & Query.For("world"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
            Assert.That(hits.Contains(page));
        }

        [Test]
        public void And_Operator_Multiple()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world hunger", root);
            var page2 = CreateOneItem<PersistableItem>(3, "Hello world fulfillment", root);
            indexer.Update(page);
            indexer.Update(page2);

            
            var hits = searcher.Search(Query.For("hello") & Query.For("world") & Query.For("hunger"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(1));
            Assert.That(hits.Contains(page));
        }

        [Test]
        public void Or()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var page2 = CreateOneItem<PersistableItem>(3, "Hello region", root);
            indexer.Update(page);
            indexer.Update(page2);

            
            var hits = searcher.Search(Query.For("region").Or(Query.For("world")));

            Assert.That(hits.Hits.Count(), Is.EqualTo(2));
            Assert.That(hits.Contains(page));
            Assert.That(hits.Contains(page2));
        }

        [Test]
        public void Or_Operator()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var page2 = CreateOneItem<PersistableItem>(3, "Hello region", root);
            indexer.Update(page);
            indexer.Update(page2);

            
            var hits = searcher.Search(Query.For("region") | Query.For("world"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(2));
            Assert.That(hits.Contains(page));
            Assert.That(hits.Contains(page2));
        }

        [Test]
        public void Or_Operator_Multiple()
        {
            var page = CreateOneItem<PersistableItem>(2, "Hello world", root);
            var page2 = CreateOneItem<PersistableItem>(3, "Hello region", root);
            var page3 = CreateOneItem<PersistableItem>(4, "Hello universe", root);
            indexer.Update(page);
            indexer.Update(page2);
            indexer.Update(page3);

            
            var hits = searcher.Search(Query.For("region") | Query.For("world") | Query.For("universe"));

            Assert.That(hits.Hits.Count(), Is.EqualTo(3));
            Assert.That(hits.Contains(page));
            Assert.That(hits.Contains(page2));
            Assert.That(hits.Contains(page3));
        }

        [Test]
        public void Language()
        {
            var sv = CreateOneItem<PersistableItem2>(2, "Svenska", root);
            sv.LanguageCode = "sv";
            var en = CreateOneItem<PersistableItem2>(3, "Engelska", root);
            en.LanguageCode = "en";

            var svitem = CreateOneItem<PersistableItem>(4, "Hello världen", sv);
            indexer.Update(svitem);

            var enitem = CreateOneItem<PersistableItem>(5, "Hello world", en);
            indexer.Update(enitem);

            
            var result = searcher.Search(Query.For("hello").Language(sv));

            Assert.That(result.Hits.Count(), Is.EqualTo(1));
            Assert.That(result.Single(), Is.EqualTo(svitem));
        }

        [Test]
        public void Language_ByLanguageCode()
        {
            var sv = CreateOneItem<PersistableItem2>(2, "Svenska", root);
            sv.LanguageCode = "sv";
            var en = CreateOneItem<PersistableItem2>(3, "Engelska", root);
            en.LanguageCode = "en";

            var svitem = CreateOneItem<PersistableItem>(4, "Hello världen", sv);
            indexer.Update(svitem);

            var enitem = CreateOneItem<PersistableItem>(5, "Hello world", en);
            indexer.Update(enitem);

            
            var result = searcher.Search(Query.For("hello").Language(sv.LanguageCode));

            Assert.That(result.Hits.Count(), Is.EqualTo(1));
            Assert.That(result.Single(), Is.EqualTo(svitem));
        }

        [Test]
        public void Language_ByFullLanguageCode()
        {
            var sv = CreateOneItem<PersistableItem2>(2, "Svenska", root);
            sv.LanguageCode = "sv-SE";
            var en = CreateOneItem<PersistableItem2>(3, "Engelska", root);
            en.LanguageCode = "en-GB";

            var svitem = CreateOneItem<PersistableItem>(4, "Hello världen", sv);
            indexer.Update(svitem);

            var enitem = CreateOneItem<PersistableItem>(5, "Hello world", en);
            indexer.Update(enitem);


            var result = searcher.Search(Query.For("hello").Language(sv.LanguageCode));

            Assert.That(result.Hits.Count(), Is.EqualTo(1));
            Assert.That(result.Single(), Is.EqualTo(svitem));
        }

        [Test]
        public void Language_ByPartialLanguageCode()
        {
            var sv = CreateOneItem<PersistableItem2>(2, "Svenska", root);
            sv.LanguageCode = "sv-SE";
            var en = CreateOneItem<PersistableItem2>(3, "Engelska", root);
            en.LanguageCode = "en-GB";

            var svitem = CreateOneItem<PersistableItem>(4, "Hello världen", sv);
            indexer.Update(svitem);

            var enitem = CreateOneItem<PersistableItem>(5, "Hello world", en);
            indexer.Update(enitem);


            var result = searcher.Search(Query.For("hello").Language("sv"));

            Assert.That(result.Hits.Count(), Is.EqualTo(1));
            Assert.That(result.Single(), Is.EqualTo(svitem));
        }

        [Test]
        public void Language_IncludesLanguageRoot()
        {
            var sv = CreateOneItem<PersistableItem2>(2, "Svenska", root);
            sv.LanguageCode = "sv";
            indexer.Update(sv);

            var en = CreateOneItem<PersistableItem2>(3, "Engelska", root);
            en.LanguageCode = "en";
            indexer.Update(en);

            var svitem = CreateOneItem<PersistableItem>(4, "Hello världen", sv);
            indexer.Update(svitem);

            var enitem = CreateOneItem<PersistableItem>(5, "Hello world", en);
            indexer.Update(enitem);

            
            var result = searcher.Search(Query.For("").Language(sv));

            Assert.That(result.Hits.Count(), Is.EqualTo(2));
            Assert.That(result.Contains(sv));
        }

        [Test]
        public void NonDetail_IndexableProperty_IsIndexed()
        {
            root.NonDetailProperty = "Lorem dolor";
            indexer.Update(root);

            
            var result = searcher.Search(Query.For("dolor"));

            Assert.That(result.Hits.Single().Content, Is.EqualTo(root));
        }

        [Test]
        public void NonDetail_IndexableOnlyGetterProperty_IsIndexed()
        {
            indexer.Update(root);

            
            var result = searcher.Search(Query.For("ipsum"));

            Assert.That(result.Hits.Single().Content, Is.EqualTo(root));
        }

        [Test]
        public void NonIndexableClass_IsNotIndexed()
        {
            var child = CreateOneItem<PersistableItem1b>(2, "child", root);
            indexer.Update(child);

            
            var result = searcher.Search(Query.For("child"));

            Assert.That(result.Hits.Any(), Is.False);
        }

        [Test]
        public void QueryByExpression_ForDetail()
        {
            root.StringProperty = "This is a very special string";
            indexer.Update(root);

            
            var query = Query.For<PersistableItem>();

            query.Contains(pi => pi.StringProperty, "special");
            var result = searcher.Search(query);

            Assert.That(result.Hits.Single().Content, Is.EqualTo(root));
        }

        [Test]
        public void QueryByExpression_ForTitleProperty()
        {
            indexer.Update(root);
            
            
            var query = Query.For<PersistableItem>();

            query.Contains(pi => pi.Title, "root");
            var result = searcher.Search(query);

            Assert.That(result.Hits.Single().Content, Is.EqualTo(root));
        }

        [Test]
        public void QueryForBelowOfTypeReadableByExceptType()
        {
            indexer.Update(root);
            var part = CreateOneItem<PersistableItem2>(0, "some other page", root);
            indexer.Update(part);

            
            var query = Query.For("page")
                .OfType(typeof(IPage))
                .Below(root)
                .ReadableBy("Everyone") // superfluous (everyone by default)
                .Except(Query.For<PersistableItem2>());
            
            var result = searcher.Search(query);

            Assert.That(result.Hits.Single().Content, Is.EqualTo(root));
        }

        [Test]
        public void OrOr_And()
        {
            indexer.Update(root);
            var first = CreateOneItem<PersistableItem2>(0, "some other page", root);
            indexer.Update(first);
            var second = CreateOneItem<PersistableItem2>(0, "some other stuff", root);
            indexer.Update(second);

            
            // TODO: support this
            //var query = (Query.For("some") | Query.For("other"))
            //    .Below(first);
            var query = new Query { Ancestor = first.GetTrail(), Intersection = Query.For("some") | Query.For("other") };

            var result = searcher.Search(query);

            result.Hits.Count().ShouldBe(1);
            result.Hits.Any(h => h.Content == second).ShouldBe(false);
        }

        [Test]
        public void QueryByExpression_ForTitleProperty_StartsWith()
        {
            indexer.Update(root);

            
            var query = Query.For<PersistableItem>();

            query.Contains(pi => pi.Title, "ro*");
            var result = searcher.Search(query);

            Assert.That(result.Hits.Single().Content, Is.EqualTo(root));
        }

        [Test]
        public void QueryByExpression_ForVisibleProperty()
        {
            indexer.Update(root);

            
            var query = Query.For<PersistableItem>();

            query.Contains(pi => pi.Visible, "true");
            var result = searcher.Search(query);

            Assert.That(result.Hits.Single().Content, Is.EqualTo(root));
        }

        [Test]
        public void Tags_are_searchable()
        {
            var page = CreateOneItem<PersistableItem>(1, "page", null);
            page.Tags = new[] { "Hello", "World" };
            indexer.Update(page);

            var page2 = CreateOneItem<PersistableItem>(2, "page2", null);
            page2.Tags = new[] { "Howdy", "Universe" };
            indexer.Update(page2);

            
            var result = searcher.Search(Query.For("world"));

            result.Single().ShouldBe(page);
        }

        [Test]
        public void Tags_are_searchable_by_property()
        {
            var page = CreateOneItem<PersistableItem>(1, "page", null);
            page.Tags = new[] { "Hello", "World" };
            indexer.Update(page);

            var page2 = CreateOneItem<PersistableItem>(2, "page2", null);
            page2.Tags = new[] { "Howdy", "Universe" };
            indexer.Update(page2);

            
            var result = searcher.Search(Query.For<PersistableItem>().Property("Tags", "Hello"));

            result.Single().ShouldBe(page);
        }

        // sorting

        [TestCase(TextExtractor.Properties.ID)]
        [TestCase(TextExtractor.Properties.Created)]
        [TestCase(TextExtractor.Properties.Published)]
        [TestCase(TextExtractor.Properties.Updated)]
        [TestCase(TextExtractor.Properties.Expires)]
        [TestCase(TextExtractor.Properties.Title)]
        [TestCase(TextExtractor.Properties.Name)]
        [TestCase(TextExtractor.Properties.SortOrder)]
        [TestCase(TextExtractor.Properties.SavedBy)]
        public void OrderBy(string field)
        {
            PersistableItem item;
            PersistableItem item2;
            Create(out item, out item2);

            var hits = Search(field, descending:false);

            hits[0].Title.ShouldBe(item.Title);
            hits[1].Title.ShouldBe(item2.Title);
        }

        [TestCase(TextExtractor.Properties.ID)]
        [TestCase(TextExtractor.Properties.Created)]
        [TestCase(TextExtractor.Properties.Published)]
        [TestCase(TextExtractor.Properties.Updated)]
        [TestCase(TextExtractor.Properties.Expires)]
        [TestCase(TextExtractor.Properties.Title)]
        [TestCase(TextExtractor.Properties.Name)]
        [TestCase(TextExtractor.Properties.SortOrder)]
        [TestCase(TextExtractor.Properties.SavedBy)]
        public void OrderBy_Descending(string field)
        {
            PersistableItem item;
            PersistableItem item2;
            Create(out item, out item2);

            var hits = Search(field, descending:true);

            hits[0].Title.ShouldBe(item2.Title);
            hits[1].Title.ShouldBe(item.Title);
        }

        [TestCase(TextExtractor.Properties.ID)]
        [TestCase(TextExtractor.Properties.SortOrder)]
        public void OrderBy_IsNotAlphabetical_ForCertainFields(string field)
        {
            var item = CreateOneItem<PersistableItem>(99, "Another", null);
            item.SortOrder = 2;
            indexer.Update(item);

            var item2 = CreateOneItem<PersistableItem>(111, "Before", null);
            item2.SortOrder = 10;
            indexer.Update(item2);

            var hits = Search("SortOrder", descending: true);

            hits[0].Title.ShouldBe(item2.Title);
            hits[1].Title.ShouldBe(item.Title);
        }

        [Test]
        public void OrderBy_MultipleFields()
        {
            var apple1 = CreateOneItem<PersistableItem>(1, "Apple", null);
            apple1.SortOrder = 4;
            indexer.Update(apple1);

            var bear = CreateOneItem<PersistableItem>(2, "Bear", null);
            bear.SortOrder = 2;
            indexer.Update(bear);

            var cake = CreateOneItem<PersistableItem>(3, "Cake", null);
            cake.SortOrder = 3;
            indexer.Update(cake);

            var apple2 = CreateOneItem<PersistableItem>(4, "Apple", null);
            apple2.SortOrder = 1;
            indexer.Update(apple2);

            var hits = Search(new SortFieldData("Name", false), new SortFieldData("SortOrder"));

            hits[0].Title.ShouldBe(apple2.Title);
            hits[1].Title.ShouldBe(apple1.Title);
            hits[2].Title.ShouldBe(bear.Title);
            hits[3].Title.ShouldBe(cake.Title);
        }

        [Test]
        public void Update_PassingCustomText()
        {
            var apple1 = CreateOneItem<PersistableItem>(1, "Apple", null);
            apple1.SortOrder = 4;
            indexer.Update(apple1);

            var hits = Search(new SortFieldData("Name", false), new SortFieldData("SortOrder"));
        }

        // concurrency

        [TestCase(4, 1, 1000, 100, 50, .5)]
        //[TestCase(8, 2, 5000, 1000, 500, .5)]
        //[TestCase(16, 4, 30 * 1000, 10000, 1000, .5)]
        //[TestCase(8, 2, 300 * 1000, 20000, 2000, .5)]
        public void Multithreading(int readerCount, int indexerCount, int workMilliseconds, int dictionaryCount, int indexedWordsCount, double updateFrequency)
        {
            var threads = new List<Thread>();
            var exceptions = new List<Exception>();
            bool loop = true;
            
            var generator = new SCG.General.MarkovNameGenerator(Words.Thousand, 3, 2);
            var words = Enumerable.Range(0, dictionaryCount).Select(i => generator.NextName).ToArray();

            int idCounter = 1;
            int creates = 0;
            int updates = 0;
            var indexFunction = new ThreadStart(() =>
            {
                var r = new Random();
                Trace.WriteLine(Thread.CurrentThread.ManagedThreadId + " Index start: " + DateTime.Now);
                while (loop)
                {
                    bool isUpdate = r.NextDouble() < updateFrequency && idCounter > 2;
                    int id = (isUpdate) ? r.Next(0, idCounter) : Interlocked.Increment(ref idCounter);

                    var item = CreateOneItem<PersistableItem>(id, "Item " + id, null);
                    item.StringProperty = Enumerable.Range(0, indexedWordsCount).Select(i => words[r.Next(0, words.Length)]).Aggregate(new StringBuilder(), (sb, w) => sb.Append(w).Append(" ")).ToString();
                    try
                    {
                        indexer.Update(item);
                        Console.Write(isUpdate ? 'U' : 'C');
                        if (isUpdate)
                            Interlocked.Increment(ref updates);
                        else
                            Interlocked.Increment(ref creates);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                        lock (exceptions)
                        {
                            exceptions.Add(ex);
                        }
                    }
                }
                Trace.WriteLine(Thread.CurrentThread.ManagedThreadId + " Index stop: " + DateTime.Now);
            });
            
            int searches = 0;
            var searchFunction = new ThreadStart(() =>
            {
                Trace.WriteLine(Thread.CurrentThread.ManagedThreadId + " Search start: " + DateTime.Now);
                var r = new Random();

                while (loop)
                {
                    try
                    {
                        var result = searcher.Search(Query.For(words[r.Next(words.Length)]));
                        Interlocked.Increment(ref searches);
                        Console.Write('?');
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                        lock (exceptions)
                        {
                            exceptions.Add(ex);
                        }
                    }
                }

                Trace.WriteLine(Thread.CurrentThread.ManagedThreadId + " Search stop: " + DateTime.Now);
            });
            for (int i = 0; i < indexerCount; i++)
            {
            threads.Add(new Thread(indexFunction));
            }
            for (int i = 0; i < readerCount; i++)
            {
            threads.Add(new Thread(searchFunction));
            }

            foreach (var t in threads)
                t.Start();
            Thread.Sleep(workMilliseconds);
            loop = false;
            foreach (var t in threads)
                t.Join();

            Trace.WriteLine("Creates: " + creates + ", Updates: " + updates + ", Searches: " + searches + " Exceptions: " + exceptions.Count);

            foreach (var ex in exceptions)
                Trace.WriteLine(ex.Message);

            exceptions.Count.ShouldBe(0);
        }

        private System.Collections.Generic.List<ContentItem> Search(string field, bool descending)
        {
            return Search(new SortFieldData(field, descending));
        }

        private System.Collections.Generic.List<ContentItem> Search(params SortFieldData[] sortFields)
        {
            var query = Query.For<PersistableItem>();
            Array.ForEach(sortFields, sortField => query.OrderBy(sortField.SortField, sortField.SortDescending));

            var hits = searcher.Search(query).Hits.Select(h => h.Content).ToList();
            return hits;
        }

        private void Create(out PersistableItem item, out PersistableItem item2)
        {
            item = CreateOneItem<PersistableItem>(1, "Another", null);
            item.Published = DateTime.Now.AddSeconds(-10);
            item.Created = DateTime.Now.AddSeconds(-10);
            item.Updated = DateTime.Now.AddSeconds(-10);
            item.Expires = DateTime.Now.AddSeconds(10);
            item.SavedBy = "admin";
            item.SortOrder = 0;
            item.State = ContentState.Draft;
            indexer.Update(item);

            item2 = CreateOneItem<PersistableItem>(2, "Before", null);
            item2.Published = DateTime.Now;
            item2.Created = DateTime.Now;
            item2.Updated = DateTime.Now;
            item2.Expires = DateTime.Now.AddSeconds(20);
            item2.SavedBy = "bob";
            item2.SortOrder = 1;
            item2.State = ContentState.Published;
            indexer.Update(item2);
        }
    }
}
