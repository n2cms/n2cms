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

namespace N2.Tests.Persistence.NH
{
	[TestFixture]
	public class LuceneSearchTests : ItemPersistenceMockingBase
	{
		LuceneIndexer indexer;
		LuceneAccesor accessor;
		PersistableItem1 root;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			var definitions = TestSupport.SetupDefinitions(typeof(PersistableItem1), typeof(PersistablePart1));

			accessor = new LuceneAccesor(new ThreadContext(), new DatabaseSection());
			indexer = new LuceneIndexer(accessor, new TextExtractor(new IndexableDefinitionExtractor(definitions)));
			root = CreateOneItem<PersistableItem1>(1, "The Root Page", null);
			indexer.Clear();
		}

		[Test]
		public void LuceneSearch_OnTitle()
		{
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("hello"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
		}

		[Test]
		public void NHibernateSearch_IndexableProperty()
		{
			var item = CreateOneItem<Definitions.PersistableItem1>(2, "Hello world", root);
			item.StringProperty = "Hej Världen";
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("världen"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
		}

		[Test]
		public void NHibernateSearch_EditableProperty()
		{
			var item = CreateOneItem<Definitions.PersistableItem1>(2, "Hello world", root);
			item.IntProperty = 444;
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("444"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
		}

		[Test]
		public void MultipleKeywords()
		{
			indexer.Update(root);
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("hello root"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(2));
		}

		[Test]
		public void TitleBoost()
		{
			root.StringProperty = "Hello world";
			indexer.Update(root);

			var item = CreateOneItem<PersistableItem1>(2, root.StringProperty, root);
			item.StringProperty = root.Title;
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);

			var hits1 = searcher.Search(SearchQuery.For("hello"));
			var hits2 = searcher.Search(SearchQuery.For("root"));

			Assert.That(hits1.Hits.Select(h => h.Content).First(), Is.EqualTo(item));
			Assert.That(hits2.Hits.Select(h => h.Content).First(), Is.EqualTo(root));
		}

		[Test]
		public void Below()
		{
			var item1 = CreateOneItem<PersistableItem1>(2, "first item", root);
			indexer.Update(item1);
			var item1b = CreateOneItem<PersistableItem1>(3, "first item B", item1);
			indexer.Update(item1b);
			var item2 = CreateOneItem<PersistableItem1>(4, "second item", root);
			indexer.Update(item2);
			var item2b = CreateOneItem<PersistableItem1>(5, "second item B", item2);
			indexer.Update(item2b);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("item").Below(item1));

			Assert.That(hits.Hits.Count(), Is.EqualTo(2));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(item1));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(item1b));
		}

		[Test]
		public void FindOnlyPages()
		{
			var page1 = CreateOneItem<PersistableItem1>(2, "first page", root);
			indexer.Update(page1);
			var part1 = CreateOneItem<PersistablePart1>(3, "first part", page1);
			indexer.Update(part1);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("first").Pages(true));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(page1));
		}

		[Test]
		public void FindOnlyParts()
		{
			var page1 = CreateOneItem<PersistableItem1>(2, "first page", root);
			indexer.Update(page1);
			var part1 = CreateOneItem<PersistablePart1>(3, "first part", page1);
			indexer.Update(part1);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("first").Pages(false));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(part1));
		}

		[Test]
		public void FindPage_ByText_OnPart_BelowPage()
		{
			var page1 = CreateOneItem<PersistableItem1>(2, "first page", root);
			var part1 = CreateOneItem<PersistablePart1>(3, "first part", page1);
			part1.ZoneName = "Zone";
			indexer.Update(page1);
			indexer.Update(part1);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("part").Pages(true));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(page1));
		}

		[Test]
		public void DeletedItems_DoesntShowUp_InSearch()
		{
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			indexer.Update(item);
			indexer.Delete(item.ID);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("hello"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(0));
		}

		[Test]
		public void ChildrenOf_DeletedItems_DoesntShowUp_InSearch()
		{
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var child = CreateOneItem<PersistableItem1>(3, "Child world", item);
			indexer.Update(item);
			indexer.Update(child);

			indexer.Delete(item.ID);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("world"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(0));
		}

		[Test]
		public void PagesWithoutMatchingRoles_AreNotDisplayed_WhenSearching_WithRoles()
		{
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			item.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(item, "Members"));
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("hello").ReadableBy("Everyone"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(0));
		}

		[Test]
		public void AuthorizedPages_AreDisplayed_WhenSearching_WithoutSpecifyingRoles()
		{
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			item.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(item, "Members"));
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("hello"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
		}

		[Test]
		public void PagesWithMatchingRole_AreDisplayed_WhenSearching_WithRoles()
		{
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			item.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(item, "Members"));
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("hello").ReadableBy("Members"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
		}

		[Test]
		public void PagesWithMatching_RoleIntersection_AreDisplayed_WhenSearching_WithRoles()
		{
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			item.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(item, "Members"));
			item.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(item, "Writers"));
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("hello").ReadableBy("Writers", "Editors"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
		}

		[Test]
		public void UnSecuredPages_AreDisplayed_WhenSearching_WithRoles()
		{
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(SearchQuery.For("hello").ReadableBy("Members"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Optimize()
		{
			indexer.Update(CreateOneItem<PersistableItem1>(2, "Hello stockholm", root));
			indexer.Update(CreateOneItem<PersistableItem1>(3, "Hello world", root));
			indexer.Update(CreateOneItem<PersistableItem1>(4, "Hello universe", root));

			indexer.Optimize();

			Assert.IsTrue(true);
		}
	}
}
