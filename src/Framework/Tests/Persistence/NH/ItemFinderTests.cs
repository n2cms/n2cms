using System;
using System.Linq;
using System.Collections.Generic;
using N2.Collections;
using N2.Definitions.Static;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using N2.Tests.Persistence.Definitions;
using N2.Web;
using NUnit.Framework;
using Shouldly;

namespace N2.Tests.Persistence.NH
{
	[TestFixture, Category("Integration")]
	public class ItemFinderTests : DatabasePreparingBase
	{
		#region SetUp

		IItemFinder finder;
		ContentItem rootItem;
		ContentItem startPage;
		ContentItem item1;
		ContentItem item2;
		ContentItem item3;
		ContentItem[] all;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			CreateRootItem();
			SaveVersionAndUpdateRootItem();
			CreateStartPageBelow(rootItem);
			item1 = CreatePageBelow(startPage, 1);
			item2 = CreatePageBelow(startPage, 2);
			item3 = CreatePageBelow(startPage, 3);
			all = new[] { rootItem, startPage, item1, item2, item3 };

			engine.Resolve<IHost>().DefaultSite.RootItemID = rootItem.ID;
			engine.Resolve<IHost>().DefaultSite.StartPageID = startPage.ID;

			ISessionProvider sessionProvider = engine.Resolve<ISessionProvider>();
			finder = new ItemFinder(sessionProvider, new DefinitionMap());
		}
		
		#endregion

