using System;
using NUnit.Framework;
using N2.Persistence;
using N2.Web;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace N2.Trashcan.Tests
{
	[TestFixture]
	public class DeleteInterceptorTests : TrashTestBase
	{
		IPersister persister;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			persister = mocks.CreateMock<IPersister>();
			Expect.Call(persister.Get(1)).Return(root).Repeat.Any();
		}

		[Test]
		public void DeletedItem_IsThrownInTrash()
		{
			persister.ItemDeleting += null;
			IEventRaiser invokeDelete = LastCall.IgnoreArguments().GetEventRaiser();
			persister.ItemCopied += null;
			LastCall.IgnoreArguments();
			persister.ItemMoved += null;
			LastCall.IgnoreArguments();

			TrashHandler th = mocks.CreateMock<TrashHandler>(persister, null, new Site(1));
			Expect.Call(delegate { th.Throw(item); });

			mocks.ReplayAll();

			DeleteInterceptor interceptor = new DeleteInterceptor(persister, th);
			interceptor.Start();

			CancellableItemEventArgs deleteArgs = new CancellableItemEventArgs(item);
			invokeDelete.Raise(persister, deleteArgs);

			Assert.AreEqual(true, deleteArgs.Cancel);

			mocks.VerifyAll();
		}

		[Test]
		public void TrashedItem_MovedFromTrashcan_IsUnexpired()
		{
			PutItemInTrash();

			persister.ItemDeleting += null;
			LastCall.IgnoreArguments();
			persister.ItemCopied += null;
			LastCall.IgnoreArguments();
			persister.ItemMoved += null;
			IEventRaiser invokeMoved = LastCall.IgnoreArguments().GetEventRaiser();

			TrashHandler th = mocks.PartialMock<TrashHandler>(persister, null, new Site(1));
			th.RestoreValues(item);

			mocks.ReplayAll();

			DeleteInterceptor interceptor = new DeleteInterceptor(persister, th);
			interceptor.Start();

			// now restore through drag&drop
			invokeMoved.Raise(persister, new CancellableDestinationEventArgs(item, root));

			mocks.VerifyAll();
		}

		[Test]
		public void TrashedItem_CopiedFromTrashcan_IsUnexpired()
		{
			PutItemInTrash();

			persister.ItemDeleting += null;
			LastCall.IgnoreArguments();
			persister.ItemCopied += null;
			IEventRaiser invokeCopied = LastCall.IgnoreArguments().GetEventRaiser();
			persister.ItemMoved += null;
			LastCall.IgnoreArguments();

			TrashHandler th = mocks.CreateMock<TrashHandler>(persister, null, new Site(1));
			th.RestoreValues(item);

			mocks.ReplayAll();

			DeleteInterceptor interceptor = new DeleteInterceptor(persister, th);
			interceptor.Start();

			// now restore through drag&drop
			invokeCopied.Raise(persister, new DestinationEventArgs(item, root));

			mocks.VerifyAll();
		}

		private void PutItemInTrash()
		{
			item.AddTo(trash);
			item[TrashHandler.DeletedDate] = DateTime.Now;
		}

		[Test]
		public void Item_MovedIntoTrash_IsNeutralized()
		{
			persister.ItemDeleting += null;
			LastCall.IgnoreArguments();
			persister.ItemCopied += null;
			LastCall.IgnoreArguments();
			persister.ItemMoved += null;
			IEventRaiser invokeMoved = LastCall.IgnoreArguments().GetEventRaiser();

			TrashHandler th = mocks.CreateMock<TrashHandler>(persister, null, new Site(1));
			th.ExpireTrashedItem(item);

			mocks.ReplayAll();

			DeleteInterceptor interceptor = new DeleteInterceptor(persister, th);
			interceptor.Start();

			// move item into trashcan
			invokeMoved.Raise(persister, new CancellableDestinationEventArgs(item, trash));

			mocks.VerifyAll();
		}

		[Test]
		[Ignore]
		public void Item_MovedOutOfTrash_IsCheckedForNameClashes()
		{
			Assert.Fail("TODO");
		}

		[Test]
		[Ignore]
		public void Item_MovedIntoTrash_GetsPreviousLocation_AsFormerLocation()
		{
			Assert.Fail("TODO");
		}
	}
}
