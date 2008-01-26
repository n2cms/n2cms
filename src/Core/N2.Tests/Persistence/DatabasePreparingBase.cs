using System;
using System.Collections.Generic;
using System.Text;

using N2;
using MbUnit.Framework;

namespace N2.Tests.Persistence
{
	public class DatabasePreparingBase : PersistenceAwareBase
	{
		[TestFixtureSetUp]
		public override void TestFixtureSetUp()
		{
			base.TestFixtureSetUp();
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			base.CreateDatabaseSchema();
		}

		protected Definitions.PersistableItem1 CreateAndSaveItem(string name, string title, ContentItem parent)
		{
			Definitions.PersistableItem1 item = CreateOneItem<Definitions.PersistableItem1>(0, "nada", parent);
			item.Title = title;
			item.Name = name;
			engine.Persister.Save(item);

			return item;
		}

		protected Definitions.PersistableItem1 CreateRoot(string name, string title)
		{
			Definitions.PersistableItem1 item = CreateAndSaveItem(name, title, null);
			return item;
		}
	}
}
