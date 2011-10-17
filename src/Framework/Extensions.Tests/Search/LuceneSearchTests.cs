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

			var definitions = TestSupport.SetupDefinitions(typeof(PersistableItem1), typeof(PersistableItem2), typeof(PersistablePart1));

			accessor = new LuceneAccesor(new ThreadContext(), new DatabaseSection());
			indexer = new LuceneIndexer(accessor, new TextExtractor(new IndexableDefinitionExtractor(definitions)));
			root = CreateOneItem<PersistableItem1>(1, "The Root Page", null);
			indexer.Clear();
		}

		[TearDown]
		public override void TearDown()
		{
			accessor.Dispose();
			base.TearDown();
		}

		[Test]
		public void Title()
		{
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Skip()
		{
			indexer.Update(CreateOneItem<PersistableItem1>(3, "Hello country", root));
			indexer.Update(CreateOneItem<PersistableItem1>(2, "Hello world", root));
			indexer.Update(CreateOneItem<PersistableItem1>(4, "Hello universe", root));

			var searcher = new LuceneSearcher(accessor, persister);
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
			indexer.Update(CreateOneItem<PersistableItem1>(3, "Hello country", root));
			indexer.Update(CreateOneItem<PersistableItem1>(2, "Hello world", root));
			indexer.Update(CreateOneItem<PersistableItem1>(4, "Hello universe", root));

			var searcher = new LuceneSearcher(accessor, persister);
			var hits1 = searcher.Search(Query.For("hello").Take(1));

			Assert.That(hits1.Count, Is.EqualTo(1));
		}

		[Test]
		public void Count_IsNumberOfItemsInHits()
		{
			indexer.Update(CreateOneItem<PersistableItem1>(3, "Hello country", root));
			indexer.Update(CreateOneItem<PersistableItem1>(2, "Hello world", root));
			indexer.Update(CreateOneItem<PersistableItem1>(4, "Hello universe", root));

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello").Take(2));

			Assert.That(hits.Hits.Count(), Is.EqualTo(2));
			Assert.That(hits.Count, Is.EqualTo(2));
		}

		[Test]
		public void Total_IsNumberOfItemsInIndex()
		{
			indexer.Update(CreateOneItem<PersistableItem1>(3, "Hello country", root));
			indexer.Update(CreateOneItem<PersistableItem1>(2, "Hello world", root));
			indexer.Update(CreateOneItem<PersistableItem1>(4, "Hello universe", root));

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello").Take(2));

			Assert.That(hits.Total, Is.EqualTo(3));
		}

		[Test]
		public void IndexableProperty()
		{
			var item = CreateOneItem<Definitions.PersistableItem1>(2, "Hello world", root);
			item.StringProperty = "Hej Världen";
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("världen"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
		}

		[Test]
		public void EditableProperty()
		{
			var item = CreateOneItem<Definitions.PersistableItem1>(2, "Hello world", root);
			item.IntProperty = 444;
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("444"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
		}

		[Test]
		public void MultipleKeywords()
		{
			indexer.Update(root);
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello root"));

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

			var hits1 = searcher.Search(Query.For("hello"));
			var hits2 = searcher.Search(Query.For("root"));

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
			var hits = searcher.Search(Query.For("item").Below(item1));

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
			var hits = searcher.Search(Query.For("first").Pages(true));

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
			var hits = searcher.Search(Query.For("first").Pages(false));

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
			var hits = searcher.Search(Query.For("part").Pages(true));

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
			var hits = searcher.Search(Query.For("hello"));

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
			var hits = searcher.Search(Query.For("world"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(0));
		}

		[Test]
		public void PagesWithoutMatchingRoles_AreNotDisplayed_WhenSearching_WithRoles()
		{
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			item.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(item, "Members"));
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello").ReadableBy("Everyone"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(0));
		}

		[Test]
		public void AuthorizedPages_AreDisplayed_WhenSearching_WithoutSpecifyingRoles()
		{
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			item.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(item, "Members"));
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
		}

		[Test]
		public void PagesWithMatchingRole_AreDisplayed_WhenSearching_WithRoles()
		{
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			item.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(item, "Members"));
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello").ReadableBy("Members"));

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
			var hits = searcher.Search(Query.For("hello").ReadableBy("Writers", "Editors"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
		}

		[Test]
		public void UnSecuredPages_AreDisplayed_WhenSearching_WithRoles()
		{
			var item = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			indexer.Update(item);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello").ReadableBy("Members"));

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

		[Test]
		public void PagesCanBeFoundByType()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var part = CreateOneItem<PersistablePart1>(3, "Hello region", root);
			indexer.Update(page);
			indexer.Update(part);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello").OfType(typeof(PersistablePart1)));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
		}

		[Test]
		public void PagesCanBeFound_ByContentType()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var part = CreateOneItem<PersistablePart1>(3, "Hello region", root);
			indexer.Update(page);
			indexer.Update(part);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello").OfType(typeof(PersistablePart1)));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
		}

		[Test]
		public void PagesCanBeFound_ByInterfaceType()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var part = CreateOneItem<PersistablePart1>(3, "Hello region", root);
			indexer.Update(page);
			indexer.Update(part);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello").OfType(typeof(IPart)));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
		}

		[Test]
		public void PagesCanBeFound_ByMultipleTypes()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var part = CreateOneItem<PersistablePart1>(3, "Hello region", root);
			indexer.Update(page);
			indexer.Update(part);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello").OfType(typeof(IPart), typeof(IPage)));

			Assert.That(hits.Hits.Count(), Is.EqualTo(2));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(page));
		}

		[Test]
		public void PagesCanBeFound_ByObjectType()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var part = CreateOneItem<PersistablePart1>(3, "Hello region", root);
			indexer.Update(page);
			indexer.Update(part);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello").OfType(typeof(object)));

			Assert.That(hits.Hits.Count(), Is.EqualTo(2));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(page));
		}

		[Test]
		public void Except_Text()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var part = CreateOneItem<PersistablePart1>(3, "Hello region", root);
			indexer.Update(page);
			indexer.Update(part);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello").Except("world"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
		}

		[Test]
		public void Except_Type()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var part = CreateOneItem<PersistablePart1>(3, "Hello region", root);
			indexer.Update(page);
			indexer.Update(part);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello").Except(typeof(IPage)));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
		}

		[Test]
		public void Except_Operator()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var part = CreateOneItem<PersistablePart1>(3, "Hello region", root);
			indexer.Update(page);
			indexer.Update(part);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello") - Query.For(typeof(IPage)));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
		}

		[Test]
		public void Except_Operator_Multiple()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var part = CreateOneItem<PersistablePart1>(3, "Hello world", root);
			var part2 = CreateOneItem<PersistablePart1>(4, "Hello region", root);
			indexer.Update(page);
			indexer.Update(part);
			indexer.Update(part2);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello") - Query.For(typeof(IPage)) - Query.For("region"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
			Assert.That(hits.Hits.Select(h => h.Content).Contains(part));
		}

		[Test]
		public void And()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var page2 = CreateOneItem<PersistableItem1>(3, "Hello region", root);
			indexer.Update(page);
			indexer.Update(page2);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello").And(Query.For("world")));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
			Assert.That(hits.Contains(page));
		}

		[Test]
		public void And_Operator()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var page2 = CreateOneItem<PersistableItem1>(3, "Hello region", root);
			indexer.Update(page);
			indexer.Update(page2);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello") & Query.For("world"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
			Assert.That(hits.Contains(page));
		}

		[Test]
		public void And_Operator_Multiple()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world hunger", root);
			var page2 = CreateOneItem<PersistableItem1>(3, "Hello world fulfillment", root);
			indexer.Update(page);
			indexer.Update(page2);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("hello") & Query.For("world") & Query.For("hunger"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(1));
			Assert.That(hits.Contains(page));
		}

		[Test]
		public void Or()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var page2 = CreateOneItem<PersistableItem1>(3, "Hello region", root);
			indexer.Update(page);
			indexer.Update(page2);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("region").Or(Query.For("world")));

			Assert.That(hits.Hits.Count(), Is.EqualTo(2));
			Assert.That(hits.Contains(page));
			Assert.That(hits.Contains(page2));
		}

		[Test]
		public void Or_Operator()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var page2 = CreateOneItem<PersistableItem1>(3, "Hello region", root);
			indexer.Update(page);
			indexer.Update(page2);

			var searcher = new LuceneSearcher(accessor, persister);
			var hits = searcher.Search(Query.For("region") | Query.For("world"));

			Assert.That(hits.Hits.Count(), Is.EqualTo(2));
			Assert.That(hits.Contains(page));
			Assert.That(hits.Contains(page2));
		}

		[Test]
		public void Or_Operator_Multiple()
		{
			var page = CreateOneItem<PersistableItem1>(2, "Hello world", root);
			var page2 = CreateOneItem<PersistableItem1>(3, "Hello region", root);
			var page3 = CreateOneItem<PersistableItem1>(4, "Hello universe", root);
			indexer.Update(page);
			indexer.Update(page2);
			indexer.Update(page3);

			var searcher = new LuceneSearcher(accessor, persister);
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

			var svitem = CreateOneItem<PersistableItem1>(4, "Hello världen", sv);
			indexer.Update(svitem);

			var enitem = CreateOneItem<PersistableItem1>(5, "Hello world", en);
			indexer.Update(enitem);

			var searcher = new LuceneSearcher(accessor, persister);
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

			var svitem = CreateOneItem<PersistableItem1>(4, "Hello världen", sv);
			indexer.Update(svitem);

			var enitem = CreateOneItem<PersistableItem1>(5, "Hello world", en);
			indexer.Update(enitem);

			var searcher = new LuceneSearcher(accessor, persister);
			var result = searcher.Search(Query.For("hello").Language(sv.LanguageCode));

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

			var svitem = CreateOneItem<PersistableItem1>(4, "Hello världen", sv);
			indexer.Update(svitem);

			var enitem = CreateOneItem<PersistableItem1>(5, "Hello world", en);
			indexer.Update(enitem);

			var searcher = new LuceneSearcher(accessor, persister);
			var result = searcher.Search(Query.For("").Language(sv));

			Assert.That(result.Hits.Count(), Is.EqualTo(2));
			Assert.That(result.Contains(sv));
		}

		[Test]
		public void NonDetail_IndexableProperty_IsIndexed()
		{
			root.NonDetailProperty = "Lorem dolor";
			indexer.Update(root);

			var searcher = new LuceneSearcher(accessor, persister);
			var result = searcher.Search(Query.For("dolor"));

			Assert.That(result.Hits.Single().Content, Is.EqualTo(root));
		}

		[Test]
		public void NonDetail_IndexableOnlyGetterProperty_IsIndexed()
		{
			indexer.Update(root);

			var searcher = new LuceneSearcher(accessor, persister);
			var result = searcher.Search(Query.For("ipsum"));

			Assert.That(result.Hits.Single().Content, Is.EqualTo(root));
		}

		[Test]
		public void NonIndexableClass_IsNotIndexed()
		{
			var child = CreateOneItem<PersistableItem1b>(2, "child", root);
			indexer.Update(child);

			var searcher = new LuceneSearcher(accessor, persister);
			var result = searcher.Search(Query.For("child"));

			Assert.That(result.Hits.Any(), Is.False);
		}

        [Test]
        public void QueryByExpression_ForDetail()
        {
            root.StringProperty = "This is a very special string";
            indexer.Update(root);

            var searcher = new LuceneSearcher(accessor, persister);
            var query = Query.For<PersistableItem1>();

            query.Contains(pi => pi.StringProperty, "special");
            var result = searcher.Search(query);

            Assert.That(result.Hits.Single().Content, Is.EqualTo(root));
        }

        [Test]
        public void QueryByExpression_ForBaseProperty()
        {
            indexer.Update(root);

            var searcher = new LuceneSearcher(accessor, persister);
            var query = Query.For<PersistableItem1>();

            query.Contains(pi => pi.Title, "root");
            var result = searcher.Search(query);

            Assert.That(result.Hits.Single().Content, Is.EqualTo(root));
        }
	}
}
