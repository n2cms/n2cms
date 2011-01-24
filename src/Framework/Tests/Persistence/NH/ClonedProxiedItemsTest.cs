using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Definitions;
using N2.Persistence.NH;
using N2.Tests.Fakes;
using N2.Persistence.NH.Finder;
using NHibernate.Tool.hbm2ddl;
using N2.Persistence;
using N2.Tests.Persistence.Proxying;
using N2.Persistence.Proxying;

namespace N2.Tests.Persistence.NH
{
	[TestFixture]
	public class ClonedProxiedItemsTest : PersisterTestsBase
	{
		public override void TestFixtureSetup()
		{
			base.persistedTypes = new[] { typeof(Definitions.PersistableItem1), typeof(InterceptableInheritorItem) };
			base.TestFixtureSetup();
		}

		[Test]
		public void CanPersistDetails_OnClonedItem()
		{
			ContentItem item1, item2;
			using (persister)
			{
				item1 = CreateOneItem<Definitions.PersistableItem1>(0, "item", null);
				item1["Hello"] = "World";
				persister.Save(item1);

				item2 = item1.Clone(false);
				persister.Save(item2);
			}
			using (persister)
			{
				item2 = persister.Get(item2.ID);
				Assert.That(item2.ID, Is.Not.EqualTo(item1.ID));
				Assert.That(item2["Hello"], Is.EqualTo("World"));
			}
		}

		[Test]
		public void CanPersistDetails_OnClonedItems_ThatAreProxied()
		{
			InterceptableInheritorItem item1, item2;
			using (persister)
			{
				item1 = proxyFactory.Create(typeof(InterceptableInheritorItem).FullName) as InterceptableInheritorItem;
				item1.Name = "Hello";
				item1.StringProperty = "Howdy";
				item1["Hello"] = "World";
				
				persister.Save(item1);

				item2 = (InterceptableInheritorItem)item1.Clone(false);
				persister.Save(item2);
				var details = item2.Details;
				var sp = details.Get("StringProperty");
				var id = sp.ID;
				var value = sp.Value;
			}
			using (persister)
			{
				item2 = persister.Get<InterceptableInheritorItem>(item2.ID);
				Assert.That(item2.ID, Is.Not.EqualTo(item1.ID));
				Assert.That(item2["Hello"], Is.EqualTo("World"));
				Assert.That(item2.StringProperty, Is.EqualTo("Howdy"));
			}
		}
	}
}
