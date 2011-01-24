using System.Linq;
using N2.Linq;
using NUnit.Framework;

namespace N2.Extensions.Tests.Linq
{
	[TestFixture]
	public class SimpleQueryableExtensions : LinqTestsBase
	{
		[Test]
		public void CanSelect_SingleItem_BySubselectingDetail_ViaExtension()
		{
			var query = engine.QueryItems()
				.WhereDetailEquals("StringProperty2", "another string");

			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}

		[Test]
		public void CanSelect_SingleItem_BySubselectingAnyDetail_ViaExtension()
		{
			var query = engine.QueryItems()
				.WhereDetailEquals("yet another string");

			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(!items.Contains(root));
			Assert.That(items.Contains(item));
		}

	}
}
