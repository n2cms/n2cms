using System.Diagnostics;
using System.Linq;
using N2.Linq;
using NUnit.Framework;

namespace N2.Extensions.Tests.Linq
{
	[TestFixture]
	public class ExpressionQueryableExtensions : LinqTestsBase
	{
		//Expr: value(NHibernate.Linq.Query`1[N2.Extensions.Tests.Linq.LinqItem]).Where(ci => ci.Details.Values.OfType().Any(value(N2.Linq.QueryableExtensions+<>c__DisplayClassc`1[N2.Extensions.Tests.Linq.LinqItem]).
		[Test]
		public void CanSelectItems_WhereDetail_WithStronglyTypedStringProperty_EqualsValue()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.StringProperty2 == "another string");

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}
	}
}
