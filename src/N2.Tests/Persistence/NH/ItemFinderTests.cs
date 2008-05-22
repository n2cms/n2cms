using System;
using System.Collections.Generic;
using NUnit.Framework;
using N2.Collections;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using N2.Tests.Persistence.Definitions;

namespace N2.Tests.Persistence.NH
{
	[TestFixture]
	public class ItemFinderTests : DatabasePreparingBase
	{
		#region SetUp

		IItemFinder finder;
		ContentItem rootItem;
		ContentItem startPage;
		ContentItem item1;
		ContentItem item2;
		ContentItem item3;

		[TestFixtureSetUp]
		public override void TestFixtureSetUp()
		{
			base.TestFixtureSetUp();
			base.CreateDatabaseSchema();

			CreateRootItem();
			SaveVersionAndUpdateRootItem();
			CreateStartPage();
			item1 = CreatePageBelowStartPage(1);
			item2 = CreatePageBelowStartPage(2);
			item3 = CreatePageBelowStartPage(3);

			engine.UrlParser.CurrentSite.RootItemID = rootItem.ID;
			engine.UrlParser.CurrentSite.StartPageID = startPage.ID;

			ISessionProvider sessionProvider = engine.Resolve<ISessionProvider>();
			finder = new ItemFinder(sessionProvider, engine.Definitions);
		}

		private ContentItem CreatePageBelowStartPage(int index)
		{
			ContentItem item;
			item = CreateOneItem<PersistableItem2>(0, "item" + index, startPage);

			N2.Details.DetailCollection details = item.GetDetailCollection("DetailCollection", true);
			details.Add(true);
			details.Add(index * 1000 + 555);
			details.Add(index * 1000.0 + 555.55);
			details.Add("string in a collection " + index);
			details.Add(startPage);
			details.Add(new DateTime(2009 + index, 1, 1));

			engine.Persister.Save(item);
			return item;
		}

		[SetUp]
		public override void SetUp()
		{
			engine.Persister.Dispose();
		}

		private void CreateStartPage()
		{
			startPage = CreateOneItem<PersistableItem1>(0, "start page", rootItem);
			startPage.ZoneName = "AZone";
			startPage.SortOrder = 34;
			startPage.Visible = true;
			startPage["IntDetail"] = 45;
			startPage["DoubleDetail"] = 56.66;
			startPage["BoolDetail"] = true;
			startPage["DateDetail"] = new DateTime(2000, 01, 01);
			startPage["StringDetail"] = "actually another string";
			startPage["StringDetail2"] = "just a string";
			startPage["ObjectDetail"] = new string[] { "two", "three", "four" };
			startPage["ItemDetail"] = rootItem;

			engine.Persister.Save(startPage);
		}

		private void SaveVersionAndUpdateRootItem()
		{
			engine.Resolve<IVersionManager>().SaveVersion(rootItem);

			rootItem.Created = DateTime.Today;
			rootItem.Published = new DateTime(2007, 06, 03);
			rootItem.Expires = new DateTime(2017, 06, 03);
			rootItem.ZoneName = "TheZone";
			rootItem.SortOrder = 23;
			rootItem.Visible = false;
			rootItem["IntDetail"] = 43;
			rootItem["DoubleDetail"] = 43.33;
			rootItem["BoolDetail"] = false;
			rootItem["DateDetail"] = new DateTime(1999, 12, 31);
			rootItem["StringDetail"] = "just a string";
			rootItem["StringDetail2"] = "just another string";
			rootItem["ObjectDetail"] = new string[] { "one", "two", "three" };

			engine.Persister.Save(rootItem);
		}

		private void CreateRootItem()
		{
			rootItem = CreateOneItem<PersistableItem1>(0, "root", null);
			rootItem.Created = new DateTime(2007, 06, 01);
			rootItem.Published = new DateTime(2007, 06, 02);
			rootItem.Expires = new DateTime(2017, 06, 02);
			rootItem.ZoneName = "ZaZone";
			rootItem.SortOrder = 12;
			rootItem.Visible = true;
			rootItem["IntDetail"] = 32;
			rootItem["DoubleDetail"] = 32.22;
			rootItem["BoolDetail"] = true;
			rootItem["DateDetail"] = new DateTime(1998, 12, 31);
			rootItem["StringDetail"] = "a string in a version";
			rootItem["StringDetail2"] = "just a string";
			rootItem["ObjectDetail"] = new string[] { "zero", "one", "two" };

			engine.Persister.Save(rootItem);
		}
		
