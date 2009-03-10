using System.Linq;
using NUnit.Framework;
using N2.Tests;

using N2.Linq;

namespace N2.Extensions.Tests.Linq
{
	[TestFixture]
	public class QueriableTests : PersistenceAwareBase
	{
		[TestFixtureSetUp]
		public override void TestFixtureSetUp()
		{
			base.TestFixtureSetUp();
			CreateDatabaseSchema();

			LinqItem root = CreateOneItem<LinqItem>(0, "root", null);
			root.StringProperty = "a string";
			engine.Persister.Save(root);
		}

		[Test, Ignore]
		public void CanSelectAllItems()
		{
			var query = from ci in engine.Database().ContentItems
						select ci;

			EnumerableAssert.Count(1, query);
		}

		[Test, Ignore]
		public void CanSelectAllItems_WithWhere()
		{
			var query = from ci in engine.Database().ContentItems
						where ci.Name == "root"
						select ci;

			EnumerableAssert.Count(1, query);
		}

        [Test, Ignore]
        public void Test()
        {
            var q = engine.Database().ContentItems.Where(x => "root" == (string)x["Name"]);
            Assert.That(q.Count(), Is.EqualTo(1));
        }
	}
}
