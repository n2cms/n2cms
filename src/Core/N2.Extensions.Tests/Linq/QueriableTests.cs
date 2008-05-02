using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using N2.Engine;
using N2.Persistence.Finder;
using N2.Tests;

namespace N2.Extensions.Tests.Linq
{
	[TestFixture]
	public class QueriableTests : N2.Tests.ItemTestsBase
	{
		IEngine engine;
		IItemFinder finder;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			//engine = new CmsEngine();
			//finder = engine.Resolve<IItemFinder>();

			LinqItem root = CreateOneItem<LinqItem>(0, "root", null);
			engine.Persister.Save(root);
		}

		//[Test]
		//public void CanSelectAllItems()
		//{
		//    var query = from ci in finder
		//                select ci;

		//    EnumerableAssert.Count(1, query);
		//}

		//[Test]
		//public void CanSelectAllItems_WithWhere()
		//{
		//    var query = from ci in finder
		//                where ci.Name == "root"
		//                select ci;

		//    EnumerableAssert.Count(1, query);
		//}
	}
}