		#endregion

		[Test]
		public void ByPropertyID()
		{
			IList<ContentItem> items = finder.Where.ID.Eq(rootItem.ID).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByPropertyParent()
		{
			IList<ContentItem> items = finder.Where.Parent.Eq(rootItem).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(startPage, items[0]);
		}

		[Test]
		public void ByPropertyTitle()
		{
			IList<ContentItem> items = finder.Where.Title.Eq(rootItem.Title).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByPropertyName()
		{
			IList<ContentItem> items = finder.Where.Name.Eq(rootItem.Name).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByPropertyZoneName()
		{
			IList<ContentItem> items = finder.Where.ZoneName.Eq(rootItem.ZoneName).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByPropertyCreated()
		{
			IList<ContentItem> items = finder.Where.Created.Eq(rootItem.Created).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByPropertyCreatedBetween()
		{
			IList<ContentItem> items = finder
				.Where.Created.Between(rootItem.Created.AddMinutes(-1), rootItem.Created.AddMinutes(1))
				.Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByPropertyPublished()
		{
			IList<ContentItem> items = finder.Where.Published.Eq(rootItem.Published.Value).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByPropertyExpires()
		{
			IList<ContentItem> items = finder.Where.Expires.Eq(rootItem.Expires.Value).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByPropertySortOrder()
		{
			IList<ContentItem> items = finder.Where.SortOrder.Eq(rootItem.SortOrder).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByPropertyVisible()
		{
			IList<ContentItem> items = finder.Where.Visible.Eq(rootItem.Visible).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByPropertyNameAndTitle()
		{
			IList<ContentItem> items = finder.Where.Name.Eq("root").And.Title.Eq("root").Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void TwoItemsByNameOrTitle()
		{
			IList<ContentItem> items = finder.Where.Name.Eq("root").Or.Title.Eq("start page").Select();
			Assert.AreEqual(2, items.Count);
			EnumerableAssert.Contains(items, rootItem);
			EnumerableAssert.Contains(items, startPage);
		}

		[Test]
		public void ByPropertyVersionOf()
		{
			IList<ContentItem> items = finder.Where.VersionOf.Eq(rootItem).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreNotEqual(rootItem, items[0]);
		}

		[Test]
		public void ByPropertyVersionOfOrVersionOf()
		{
			IList<ContentItem> items = finder.Where
				.VersionOf.Eq(rootItem)
				.Or.VersionOf.Eq(startPage)
				.Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreNotEqual(rootItem, items[0]);
		}

		[Test]
		public void ByPropertySavedBy()
		{
			IList<ContentItem> items = finder.Where.SavedBy.Eq(rootItem.SavedBy).Select();
			Assert.AreEqual(5, items.Count);
		}

		[Test]
		public void IntDetail()
		{
			IList<ContentItem> items = finder.Where.Detail("IntDetail").Eq(43).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void AnyIntDetail()
		{
			IList<ContentItem> items = finder.Where.Detail().Eq(43).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void DoubleDetail()
		{
			IList<ContentItem> items = finder.Where.Detail("DoubleDetail").Eq(43.33).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void AnyDoubleDetail()
		{
			IList<ContentItem> items = finder.Where.Detail().Eq(43.33).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void BoolDetail()
		{
			IList<ContentItem> items = finder.Where.Detail("BoolDetail").Eq(false).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void AnyBoolDetail()
		{
			IList<ContentItem> items = finder.Where.Detail().Eq(false).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void DateDetail()
		{
			IList<ContentItem> items = finder.Where.Detail("DateDetail").Eq(new DateTime(1999, 12, 31)).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void AnyDateDetail()
		{
			IList<ContentItem> items = finder.Where.Detail().Eq(new DateTime(1999, 12, 31)).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void StringDetail()
		{
			IList<ContentItem> items = finder.Where.Detail("StringDetail").Eq("just a string").Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void AnyStringDetail()
		{
			IList<ContentItem> items = finder.Where.Detail().Eq("just a string").Select();
			Assert.AreEqual(2, items.Count);
			EnumerableAssert.Contains(items, rootItem);
			EnumerableAssert.Contains(items, startPage);
		}

		[Test]
		public void DetailBetween()
		{
			IList<ContentItem> items = finder.Where
				.Detail("IntDetail").Between(42, 44)
				.Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void AnyDetailBetween()
		{
			IList<ContentItem> items = finder
				.Where.Detail().Between(42, 44)
				.Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void StringDetailWithLike()
		{
			IList<ContentItem> items = finder.Where.Detail("StringDetail").Like("just %").Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void AnyStringDetailWithLike()
		{
			IList<ContentItem> items = finder.Where.Detail().Like("just %").Select();
			Assert.AreEqual(2, items.Count);
			EnumerableAssert.Contains(items, rootItem);
			EnumerableAssert.Contains(items, startPage);
		}

		[Test]
		public void StringDetailWithLike2()
		{
			IList<ContentItem> items = finder.Where.Detail("StringDetail").Like("% string").Select();
			Assert.AreEqual(2, items.Count);
			EnumerableAssert.Contains(items, rootItem);
			EnumerableAssert.Contains(items, startPage);
		}
		[Test]
		public void StringDetailWithLike3()
		{
			IList<ContentItem> items = finder.Where.Detail("StringDetail").Like("% a %").Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void DetailAndDetailWhereBothAreThere()
		{
			IList<ContentItem> items = finder.Where.Detail("StringDetail").Eq("just a string")
				.And.Detail("IntDetail").Eq(43)
				.Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void CantFindDetailAndDetailWhenOnlyOneIsThere()
		{
			IList<ContentItem> items = finder.Where.Detail("StringDetail").Eq("just a string")
				.And.Detail("StringDetail").Eq("min mollgan sträng")
				.Select();
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void CantFindPropertyAndDetailWhenOnlyPropertyIsThere()
		{
			IList<ContentItem> items = finder.Where.Title.Eq("root")
				.And.Detail("StringDetail").Eq("min mollgan sträng")
				.Select();
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void CantFindPropertyAndDetailWhenOnlyDetailIsThere()
		{
			IList<ContentItem> items = finder.Where.Title.Eq("not the root")
				.And.Detail("StringDetail").Eq("just a string")
				.Select();
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void PropertyOrDetailWhenOnlyPropertyIsThere()
		{
			IList<ContentItem> items = finder.Where.Title.Eq("root")
				.Or.Detail("StringDetail").Eq("string that just isn't there")
				.Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void PropertyOrDetailWhenOnlyDetailIsThere()
		{
			IList<ContentItem> items = finder.Where.Title.Eq("not the root")
				.Or.Detail("StringDetail").Eq("just a string")
				.Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void DetailOrDetailWhereOneIsSet()
		{
			IList<ContentItem> items = finder.Where.Detail("StringDetail").Eq("just a string")
				.Or.Detail("StringDetail").Eq("string that just isn't there")
				.Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void DetailOrDetailWhereBothAreSet()
		{
			IList<ContentItem> items = finder.Where.Detail("StringDetail").Eq("just a string")
				.Or.Detail("IntDetail").Eq(43)
				.Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void OnePropertyAndEitherOfTwoProperties()
		{
			IList<ContentItem> items = finder.Where.Name.Eq("root")
				.And.OpenBracket()
					.Title.Eq("root")
					.Or
					.Title.Eq("start page")
				.CloseBracket()
				.Select();

			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void OneDetailAndEitherOfTwoProperties()
		{
			IList<ContentItem> items = finder.Where.Detail("StringDetail").Eq("just a string")
				.And.OpenBracket()
					.Title.Eq("root")
					.Or
					.Title.Eq("start page")
				.CloseBracket()
				.Select();

			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void OnePropertyAndEitherOfTwoDetails()
		{
			IList<ContentItem> items = finder.Where.Name.Eq("root")
				.And.OpenBracket()
					.Detail("IntDetail").Eq(45)
					.Or
					.Detail("IntDetail").Eq(43)
				.CloseBracket()
				.Select();

			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void OneDetailAndEitherOfTwoDetails()
		{
			IList<ContentItem> items = finder.Where.Detail("StringDetail").Eq("just a string")
				.And.OpenBracket()
					.Detail("IntDetail").Eq(45)
					.Or
					.Detail("IntDetail").Eq(43)
				.CloseBracket()
				.Select();

			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void CanSearchForPreviousVersions()
		{
			IList<ContentItem> items = finder.Where
				.Detail().Eq("a string in a version")
				.PreviousVersions(VersionOption.Include)
				.OrderBy.Expires.Desc
				.Select();

			Assert.AreEqual(1, items.Count);
			Assert.AreNotEqual(rootItem, items[0]);
			Assert.AreNotEqual(startPage, items[0]);
		}

		[Test]
		public void All()
		{
			IList<ContentItem> items = finder.All.Select();

			Assert.AreEqual(5, items.Count);
			EnumerableAssert.Contains(items, rootItem);
			EnumerableAssert.Contains(items, startPage);
		}

		[Test]
		public void OrderByPropertyAsc()
		{
			IList<ContentItem> items = finder.All.OrderBy.Title.Asc.Select();

			Assert.AreEqual(5, items.Count);
			Assert.AreEqual(rootItem, items[3]);
			Assert.AreEqual(startPage, items[4]);
		}

		[Test]
		public void OrderByPropertyDesc()
		{
			IList<ContentItem> items = finder.All.OrderBy.Title.Desc.Select();

			Assert.AreEqual(5, items.Count);
			Assert.AreEqual(startPage, items[0]);
			Assert.AreEqual(rootItem, items[1]);
		}

		[Test]
		public void WithCountFilter()
		{
			IList<ContentItem> items = finder.All
				.Filters(new CountFilter(0, 2))
				.OrderBy.ID.Asc.Select();
			
			Assert.AreEqual(2, items.Count);
			Assert.AreEqual(rootItem, items[0]);
			Assert.AreEqual(startPage, items[1]);
		}

		[Test]
		public void FindWithFilterEnumeration()
		{
			IList<ItemFilter> filters = new List<ItemFilter>();
			filters.Add(new ParentFilter(startPage));
			filters.Add(new CountFilter(0, 2));
			IList<ContentItem> items = finder.All
				.Filters(filters)
				.OrderBy.ID.Asc.Select();

			Assert.AreEqual(2, items.Count);
			Assert.AreEqual(item1, items[0]);
			Assert.AreEqual(item2, items[1]);
		}

		[Test]
		public void WithTheZone()
		{
			IList<ContentItem> items = finder.All
				.Filters(new ZoneFilter("TheZone"))
				.OrderBy.ID.Asc.Select();

			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void OrderByDetailDesc()
		{
			IList<ContentItem> items = finder.All.OrderBy.Detail("IntDetail").Desc.Select();

			Assert.AreEqual(5, items.Count);
			Assert.AreEqual(startPage, items[0]);
			Assert.AreEqual(rootItem, items[1]);
		}

		[Test]
		public void FilterCount()
		{
			IList<ContentItem> items = finder.All.OrderBy.Detail("IntDetail").Desc.Select();

			Assert.AreEqual(5, items.Count);
			Assert.AreEqual(startPage, items[0]);
			Assert.AreEqual(rootItem, items[1]);
		}

		[Test]
		public void BoolInDetailCollection()
		{
			IList<ContentItem> items = finder.Where.Detail("DetailCollection").Eq(true).Select();
			Assert.AreEqual(3, items.Count);
		}

		[Test]
		public void IntInDetailCollection()
		{
			IList<ContentItem> items = finder.Where.Detail("DetailCollection").Gt(2000).Select();
			Assert.AreEqual(2, items.Count);
		}

		[Test]
		public void StringInDetailCollection()
		{
			IList<ContentItem> items = finder.Where.Detail("DetailCollection").Like("string in a collection %").Select();
			Assert.AreEqual(3, items.Count);
		}

		[Test]
		public void LinkInDetailCollection()
		{
			IList<ContentItem> items = finder.Where.Detail("DetailCollection").Eq(startPage).Select();
			Assert.AreEqual(3, items.Count);
		}

		[Test]
		public void DateInDetailCollection()
		{
			IList<ContentItem> items = finder.Where.Detail("DetailCollection").Le(new DateTime(2011, 1, 1)).Select();
			Assert.AreEqual(2, items.Count);
		}

		[Test]
		public void AllWithFirstResult()
		{
			IList<ContentItem> items = finder.All.FirstResult(3).OrderBy.ID.Desc.Select();
			Assert.AreEqual(2, items.Count);
			Assert.AreEqual(startPage, items[0]);
			Assert.AreEqual(rootItem, items[1]);
		}

		[Test]
		public void AllWithMaxResults()
		{
			IList<ContentItem> items = finder.All.MaxResults(2).OrderBy.ID.Asc.Select();
			Assert.AreEqual(2, items.Count);
			Assert.AreEqual(rootItem, items[0]);
			Assert.AreEqual(startPage, items[1]);
		}

		[Test]
		public void AllWithFirstAndMaxResults()
		{
			IList<ContentItem> items = finder.All.FirstResult(2).MaxResults(2).OrderBy.ID.Asc.Select();
			Assert.AreEqual(2, items.Count);
			Assert.AreEqual(item1, items[0]);
			Assert.AreEqual(item2, items[1]);
		}

		[Test]
		public void FilterByTypeEqual()
		{
			IList<PersistableItem2> items = finder.Where.Type.Eq(typeof(PersistableItem2)).Select<PersistableItem2>();
			Assert.AreEqual(3, items.Count);
			EnumerableAssert.Contains(items, item1);
			EnumerableAssert.Contains(items, item2);
			EnumerableAssert.Contains(items, item3);
		}

		[Test]
		public void FilterByTypeEqual2()
		{
			IList<PersistableItem1> items = finder.Where.Type.Eq(typeof(PersistableItem1)).Select<PersistableItem1>();
			Assert.AreEqual(2, items.Count);
			EnumerableAssert.Contains(items, rootItem);
			EnumerableAssert.Contains(items, startPage);
		}

		[Test]
		public void FilterByTypeNotEqual()
		{
			IList<ContentItem> items = finder.Where.Type.NotEq(typeof(PersistableItem2)).Select();
			Assert.AreEqual(2, items.Count);
			EnumerableAssert.Contains(items, rootItem);
			EnumerableAssert.Contains(items, startPage);
		}

		[Test]
		public void FilterByTypeAndName()
		{
			IList<PersistableItem2> items = finder
				.Where.Type.Eq(typeof(PersistableItem2))
				.And.Name.Eq("item1")
				.Select<PersistableItem2>();
			Assert.AreEqual(1, items.Count);
			EnumerableAssert.Contains(items, item1);
		}

		[Test]
		public void FindAllItemsIncludingVersions()
		{
			IList<ContentItem> items = finder
				.All.PreviousVersions(VersionOption.Include)
				.Select();
			Assert.AreEqual(6, items.Count);
		}

		[Test]
		public void FindAndSortAllItemsIncludingVersions()
		{
			IList<ContentItem> items = finder
				.All.PreviousVersions(VersionOption.Include)
				.OrderBy.SavedBy.Desc
				.Select();
			Assert.AreEqual(6, items.Count);
		}

		[Test]
		public void CanCountByTypeEqual()
		{
			int count = finder.Where.Type.Eq(typeof (PersistableItem2)).Count();
			Assert.AreEqual(3, count);
		}

		[Test]
		public void CanCountAll()
		{
			int count = finder.All.Count();
			Assert.AreEqual(5, count);
		}

		[Test]
		public void CanCountByPropertyTitle()
		{
			int count = finder.Where.Title.Eq(rootItem.Title).Count();
			Assert.AreEqual(1, count);
		}

		[Test]
		public void CanCountAnyStringDetail()
		{
			int count = finder.Where.Detail().Eq("just a string").Count();
			Assert.AreEqual(2, count);
		}

		[Test]
		public void CanSelectByPropertyNameIn()
		{
			IList<ContentItem> items = finder.Where.Name.In(rootItem.Name, startPage.Name).Select();
			Assert.AreEqual(2, items.Count);
			EnumerableAssert.Contains(items, rootItem);
			EnumerableAssert.Contains(items, startPage);
		}

		[Test]
		public void CanSelectByDetailIn()
		{
			IList<ContentItem> items = finder.Where.Detail().In(43, 45).Select();
			Assert.AreEqual(2, items.Count);
			EnumerableAssert.Contains(items, rootItem);
			EnumerableAssert.Contains(items, startPage);
		}

		[Test]
		public void CanSelectByClassIn()
		{
			IList<ContentItem> items = finder.Where.Type.In(typeof(PersistableItem2), typeof(PersistableItem1)).Select();
			Assert.AreEqual(5, items.Count);
		}
	}
}