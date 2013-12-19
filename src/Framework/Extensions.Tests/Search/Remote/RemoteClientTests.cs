using N2.Persistence.Search;
using N2.Search.Remote.Client;
using N2.Search.Remote.Server;
using N2.Tests;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Extensions.Tests.Search.Remote
{
    [TestFixture]
    public class RemoteClientTests : ItemPersistenceMockingBase
    {
        private Definitions.IDefinitionManager definitions;
        private PersistableItem root;
        private IndexerServer server;
        private IndexerClient indexer;
        private SearcherClient searcher;
        private TextExtractor extractor;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
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
            indexer = new IndexerClient(new Configuration.DatabaseSection());
            searcher = new SearcherClient(persister, new Configuration.DatabaseSection());
            extractor = new TextExtractor();
        }

        [Test]
        public void IndexedDocument_IsSearchable()
        {
            var doc = extractor.CreateDocument(root);

            indexer.Update(doc);

            var result = searcher.Search(Query.For("root"));
            result.Hits.Single().Title.ShouldBe("The Root Page");
        }

        [Test]
        public void IndexedDocument_WhichIsDeleted_IsNotSearchable()
        {
            var doc = extractor.CreateDocument(root);
            indexer.Update(doc);

            indexer.Delete(doc.ID);

            var result = searcher.Search(Query.For("root"));
            result.Hits.Count().ShouldBe(0);
        }

        [Test]
        public void IndexedDocument_UpdatesStatistics()
        {
            var doc = extractor.CreateDocument(root);
            indexer.Update(doc);

            indexer.GetStatistics().TotalDocuments.ShouldBe(1);
        }

        [Test]
        public void Indexed_WhichWasCleard_IsEmpty()
        {
            var doc = extractor.CreateDocument(root);
            indexer.Update(doc);

            indexer.Clear();

            var result = searcher.Search(Query.For("root"));
            result.Hits.Count().ShouldBe(0);
        }
    }
}
