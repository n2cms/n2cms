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
using N2.Engine;
using System;

namespace N2.Tests.Persistence.NH
{
	[TestFixture]
	public class ContentChangeTrackerTests : ItemPersistenceMockingBase
	{
		LuceneIndexer indexer;
		LuceneAccesor accessor;
		LuceneSearcher searcher;
		ContentChangeTracker tracker;
		PersistableItem1 root;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			var definitions = TestSupport.SetupDefinitions(typeof(PersistableItem1), typeof(PersistablePart1));

			accessor = new LuceneAccesor(new ThreadContext(), new DatabaseSection());
			indexer = new LuceneIndexer(accessor, new TextExtractor(new IndexableDefinitionExtractor(definitions)));
			searcher = new LuceneSearcher(accessor, persister);
			var worker = new AsyncWorker();
			tracker = new ContentChangeTracker(indexer, persister, worker, Rhino.Mocks.MockRepository.GenerateStub<IErrorNotifier>(), new DatabaseSection());

			accessor.LockTimeout = 1L;
			worker.QueueUserWorkItem = (cb) => { cb.Invoke(null); return true; };
			indexer.Clear();
			root = CreateOneItem<PersistableItem1>(1, "The Root Page", null);
		}

		public override void TearDown()
		{
			base.TearDown();
			indexer.Unlock();
		}

		[Test]
		public void LockTimeoutException_WillNotIndexImmediately()
		{
			var world = CreateOneItem<PersistableItem1>(2, "hello world", root);

			accessor.GetDirectory().MakeLock("write.lock").Obtain();
			tracker.ItemChanged(world.ID, false);

			Assert.That(searcher.Search("hello").Hits.Count(), Is.EqualTo(0));
		}

		[Test]
		public void LockTimeoutException_WillRetryIndex_AfterSomeTime()
		{
			var world = CreateOneItem<PersistableItem1>(2, "hello world", root);
			var universe = CreateOneItem<PersistableItem1>(3, "hello universe", root);

			accessor.GetDirectory().MakeLock("write.lock").Obtain();
			tracker.ItemChanged(world.ID, false);

			indexer.Unlock();
			tracker.RetryInterval = TimeSpan.FromSeconds(0);
			tracker.ItemChanged(universe.ID, false);

			Assert.That(searcher.Search("hello").Hits.Count(), Is.EqualTo(2));
		}

		[Test]
		public void CanUnlock_AlreadyUnlocked()
		{
			indexer.Unlock();
			indexer.Unlock();
		}

		[Test]
		public void Clear_RemovesIndexedData()
		{
			indexer.Update(root);
			indexer.Clear();

			Assert.That(searcher.Search("root").Hits.Count(), Is.EqualTo(0));
		}
	}
}
