using System;
using MbUnit.Framework;
using N2.Definitions;
using N2.Persistence;
using N2.Web;
using Rhino.Mocks;

namespace N2.Trashcan.Tests
{
	[TestFixture]
	public class TrashHandlerTests : TrashTestBase
	{
		[Test]
		public void ThrownItem_IsMovedToTrashcan()
		{
			IDefinitionManager definitions = mocks.CreateMock<IDefinitionManager>();

			IPersister persister = mocks.CreateMock<IPersister>();
			Expect.Call(persister.Get(1)).Return(root);
			Expect.Call(delegate { persister.Save(item); });
			
			Site site = new Site(1);
			
			mocks.ReplayAll();
			
			TrashHandler th = new TrashHandler(persister, definitions, site);
			th.Throw(item);

			Assert.AreEqual(trash, item.Parent);

			mocks.VerifyAll();
		}

		[Test]
		public void ThrownItem_IsExpired()
		{
			TrashHandler th = CreateTrashHandler();
			th.Throw(item);

			Assert.Less(DateTime.Now.AddSeconds(-10), item.Expires);
		}

		[Test]
		public void ThrownItem_NameIsCleared()
		{
			TrashHandler th = CreateTrashHandler();
			th.Throw(item);

			Assert.AreNotEqual("item", item.Name);
		}

		[Test]
		public void ThrownItem_OldValuesAreStoredInDetailBag()
		{
			TrashHandler th = CreateTrashHandler();
			th.Throw(item);

			Assert.AreEqual("item", item[TrashHandler.FormerName]);
			Assert.AreEqual(root, item[TrashHandler.FormerParent]);
			Assert.IsNull(item[TrashHandler.FormerExpires]);
			Assert.Less(DateTime.Now.AddSeconds(-10), (DateTime)item[TrashHandler.DeletedDate]);
		}

		#region Helper methods

		private TrashHandler CreateTrashHandler()
		{
			IDefinitionManager definitions = MockDefinitions();
			IPersister persister = MockPersister(root, trash, item);
			Site site = new Site(1);

			mocks.ReplayAll();

			return new TrashHandler(persister, definitions, site);
		}

		private IPersister MockPersister(ContentItem root, ContentItem trash, ContentItem item)
		{
			IPersister persister = mocks.CreateMock<IPersister>();
			Expect.Call(persister.Get(1)).Return(root);
			Expect.Call(delegate { persister.Save(item); });
			return persister;
		}

		private IDefinitionManager MockDefinitions()
		{
			return mocks.CreateMock<IDefinitionManager>();
		}

		#endregion

	}
}
