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
	public class QueryingOverSingleItemTests : PersistenceAwareBase
	{
		LinqItem root;


		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			CreateDatabaseSchema();

			root = CreateOneItem<LinqItem>(0, "root", null);
			root.StringProperty = "a string";

			engine.Persister.Repository.Save(root);

			Debug.WriteLine("===== Starting Test =====");
		}



		[Test]
		public void CanSelect_AllItems()
		{
			var query = from ci in engine.ContentItems<LinqItem>()
						select ci;

			var items = query.ToList();
			
			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items[0], Is.EqualTo(root));
		}

		[Test]
		public void CanSelect_Count()
		{
			var query = from ci in engine.ContentItems<LinqItem>()
						select ci;

			int count = query.Count();

			Assert.That(count, Is.EqualTo(1));
		}

		[Test]
		public void CanSelect_Where_Name_Equals()
		{
			var query = from ci in engine.ContentItems<LinqItem>()
						where ci.Name == "root"
						select ci;

			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items[0], Is.EqualTo(root));
		}

		[Test]
		public void CanSelect_Details_ByNameAndValue()
		{
			var query = engine.ContentDetails<StringDetail>()
				.Where(cd => cd.Name == "StringProperty")
				.Where(cd => cd.StringValue == "a string");

			var details = query.ToList();

			Assert.That(details.Count, Is.EqualTo(1));
			Assert.That(details[0].EnclosingItem, Is.EqualTo(root));
		}

		[Test, Ignore]
		public void CanSelect_Items_ByNameAndValue_ViaDetail()
		{
			var query = engine.ContentDetails<StringDetail>()
				.Where(cd => cd.Name == "StringProperty")
				.Where(cd => cd.StringValue == "a string")
				.Select(cd => cd.EnclosingItem);

			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items[0], Is.EqualTo(root));
		}

		[Test]
		public void CanSelect_Items_ByName_ViaDetailCollection()
		{
			var query = engine.ContentItems<LinqItem>()
				.Where(ci => ci.Details.Values
					.Where(cd => cd.Name == "StringProperty").Any());

			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items[0], Is.EqualTo(root));
		}

		[Test, Ignore("Cannot use subqueries on a criteria without a projection")]
		public void CanSelect_Items_ByName_ViaIndexer()
		{
			var query = engine.ContentItems<LinqItem>()
				.Where(ci => ci.Details["StringProperty"] != null);

			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items[0], Is.EqualTo(root));
		}

		[Test]
		public void CanSelect_Items_ByNameAndValue_ViaDetailValues()
		{
			var query = engine.ContentItems<LinqItem>()
				.Where(ci => ci.Details.Values
					.Where(cd => cd.Name == "StringProperty")
					.OfType<StringDetail>()
					.Where(sd => sd.StringValue == "a string").Any());

			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items[0], Is.EqualTo(root));
		}
	}
}
