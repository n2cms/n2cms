using System.Diagnostics;
using N2.Tests;
using NUnit.Framework;

namespace N2.Extensions.Tests.Linq
{
	public abstract class LinqTestsBase : PersistenceAwareBase
	{
		protected LinqItem root;
		protected LinqItem item;

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
	}
}
