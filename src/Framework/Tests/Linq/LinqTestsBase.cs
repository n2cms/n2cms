using System;
using System.Diagnostics;
using N2.Tests;
using NUnit.Framework;

namespace N2.Extensions.Tests.Linq
{
	public abstract class LinqTestsBase : PersistenceAwareBase
	{
		protected LinqItem root;
		protected LinqItem item;
		protected DateTime now;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			CreateDatabaseSchema();

			root = CreateOneItem<LinqItem>(0, "root", null);
			root.StringProperty = "a string";
			root.StringProperty2 = "another string";
			root.IntProperty = 123;
			root.DateTimeProperty = now = DateTime.Now;
			root.DoubleProperty = 345.678;
			root.BooleanProperty = true;

			item = CreateOneItem<LinqItem>(0, "item", null);
			item.StringProperty = "a string";
			item.StringProperty2 = "yet another string";
			item.IntProperty = 234;
			item.DateTimeProperty = now.AddDays(-1);
			item.DoubleProperty = 123456789;
			item.BooleanProperty = false;
			item.ContentItemProperty = root;
			item.AddTo(root);

			engine.Persister.Repository.Save(root);
			engine.Persister.Repository.Save(item);

			Debug.WriteLine("===== Starting Test =====");
		}
	}
}
