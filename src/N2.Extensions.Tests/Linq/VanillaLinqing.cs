using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using N2.Linq;
using N2.Details;

namespace N2.Extensions.Tests.Linq
{
	/// <summary>
	/// these tests asserts nhibernate.linq's built-in capabilities
	/// </summary>
	[TestFixture]
	public class VanillaLinqing : LinqTestsBase
	{
		[Test]
		public void CanSelect_Count()
		{
			var query = from ci in engine.QueryItems<LinqItem>()
						select ci;

			int count = query.Count();

			Assert.That(count, Is.EqualTo(2));
		}

		[Test]
		public void CanSelect_AllItems()
		{
			var query = from ci in engine.QueryItems<LinqItem>()
						select ci;

			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(2));
			Assert.That(items.Contains(root));
			Assert.That(items.Contains(item));
		}

		[Test]
		public void CanSelect_Where_Name_Equals()
		{
			var query = from ci in engine.QueryItems<LinqItem>()
						where ci.Name == "root"
						select ci;

			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items[0], Is.EqualTo(root));
		}

		//Expr: value(NHibernate.Linq.Query`1[N2.ContentItem]).Where(ci => ci.Details.Values.OfType().Any(sd => ((sd.Name = "StringProperty2") && (sd.StringValue = "another string"))))
		[Test]
		public void CanSelect_SingleItem_BySubselectingDetail_ByNameAndValue()
		{
			var query = engine.QueryItems()
				.Where(ci => ci.Details.Values.OfType<StringDetail>()
					.Any(sd => sd.Name == "StringProperty2" && sd.StringValue == "another string"));

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}

		//Expr: value(NHibernate.Linq.Query`1[N2.Extensions.Tests.Linq.LinqItem]).Where(ci => ci.Details.Values.OfType().Any(cd => ((cd.Name = "StringProperty2") && (cd.StringValue = "another string"))))
		[Test]
		public void CanSelect_SingleItem_BySubselectingDetail_ByNameAndValue_StronglyTypedQuery()
		{
			var query = engine.QueryItems<LinqItem>()
				.Where(ci => ci.Details.Values.OfType<StringDetail>()
					.Any(cd => cd.Name == "StringProperty2" && cd.StringValue == "another string"));

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}
	}
}
