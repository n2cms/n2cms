using N2.Persistence.Search;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Tests.Persistence.Xml
{
	[TestFixture]
	public class FindingContentSearcher_XmlRepositoryTests : XmlRepositoryTestsBase
	{
		private FindingContentSearcher searcher;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			searcher = new FindingContentSearcher(repository);
		}

		[Test]
		public void FindBy_Title()
		{
			var item = CreateOneItem<Definitions.PersistableItem>(0, "hello", null);
			repository.SaveOrUpdate(item);
			var notfound = CreateOneItem<Definitions.PersistableItem>(0, "bye", null);
			repository.SaveOrUpdate(notfound);

			var items = searcher.Search(Query.For("hello"));
			items.Single().ShouldBe(item);
		}

		[Test]
		public void FindBy_Detail()
		{
			var item = CreateOneItem<Definitions.PersistableItem>(0, "hello", null);
			item["world"] = "earth";
			repository.SaveOrUpdate(item);
			var notfound = CreateOneItem<Definitions.PersistableItem>(0, "bye", null);
			notfound["world"] = "something else";
			repository.SaveOrUpdate(notfound);

			var items = searcher.Search(Query.For("earth"));
			items.Single().ShouldBe(item);
		}

		[Test]
		public void FindBy_TitleContains()
		{
			var item = CreateOneItem<Definitions.PersistableItem>(0, "hello world", null);
			repository.SaveOrUpdate(item);
			var notfound = CreateOneItem<Definitions.PersistableItem>(0, "bye", null);
			repository.SaveOrUpdate(notfound);

			var items = searcher.Search(Query.For("world"));
			items.Single().ShouldBe(item);
		}

		[Test]
		public void FindBy_DetailContains()
		{
			var item = CreateOneItem<Definitions.PersistableItem>(0, "hello world", null);
			item["world"] = "mother earth";
			repository.SaveOrUpdate(item);
			var notfound = CreateOneItem<Definitions.PersistableItem>(0, "bye", null);
			notfound["world"] = "something else";
			repository.SaveOrUpdate(notfound);

			var items = searcher.Search(Query.For("earth"));
			items.Single().ShouldBe(item);
		}

		[Test]
		public void FindBy_Below()
		{
			var root = CreateOneItem<Definitions.PersistableItem>(0, "root", null);
			repository.SaveOrUpdate(root);
			var item = CreateOneItem<Definitions.PersistableItem>(0, "hello", root);
			repository.SaveOrUpdate(item);
			var child = CreateOneItem<Definitions.PersistableItem>(0, "child", item);
			repository.SaveOrUpdate(child);
			var notfound = CreateOneItem<Definitions.PersistableItem>(0, "bye", root);
			repository.SaveOrUpdate(notfound);

			var items = searcher.Search(new Query().Below(item));
			items.Single().ShouldBe(child);
		}

		[Test]
		public void FindBy_Type()
		{
			var item = CreateOneItem<Definitions.PersistableItem>(0, "hello", null);
			repository.SaveOrUpdate(item);
			var notfound = CreateOneItem<Definitions.PersistablePart>(0, "bye", null);
			repository.SaveOrUpdate(notfound);

			var items = searcher.Search(new Query().OfType(typeof(Definitions.PersistableItem)));
			items.Single().ShouldBe(item);
		}

		[Test]
		public void FindBy_Pages()
		{
			var item = CreateOneItem<Definitions.PersistableItem>(0, "hello", null);
			repository.SaveOrUpdate(item);
			var notfound = CreateOneItem<Definitions.PersistablePart>(0, "bye", null);
			notfound.ZoneName = "Left";
			repository.SaveOrUpdate(notfound);

			var items = searcher.Search(new Query().Pages(true));
			items.Single().ShouldBe(item);
		}

		[Test]
		public void FindBy_Parts()
		{
			var item = CreateOneItem<Definitions.PersistableItem>(0, "hello", null);
			repository.SaveOrUpdate(item);
			var part = CreateOneItem<Definitions.PersistablePart>(0, "bye", null);
			part.ZoneName = "Left";
			repository.SaveOrUpdate(part);

			var items = searcher.Search(new Query().Pages(false));
			items.Single().ShouldBe(part);
		}

		[Test]
		public void FindBy_All()
		{
			var item = CreateOneItem<Definitions.PersistableItem>(0, "hello", null);
			repository.SaveOrUpdate(item);
			var part = CreateOneItem<Definitions.PersistablePart>(0, "bye", null);
			repository.SaveOrUpdate(part);

			var items = searcher.Search(new Query());
			items.Count.ShouldBe(2);
			items.ShouldContain(item);
			items.ShouldContain(part);
		}
	}
}
