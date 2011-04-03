using NUnit.Framework;
using N2.Integrity;
using N2.Web;
using N2.Tests.Fakes;
using System.Linq;
using System;
using N2.Definitions;
using Rhino.Mocks;

namespace N2.Tests.Persistence.NH
{
	[TestFixture]
	public class PersisterIntegrityTests : PersisterTestsBase
	{
		IUrlParser parser;
		new FakeItemFinder finder;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			parser = mocks.StrictMock<IUrlParser>();
			mocks.ReplayAll();

			finder = new FakeItemFinder(definitions, () => Enumerable.Empty<ContentItem>());

			IntegrityManager integrity = new IntegrityManager(definitions, finder, parser);
			IntegrityEnforcer enforcer = new IntegrityEnforcer(persister, integrity, activator);
			enforcer.Start();
		}

		[Test]
		public void CannotCopy_WhenNameIsOccupied()
		{
			ContentItem root = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			ContentItem item1 = CreateOneItem<Definitions.PersistableItem1>(0, "item1", root);
			ContentItem item1_2 = CreateOneItem<Definitions.PersistableItem1>(0, "item2", item1);
			ContentItem item2 = CreateOneItem<Definitions.PersistableItem1>(0, "item2", root);

			using (persister)
			{
				persister.Save(root);
				finder.Selector = () => root.Children.Where(c => c.Name.Equals("item1", StringComparison.InvariantCultureIgnoreCase));
				persister.Save(item1);
				finder.Selector = () => root.Children.Where(c => c.Name.Equals("item2", StringComparison.InvariantCultureIgnoreCase));
				persister.Save(item2);
				finder.Selector = () => item1.Children.Where(c => c.Name.Equals("item2", StringComparison.InvariantCultureIgnoreCase));
				persister.Save(item1_2);

				ExceptionAssert.Throws<NameOccupiedException>(delegate
				                                              	{
																	finder.Selector = () => item1.Children.Where(c => c.Name.Equals("item2", StringComparison.InvariantCultureIgnoreCase));
																	persister.Copy(item2, item1);
				                                              	});
				Assert.AreEqual(1, item1.Children.Count);
			}
		}

		[Test]
		public void CanCopy_ToSameLocation_ItemWithNoName()
		{
			ContentItem root = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			ContentItem item1 = CreateOneItem<Definitions.PersistableItem1>(0, null, root);

			using (persister)
			{
				persister.Save(root);
				persister.Save(item1);

				ContentItem item2 = persister.Copy(item1, root);
				Assert.AreNotSame(item1, item2);
				Assert.AreNotEqual(item1.Name, item2.Name);
				Assert.AreEqual(2, root.Children.Count);
			}
		}

		[Test]
		public void CannotCopy_ItemWithName()
		{
			ContentItem root = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			finder.Selector = () => root.Children.Where(c => c.Name.Equals("item1", StringComparison.InvariantCultureIgnoreCase));
			ContentItem item1 = CreateOneItem<Definitions.PersistableItem1>(0, "item1", root);

			using (persister)
			{
				persister.Save(root);
				persister.Save(item1);

				Assert.Throws<NameOccupiedException>(() => persister.Copy(item1, root));
			}
		}
	}
}
