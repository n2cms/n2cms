using System;
using N2.Definitions;
using N2.Edit.Trash;
using N2.Edit.Workflow;
using N2.Persistence;
using N2.Web;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace N2.Edit.Tests.Trash
{
    [TestFixture]
    public class DeleteInterceptorTests : TrashTestBase
    {
        IPersister persister;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            persister = mocks.StrictMock<IPersister>();
            Expect.Call(persister.Get(1)).Return(root).Repeat.Any();
        }

        [Test]
        public void DeletedItem_IsThrownInTrash()
        {
            persister.ItemDeleting += null;
            IEventRaiser invokeDelete = LastCall.IgnoreArguments().GetEventRaiser();
            persister.ItemCopied += null;
            LastCall.IgnoreArguments();
            persister.ItemMoving += null;
            LastCall.IgnoreArguments();

			TrashHandler th = mocks.StrictMock<TrashHandler>(persister, null, null, new ContainerRepository<TrashContainerItem>(persister, null, new Host(webContext, 1, 1), null), null);
			th.UseNavigationMode = true;
            Expect.Call(delegate { th.Throw(item); });

            mocks.ReplayAll();

            DeleteInterceptor interceptor = new DeleteInterceptor(persister, th);
            interceptor.Start();

            CancellableItemEventArgs deleteArgs = new CancellableItemEventArgs(item);
            invokeDelete.Raise(persister, deleteArgs);

        	deleteArgs.FinalAction(deleteArgs.AffectedItem);

            mocks.VerifyAll();
        }

        [Test]
        public void NonThrowableItem_IsNotMovedToTrashcan()
        {
            IDefinitionManager definitions = mocks.Stub<IDefinitionManager>();

            IPersister persister = mocks.Stub<IPersister>();
            Expect.Call(persister.Get(1)).Return(root).Repeat.Any();
            persister.ItemDeleting += null;
            IEventRaiser invokeDelete = LastCall.IgnoreArguments().GetEventRaiser();

            mocks.ReplayAll();

			var host = new Host(webContext, 1, 1);
			TrashHandler th = new TrashHandler(persister, null, null, new ContainerRepository<TrashContainerItem>(persister, null, host, null), new StateChanger()) { UseNavigationMode = true };
            DeleteInterceptor interceptor = new DeleteInterceptor(persister, th);
            interceptor.Start();

            CancellableItemEventArgs deleteArgs = new CancellableItemEventArgs(nonThrowable);
            invokeDelete.Raise(persister, deleteArgs);

            Assert.That(deleteArgs.Cancel, Is.False);
            Assert.That(trash.Children.Count, Is.EqualTo(0));
		}

		[Test]
		public void NonThrowableItem_IsNotMovedToTrashcan_LegacyAttribute()
		{
			IDefinitionManager definitions = mocks.Stub<IDefinitionManager>();

			IPersister persister = mocks.Stub<IPersister>();
			Expect.Call(persister.Get(1)).Return(root).Repeat.Any();
			persister.ItemDeleting += null;
			IEventRaiser invokeDelete = LastCall.IgnoreArguments().GetEventRaiser();

			mocks.ReplayAll();

			var host = new Host(webContext, 1, 1);
			TrashHandler th = new TrashHandler(persister, null, null, new ContainerRepository<TrashContainerItem>(persister, null, host, null), new StateChanger()) { UseNavigationMode = true };
			DeleteInterceptor interceptor = new DeleteInterceptor(persister, th);
			interceptor.Start();

			CancellableItemEventArgs deleteArgs = new CancellableItemEventArgs(nonThrowable2);
			invokeDelete.Raise(persister, deleteArgs);

			Assert.That(deleteArgs.Cancel, Is.False);
			Assert.That(trash.Children.Count, Is.EqualTo(0));
		}

        [Test]
        public void TrashedItem_MovedFromTrashcan_IsUnexpired()
        {
            PutItemInTrash();

            persister.ItemDeleting += null;
            LastCall.IgnoreArguments();
            persister.ItemCopied += null;
            LastCall.IgnoreArguments();
            persister.ItemMoving += null;
            IEventRaiser invokeMoved = LastCall.IgnoreArguments().GetEventRaiser();

			TrashHandler th = mocks.PartialMock<TrashHandler>(persister, null, null, new ContainerRepository<TrashContainerItem>(persister, null, new Host(webContext, 1, 1), null), null);
			th.UseNavigationMode = true;
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
            persister.ItemMoving += null;
            LastCall.IgnoreArguments();

			TrashHandler th = mocks.StrictMock<TrashHandler>(persister, null, null, new ContainerRepository<TrashContainerItem>(persister, null, new Host(webContext, 1, 1), null), null);
			th.UseNavigationMode = true;
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
            persister.ItemMoving += null;
            IEventRaiser invokeMoved = LastCall.IgnoreArguments().GetEventRaiser();

			TrashHandler th = mocks.StrictMock<TrashHandler>(persister, null, null, new ContainerRepository<TrashContainerItem>(persister, null, new Host(webContext, 1, 1), null), null);
			th.UseNavigationMode = true;
            th.ExpireTrashedItem(item);

            mocks.ReplayAll();

            DeleteInterceptor interceptor = new DeleteInterceptor(persister, th);
            interceptor.Start();

            // move item into trashcan
            invokeMoved.Raise(persister, new CancellableDestinationEventArgs(item, trash));

            mocks.VerifyAll();
        }
    }
}
