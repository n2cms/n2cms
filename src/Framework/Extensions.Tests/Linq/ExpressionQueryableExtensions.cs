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
		public void CanSelectItems_Where_DetailBackingProperty_Equals_StringConstant()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.StringProperty2 == "another string");

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_DetailBackingProperty_Equals_StringConstant_DoesntSelect_FromOtherwiseNamedDetails()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.StringProperty2 == "a string");

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(0));
		}

		[Test]
		public void CanSelectItems_Where_DetailBackingProperty_StartsWith_StringConstant()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.StringProperty2.StartsWith("another"));

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_DetailBackingProperty_EndsWith_StringConstant()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.StringProperty2.EndsWith("string"));

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(2));
		}

		[Test]
		public void CanSelectItems_Where_DetailBackingProperty_Contains_StringConstant()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.StringProperty2.Contains("another"));

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(2));
		}

		[Test]
		public void CanSelectItems_Where_StringConstant_Equals_DetailBackingProperty()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => "a string" == ci.StringProperty);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(2));
			Assert.That(items.Contains(root));
			Assert.That(items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_DetailBackingProperty_Equals_IntegerConstant()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.IntProperty == 123);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_DetailBackingProperty_LessThan_IntegerConstant()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.IntProperty < 234);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_DetailBackingProperty_LessThanOrEquals_IntegerConstant()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.IntProperty <= 234);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(2));
			Assert.That(items.Contains(root));
			Assert.That(items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_IntegerConstant_GreaterThan_DetailBackingProperty()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => 234 > ci.IntProperty);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_DetailBackingProperty_GreaterThan_IntegerConstant()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.IntProperty > 123);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(!items.Contains(root));
			Assert.That(items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_DetailBackingProperty_GreaterThanOrEquals_IntegerConstant()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.IntProperty >= 123);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(2));
			Assert.That(items.Contains(root));
			Assert.That(items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_DetailBackingProperty_Equals_DateTimeField()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.DateTimeProperty == now);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_DetailBackingProperty_Equals_DoubleConstant()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.DoubleProperty == 345.678);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}

		[Test]
		[Ignore("This is mighty strange")]
		public void CanSelectItems_Where_DetailBackingProperty_Equals_BooleanConstant()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.BooleanProperty == true);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_DetailBackingProperty_Equals_FalseBooleanConstant()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.BooleanProperty == false);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(item));
			Assert.That(!items.Contains(root));
		}

		[Test]
		[Ignore("Not supported (as of now)")]
		public void CanSelectItems_Where_DetailBackingBoolean()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.BooleanProperty);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_CombinedExpressions_StringEquals_And_IntEquals()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.StringProperty == "a string" && ci.IntProperty == 123);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_CombinedExpressions_StringEquals_And_IntGreaterThan()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.StringProperty == "a string" && ci.IntProperty > 200);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(!items.Contains(root));
			Assert.That(items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_CombinedExpressions_StringEquals_Or_IntEquals()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.StringProperty == "no string" || ci.IntProperty == 123);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}



		[Test, Ignore("TODO")]
		public void CanSelectItems_Where_BooleanDetailBackingProperty()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.BooleanProperty);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_DetailBackingProperty_Equals_ContentItemField()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.ContentItemProperty == root);

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(!items.Contains(root));
			Assert.That(items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_MultipleTimes()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.StringProperty == "a string")
				.WhereDetail(ci => ci.StringProperty2 == "another string");

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(items.Contains(root));
			Assert.That(!items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_OneString_Equals_And_AnotherString_Contains()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.StringProperty == "a string" && ci.StringProperty2.Contains("yet"));

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(1));
			Assert.That(!items.Contains(root));
			Assert.That(items.Contains(item));
		}

		[Test]
		public void CanSelectItems_Where_MultipleTimes_ExpectingNegativeResult()
		{
			var query = engine.QueryItems<LinqItem>()
				.WhereDetail(ci => ci.StringProperty2 == "another string") // only on root
				.WhereDetail(ci => ci.IntProperty == 234); // only on itme

			Debug.WriteLine(query.Expression);
			var items = query.ToList();

			Assert.That(items.Count, Is.EqualTo(0));
		}
	}
}