		[Test]
		public void ByProperty_ID()
		{
			IList<ContentItem> items = finder.Where.ID.Eq(rootItem.ID).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByProperty_Parent()
		{
			IList<ContentItem> items = finder.Where.Parent.Eq(rootItem).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(startPage, items[0]);
		}

		[Test]
		public void ByProperty_Parent_ID()
		{
			IList<ContentItem> items = finder.Where.ParentID.Eq(rootItem.ID).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(startPage, items[0]);
		}

		[Test]
		public void ByProperty_Title()
		{
			IList<ContentItem> items = finder.Where.Title.Eq(rootItem.Title).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByProperty_Name()
		{
			IList<ContentItem> items = finder.Where.Name.Eq(rootItem.Name).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByProperty_ZoneName()
		{
			IList<ContentItem> items = finder.Where.ZoneName.Eq(rootItem.ZoneName).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByProperty_Created()
		{
			IList<ContentItem> items = finder.Where.Created.Eq(rootItem.Created).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByProperty_CreatedBetween()
		{
			IList<ContentItem> items = finder
				.Where.Created.Between(rootItem.Created.AddMinutes(-1), rootItem.Created.AddMinutes(1))
				.Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByProperty_Published()
		{
			IList<ContentItem> items = finder.Where.Published.Eq(rootItem.Published.Value).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByProperty_Expires()
		{
			IList<ContentItem> items = finder.Where.Expires.Eq(rootItem.Expires.Value).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByProperty_SortOrder()
		{
			IList<ContentItem> items = finder.Where.SortOrder.Eq(rootItem.SortOrder).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
        }

        [Test]
        public void ByProperty_VersionIndex_GreaterThan()
        {
            IList<ContentItem> items = finder.Where.VersionIndex.Gt(0).Select();
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(rootItem, items[0]);
        }

        [Test]
        public void ByProperty_VersionIndex_Equals_PreviousVersion()
        {
            IList<ContentItem> items = finder.Where.VersionIndex.Eq(0)
                .And.VersionOf.Eq(rootItem)
                .PreviousVersions(VersionOption.Include).Select();
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(rootItem, items[0].VersionOf.Value);
        }

        [Test]
        public void OrderBy_VersionIndex_Desc()
        {
            IList<ContentItem> items = finder
                .Where.ID.Eq(rootItem.ID)
                .Or.VersionOf.Eq(rootItem)
                .OrderBy.VersionIndex.Desc
                .PreviousVersions(VersionOption.Include).Select();
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(rootItem, items[0]);
            Assert.AreEqual(rootItem, items[1].VersionOf.Value);
        }

        [Test]
        public void OrderBy_VersionIndex_Asc()
        {
            IList<ContentItem> items = finder
                .Where.ID.Eq(rootItem.ID)
                .Or.VersionOf.Eq(rootItem)
                .PreviousVersions(VersionOption.Include)
                .OrderBy.VersionIndex.Asc
                .Select();
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(rootItem, items[0].VersionOf.Value);
            Assert.AreEqual(rootItem, items[1]);
        }

        [Test]
        public void ByProperty_State_EqualsPublished()
        {
            IList<ContentItem> items = finder.Where.State.Eq(ContentState.Published).Select();
            Assert.AreEqual(5, items.Count);
        }

        [Test]
        public void ByProperty_State_EqualsUnpublished()
        {
            IList<ContentItem> items = finder
                .Where.State.Eq(ContentState.Unpublished)
                .PreviousVersions(VersionOption.Include).Select();
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(rootItem, items[0].VersionOf.Value);
        }

        [Test]
        public void OrderBy_State()
        {
            IList<ContentItem> items = finder.All.PreviousVersions(VersionOption.Include)
                .OrderBy.State.Desc.Select();
            Assert.That(items[0].State, Is.GreaterThan(items[items.Count - 1].State));
        }

		[Test]
		public void ByPropertyVisible()
		{
			IList<ContentItem> items = finder.Where.Visible.Eq(rootItem.Visible).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void ByProperties_NameAndTitle()
		{
			IList<ContentItem> items = finder.Where.Name.Eq("root").And.Title.Eq("root").Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(rootItem, items[0]);
		}

		[Test]
		public void TwoItems_ByNameOrTitle()
		{
			IList<ContentItem> items = finder.Where.Name.Eq("root").Or.Title.Eq("start page").Select();
			Assert.AreEqual(2, items.Count);
			EnumerableAssert.Contains(items, rootItem);
			EnumerableAssert.Contains(items, startPage);
		}

		[Test]
		public void ByProperty_VersionOf()
		{
			IList<ContentItem> items = finder.Where.VersionOf.Eq(rootItem).Select();
			Assert.AreEqual(1, items.Count);
			Assert.AreNotEqual(rootItem, items[0]);
		}

		[Test]
		public void ByProperty_VersionOf_Null()
		{
			IList<ContentItem> items = finder.Where.VersionOf.Eq(null).Select();
			Assert.AreEqual(5, items.Count);
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
		public void WithFilterAndMaxResults()
		{
			IList<ContentItem> items = finder.All
				.Filters(new CountFilter(1, 2))
				.MaxResults(2)
				.OrderBy.ID.Asc.Select();

			Assert.AreEqual(2, items.Count);
			Assert.AreEqual(startPage, items[0]);
			Assert.AreEqual(item1, items[1]);
		}

		[Test]
		public void WithFilter_AndFirstResult()
		{
			IList<ContentItem> items = finder.All
				.Filters(new CountFilter(1, 2))
				.FirstResult(1)
				.OrderBy.ID.Asc.Select();

			Assert.AreEqual(2, items.Count);
			Assert.AreEqual(item1, items[0]);
			Assert.AreEqual(item2, items[1]);
		}

		[Test]
		public void WithFilter_AndFirstResult_AndMaxResults()
		{
			IList<ContentItem> items = finder.All
				.Filters(new CountFilter(1, 2))
				.FirstResult(2)
				.MaxResults(2)
				.OrderBy.ID.Asc.Select();

			Assert.AreEqual(2, items.Count);
			Assert.AreEqual(item2, items[0]);
			Assert.AreEqual(item3, items[1]);
		}

		[Test]
		public void WithFilterAndHugeMaxResults()
		{
			IList<ContentItem> items = finder.All
				.Filters(new CountFilter(1, 2))
				.MaxResults(200)
				.OrderBy.ID.Asc.Select();

			Assert.AreEqual(2, items.Count);
		}

		[Test]
		public void WithNoneFilteredAndMaxResults()
		{
			IList<ContentItem> items = finder.All
				.Filters(new NullFilter())
				.MaxResults(2)
				.OrderBy.ID.Asc.Select();

			Assert.AreEqual(2, items.Count);
		}

		[Test]
		public void WithNoneFilteredAndHugeMaxResults()
		{
			IList<ContentItem> items = finder.All
				.Filters(new NullFilter())
				.MaxResults(200)
				.OrderBy.ID.Asc.Select();

			Assert.AreEqual(5, items.Count);
		}

		[Test]
		public void FindWithFilterEnumeration()
		{
			IList<ItemFilter> filters = new List<ItemFilter>();
			filters.Add(new AncestorFilter(startPage));
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
		public void FindCertainProperties()
		{
			var items = finder
				.Where.Type.Eq(typeof(PersistableItem2))
				.And.Name.Eq("item1")
				.Select("Title", "Name");

			items.Single()["Title"].ShouldBe("item1");
			items.Single()["Name"].ShouldBe("item1");
		}

		//[Test]
		//public void FindCertainPropertiesWithSelector()
		//{
		//    var items = finder
		//        .Where.Type.Eq(typeof(PersistableItem2))
		//        .And.Name.Eq("item1")
		//        .Select(row => new { Title = row["Title"], Name = row["Name"] });

		//    items.Single().Title.ShouldBe("item1");
		//    items.Single().Name.ShouldBe("item1");
		//}

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

		[Test]
		public void CanSelect_WithIn_AndNamedDetail_21454()
		{
			IList<ContentItem> items = finder.Where.Detail("IntDetail").In(43, 45, 47).Select();

			Assert.That(items.Count, Is.EqualTo(2));
		}

		[Test]
		public void CanSelect_WithIn_AndNamedDetailCollection_21454()
		{
			IList<ContentItem> items = finder.Where.Detail("DetailCollection").In(555, 1555, 2555, 3555).Select();

			Assert.That(items.Count, Is.EqualTo(3));
		}

		[Test]
		public void CanSelect_WhenNoItems_InResultSet()
		{
			IList<ContentItem> items = finder.Where.Detail("DetailCollection").Eq(12345).Select();

			Assert.That(items.Count, Is.EqualTo(0));
		}

		[Test]
		public void CanSelect_All_WithMaxRestriction()
		{
			IList<ContentItem> items = finder.All.MaxResults(1).Select();

			Assert.That(items.Count, Is.EqualTo(1));
		}

		[Test]
		public void CanSelect_AllItems_ByAncestralTrail()
		{
			IList<ContentItem> items = finder.Where.AncestralTrail.Like(rootItem.AncestralTrail + "%").Select();
			int allCount = finder.All.Count();

			Assert.That(items.Count, Is.EqualTo(allCount));
		}

		[Test]
		public void CanSelect_Descendantds_ByAncestralTrail()
		{
			IList<ContentItem> items = finder.Where.AncestralTrail.Like(startPage.AncestralTrail + "%").Select();
			int allCount = finder.All.Count();

			Assert.That(items.Count, Is.EqualTo(allCount - 1));
		}

		[Test]
		public void CanSelect_Siblings_ByAncestralTrail()
		{
			IList<ContentItem> items = finder.Where.AncestralTrail.Like(item1.AncestralTrail).Select();

			Assert.That(items.Count, Is.EqualTo(startPage.Children.Count));
		}

		[Test]
		public void Throws_RelevantException_WhenPassed_UnknownType()
		{
			ExceptionAssert.Throws<ArgumentException>(delegate
				{
					finder.Where.Type.Eq(typeof (int)).Select();
				});
		}

		[Test]
		public void FilterByProperty_Boolean_Equals_MatchingValue()
		{
			var items = finder.Where.Property("BoolPersistableProperty").Eq(true).Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}
		[Test]
		public void FilterByProperty_Boolean_Equals_NonMatchingValue()
		{
			var items = finder.Where.Property("BoolPersistableProperty").Eq(false).Select();
			Assert.That(items.Count, Is.EqualTo(0));
		}
		[Test]
		public void FilterByProperty_Boolean_In_MatchingValue()
		{
			var items = finder.Where.Property("BoolPersistableProperty").In(true, false).Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}

		[Test]
		public void FilterByProperty_Double_Equals_MatchingValue()
		{
			var items = finder.Where.Property("DoublePersistableProperty").Eq(555.555).Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}
		[Test]
		public void FilterByProperty_Double_GreaterThan_MatchingValue()
		{
			var items = finder.Where.Property("DoublePersistableProperty").Gt(444.444).Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}
		[Test]
		public void FilterByProperty_Double_Equals_NonMatchingValue()
		{
			var items = finder.Where.Property("DoublePersistableProperty").Eq(666.666).Select();
			Assert.That(items.Count, Is.EqualTo(0));
		}
		[Test]
		public void FilterByProperty_Double_LessThan_NonMatchingValue()
		{
			var items = finder.Where.Property("DoublePersistableProperty").Lt(444.444).Select();
			Assert.That(items.Count, Is.EqualTo(0));
		}
		[Test]
		public void FilterByProperty_Double_In_MatchingValue()
		{
			var items = finder.Where.Property("DoublePersistableProperty").In(444.444, 555.555).Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}

		[Test]
		public void FilterByProperty_Integer_Equals_MatchingValue()
		{
			var items = finder.Where.Property("IntPersistableProperty").Eq(555).Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}
		[Test]
		public void FilterByProperty_Integer_LessThanOrEquals_MatchingValue()
		{
			var items = finder.Where.Property("IntPersistableProperty").Le(555).Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}
		[Test]
		public void FilterByProperty_Integer_Equals_NonMatchingValue()
		{
			var items = finder.Where.Property("IntPersistableProperty").Eq(666).Select();
			Assert.That(items.Count, Is.EqualTo(0));
		}
		[Test]
		public void FilterByProperty_Integer_GreaterThanOrEquals_NonMatchingValue()
		{
			var items = finder.Where.Property("IntPersistableProperty").Ge(556).Select();
			Assert.That(items.Count, Is.EqualTo(0));
		}
		[Test]
		public void FilterByProperty_Integer_In_MatchingValue()
		{
			var items = finder.Where.Property("IntPersistableProperty").In(555, 666).Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}

		[Test]
		public void FilterByProperty_String_Equals_MatchingValue()
		{
			var items = finder.Where.Property("StringPersistableProperty").Eq("in table text").Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}
		[Test]
		public void FilterByProperty_String_NotEquals_MatchingValue()
		{
			var items = finder.Where.Property("StringPersistableProperty").NotEq("in table text").Select();
			Assert.That(items.Count, Is.EqualTo(0));
		}
		[Test]
		public void FilterByProperty_String_Like_MatchingValue()
		{
			var items = finder.Where.Property("StringPersistableProperty").Like("in table%").Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}
		[Test]
		public void FilterByProperty_String_NotLike_MatchingValue()
		{
			var items = finder.Where.Property("StringPersistableProperty").NotLike("in table%").Select();
			Assert.That(items.Count, Is.EqualTo(0));
		}
		[Test]
		public void FilterByProperty_String_Equals_NonMatchingValue()
		{
			var items = finder.Where.Property("StringPersistableProperty").Eq("non in table").Select();
			Assert.That(items.Count, Is.EqualTo(0));
		}
		[Test]
		public void FilterByProperty_String_NotEquals_NonMatchingValue()
		{
			var items = finder.Where.Property("StringPersistableProperty").NotEq("non in table").Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}
		[Test]
		public void FilterByProperty_String_Like_NonMatchingValue()
		{
			var items = finder.Where.Property("StringPersistableProperty").Like("non in table%").Select();
			var all = new List<ContentItem>(finder.All.Select());
			Assert.That(items.Count, Is.EqualTo(0));
		}
		[Test]
		public void FilterByProperty_String_NotLike_NonMatchingValue()
		{
			var items = finder.Where.Property("StringPersistableProperty").NotLike("non in table%").Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}
		[Test]
		public void FilterByProperty_String_In_MatchingValue()
		{
			var items = finder.Where.Property("StringPersistableProperty").In("in table text", "not in table").Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}

		[Test]
		public void FilterByProperty_DateTime_Equals_MatchingValue()
		{
			var items = finder.Where.Property("DateTimePersistableProperty").Eq(new DateTime(2010, 06, 18, 14, 30, 00)).Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}
		[Test]
		public void FilterByProperty_DateTime_Equals_NonMatchingValue()
		{
			var items = finder.Where.Property("DateTimePersistableProperty").Eq(DateTime.Now).Select();
			Assert.That(items.Count, Is.EqualTo(0));
		}
		[Test]
		public void FilterByProperty_DateTime_GreaterThan_MatchingValue()
		{
			var items = finder.Where.Property("DateTimePersistableProperty").Gt(new DateTime(2010, 06, 18, 14, 29, 50)).Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}
		[Test]
		public void FilterByProperty_DateTime_GreaterThan_NonMatchingValue()
		{
			var items = finder.Where.Property("DateTimePersistableProperty").Gt(new DateTime(2010, 06, 18, 14, 30, 10)).Select();
			Assert.That(items.Count, Is.EqualTo(0));
		}
		[Test]
		public void FilterByProperty_DateTime_In_MatchingValue()
		{
			var items = finder.Where.Property("DateTimePersistableProperty").In(new DateTime(2010, 06, 18, 14, 30, 00), DateTime.Now).Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}

		[Test]
		public void FilterByProperty_Link_Equals_MatchingValue()
		{
			var items = finder.Where.Property("LinkPersistableProperty").Eq(rootItem).Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}
		[Test]
		public void FilterByProperty_Link_Equals_NonMatchingValue()
		{
			var items = finder.Where.Property("LinkPersistableProperty").Eq(startPage).Select();
			Assert.That(items.Count, Is.EqualTo(0));
		}
		[Test]
		public void FilterByProperty_Link_In_NonMatchingValue()
		{
			var items = finder.Where.Property("LinkPersistableProperty").In(rootItem, startPage).Select();
			Assert.That(items.Count, Is.EqualTo(3));
		}

		[Test]
		public void Property_Null()
		{
			var items = finder.Where.ZoneName.IsNull(true).Select();
			Assert.That(items.Count, Is.EqualTo(all.Where(i => i.ZoneName == null).Count()));
		}

		[Test]
		public void Property_NotNull()
		{
			var items = finder.Where.ZoneName.IsNull(false).Select();
			Assert.That(items.Count, Is.EqualTo(all.Where(i => i.ZoneName != null).Count()));
		}

		[Test]
		public void PersistableProperty_Null()
		{
			var items = finder.Where.Property("IntPersistableProperty").Null(true).Select();
			Assert.That(items.Count, Is.EqualTo(all.Where(i => i["IntPersistableProperty"] == null).Count()));
		}

		[Test]
		public void PersistableProperty_NotNull()
		{
			var items = finder.Where.Property("IntPersistableProperty").Null(false).Select();
			Assert.That(items.Count, Is.EqualTo(all.Where(i => i["IntPersistableProperty"] != null).Count()));
		}

		[Test]
		public void Detail_Null_ExistingDetail()
		{
			var items = finder.Where.Detail("IntDetail").Null<int>(true).Select();
			Assert.That(items.Count, Is.EqualTo(all.Where(i => i["IntDetail"] == null).Count()));
		}

		[Test]
		public void Detail_NotNull_ExistingDetail()
		{
			var items = finder.Where.Detail("IntDetail").Null<int>(false).Select();
			Assert.That(items.Count, Is.EqualTo(all.Where(i => i["IntDetail"] != null).Count()));
		}

		[Test]
		public void Detail_Null_NonExistingDetail()
		{
			var items = finder.Where.Detail("IntDetailXYZ").Null<int>(true).Select();
			Assert.That(items.Count, Is.EqualTo(all.Length));
		}

		[Test]
		public void Detail_NotNull_NonExistingDetail()
		{
			var items = finder.Where.Detail("IntDetailXYZ").Null<int>(false).Select();
			Assert.That(items.Count, Is.EqualTo(0));
		}

		#region Helpers
		
		private ContentItem CreatePageBelow(ContentItem parentPage, int index)
		{
			var item = CreateOneItem<PersistableItem2>(0, "item" + index, parentPage);
            item.State = ContentState.Published;
            
			N2.Details.DetailCollection details = item.GetDetailCollection("DetailCollection", true);
			details.Add(true);
			details.Add(index * 1000 + 555);
			details.Add(index * 1000.0 + 555.55);
			details.Add("string in a collection " + index);
			details.Add(parentPage);
			details.Add(new DateTime(2009 + index, 1, 1));

			item.BoolPersistableProperty = true;
			item.DateTimePersistableProperty = new DateTime(2010, 06, 18, 14, 30, 00);
			item.DoublePersistableProperty = 555.555;
			item.IntPersistableProperty = 555;
			item.LinkPersistableProperty = rootItem;
			item.StringPersistableProperty = "in table text";

			engine.Persister.Save(item);
			return item;
		}

		private void CreateStartPageBelow(ContentItem root)
		{
			startPage = CreateOneItem<PersistableItem1>(0, "start page", root);
			startPage.ZoneName = "AZone";
			startPage.SortOrder = 34;
			startPage.Visible = true;
            startPage.State = ContentState.Published;
            startPage["IntDetail"] = 45;
			startPage["DoubleDetail"] = 56.66;
			startPage["BoolDetail"] = true;
			startPage["DateDetail"] = new DateTime(2000, 01, 01);
			startPage["StringDetail"] = "actually another string";
			startPage["StringDetail2"] = "just a string";
			startPage["ObjectDetail"] = new string[] { "two", "three", "four" };
			startPage["ItemDetail"] = root;

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
            rootItem.VersionIndex = 1;

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
            rootItem.State = ContentState.Published;
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
	}
}