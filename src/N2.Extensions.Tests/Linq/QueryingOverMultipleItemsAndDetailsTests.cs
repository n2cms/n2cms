using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

using NUnit.Framework;
using N2.Tests;

using N2.Linq;
using N2.Details;

namespace N2.Extensions.Tests.Linq
{
	[TestFixture]
	public class QueryingOverMultipleItemsAndDetailsTests : PersistenceAwareBase
	{
		LinqItem root;
		LinqItem item;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			CreateDatabaseSchema();

			root = CreateOneItem<LinqItem>(0, "root", null);
			root.StringProperty = "a string";
			root.StringProperty2 = "another string";
			
			item = CreateOneItem<LinqItem>(0, "item", null);
			item.StringProperty = "a string";
			item.StringProperty2 = "yet another string";

			engine.Persister.Repository.Save(root);
			engine.Persister.Repository.Save(item);

			Debug.WriteLine("===== Starting Test =====");
		}

		[Test]
		public void CanSelect_MultipleItems_ByDetail_NameAndValue()
		{
			var query = engine.ContentItems<LinqItem>()
				.Where(ci => 
					ci.Details.Values
					.Where(cd => cd.Name == "StringProperty")
					.OfType<StringDetail>()
					.Where(sd => sd.StringValue == "a string")
					.Any());

			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(2));
			Assert.That(items.Contains(root));
			Assert.That(items.Contains(item));
		}

		[Test, Ignore("This confuses NH")]
		public void CanSelect_SingleItem_ByDetail_NameAndValue()
		{
			var query = engine.ContentItems()
				.Where(ci => ci.Details.Values
					.Where(cd => cd.Name == "StringProperty2")
					.OfType<StringDetail>()
					.Where(sd => sd.StringValue == "another string")
					.Any());

			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}
//SELECT this_.ID AS ID2_1_, this_.Created AS Created2_1_, this_.Published AS Published2_1_, this_.Updated AS Updated2_1_, this_.Expires AS Expires2_1_, this_.Name AS Name2_1_, this_.ZoneName AS ZoneName2_1_, this_.Title AS Title2_1_, this_.SortOrder AS SortOrder2_1_, this_.Visible AS Visible2_1_, this_.SavedBy AS SavedBy2_1_, this_.VersionOfID AS Version13_2_1_, this_.ParentID AS ParentID2_1_, this_.TYPE AS Type2_1_, authorized2_.ItemID AS ItemID3_, authorized2_.ID AS ID3_, authorized2_.ID AS ID0_0_, authorized2_.ItemID AS ItemID0_0_, authorized2_.Role AS Role0_0_
//    FROM n2Item this_ LEFT OUTER JOIN n2AllowedRole authorized2_ ON this_.ID=authorized2_.ItemID
//    WHERE this_.ID IN (
//    SELECT this_0_.ID AS y0_
//        FROM n2Item this_0_
//        WHERE EXISTS(
//        SELECT 1
//            FROM n2Detail
//            WHERE this_0_.ID=ItemID
//                AND ((n2Detail.DetailCollectionID IS NULL)) ))
               
	}
}
