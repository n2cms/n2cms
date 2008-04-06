using NUnit.Framework;
using N2.Integrity;
using N2.Tests.Persistence.NH;
using N2.Web;

namespace N2.Tests.Persistence.NH
{
	[TestFixture]
	public class PersisterIntegrityTests : PersisterTestsBase
	{
		IUrlParser parser;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			parser = mocks.CreateMock<IUrlParser>();
			mocks.ReplayAll();

			DefaultIntegrityManager integrity = new DefaultIntegrityManager(definitions, parser);
			IntegrityEnforcer enforcer = new IntegrityEnforcer(persister, integrity);
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
				persister.Save(item1);
				persister.Save(item2);
				persister.Save(item1_2);

				ExceptionAssert.Throws<NameOccupiedException>(delegate
				                                              	{
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

		[Test, ExpectedException(typeof(NameOccupiedException))]
		public void CannotCopy_ItemWithName()
		{
			ContentItem root = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			ContentItem item1 = CreateOneItem<Definitions.PersistableItem1>(0, "item1", root);

			using (persister)
			{
				persister.Save(root);
				persister.Save(item1);

				persister.Copy(item1, root);
			}
		}
	}
}
