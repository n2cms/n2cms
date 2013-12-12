using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Tests;
using N2.Persistence.Search;
using N2.Engine;
using N2.Tests.Persistence.Definitions;
using N2.Web;
using N2.Configuration;
using System.Threading;
using System.Diagnostics;

namespace N2.Extensions.Tests.Search
{
    [TestFixture]
    public class AsyncIndexerTests : ItemPersistenceMockingBase
    {
        ContentIndexer indexer;
        LuceneAccesor accessor;
        LuceneContentSearcher searcher;
        ContentChangeTracker tracker;
        AsyncIndexer asyncIndexer;
        AsyncWorker worker;

        PersistableItem root;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var definitions = TestSupport.SetupDefinitions(typeof(PersistableItem), typeof(PersistableItem2), typeof(PersistablePart));

            accessor = new LuceneAccesor(new ThreadContext(), new DatabaseSection());
            indexer = new ContentIndexer(new LuceneIndexer(accessor), new TextExtractor(new IndexableDefinitionExtractor(definitions)));
            searcher = new LuceneContentSearcher(accessor, persister);
            worker = new AsyncWorker();
            asyncIndexer = new AsyncIndexer(indexer, persister, worker, Rhino.Mocks.MockRepository.GenerateStub<IErrorNotifier>(), new DatabaseSection());
            tracker = new ContentChangeTracker(asyncIndexer, persister, new N2.Plugin.ConnectionMonitor(), new DatabaseSection());

            accessor.LockTimeout = 1L;
            indexer.Clear();
            root = CreateOneItem<PersistableItem>(1, "The Root Page", null);
        }

        [TestCase(50)]
        //[TestCase(500)]
        //[TestCase(5000)]
        public void AsyncIndexer(int actionCount)
        {
            var world = CreateOneItem<PersistableItem>(2, "hello world", root);
            var universe = CreateOneItem<PersistableItem>(3, "hello universe", root);

            var generator = new SCG.General.MarkovNameGenerator(Words.Thousand, 3, 2);
            var words = Enumerable.Range(0, 1000).Select(i => generator.NextName).ToArray();
            var items = new [] { root, world, universe };
            var r = new Random();
            for (int i = 0; i < actionCount; i++)
            {
                if (r.NextDouble() < .75)
                {
                    var item = items[r.Next(0, items.Length)];
                    item.StringProperty = Enumerable.Range(0, 50).Select(index => words[r.Next(0, words.Length)]).Aggregate(new StringBuilder(), (sb, w) => sb.Append(w).Append(" ")).ToString();
                    asyncIndexer.Reindex(item.ID, false);
                }
                else
                    asyncIndexer.Delete(items[r.Next(0, items.Length)].ID);
                Thread.Sleep(1);
            }

            int times = 0;
            while (asyncIndexer.GetCurrentStatus().QueueSize > 0)
            {
                Debug.WriteLine("Waiting " + times + " times on thread " + Thread.CurrentThread.ManagedThreadId);
                times++;
                Thread.Sleep(100);
                if (times > actionCount)
                    throw new Exception("Waited for a long time for indexing to finish");
            }
        }
    }
}
