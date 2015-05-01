using System;
using N2.Definitions;
using N2.Edit.Trash;
using N2.Edit.Workflow;
using N2.Persistence;
using N2.Web;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using N2.Tests;
using Shouldly;

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

            persister = TestSupport.SetupFakePersister();
            persister.Save(root);
        }

        [Test]
        public void DeletedItem_IsThrownInTrash()
        {
            var th = CreateTrashHandler();
            th.UseNavigationMode = true;

            DeleteInterceptor interceptor = new DeleteInterceptor(persister, th);
            interceptor.Start();

            persister.Delete(item);

            item.Parent.ShouldBeOfType<TrashContainerItem>();
        }

        [Test]
        public void NonThrowableItem_IsNotMovedToTrashcan()
        {
            var nonThrowable = CreateItem<NonThrowableItem>(4, "neverInTrash", root);
            var nonThrowable2 = CreateItem<LegacyNonThrowableItem>(5, "neverInTrash2", root);

            var th = CreateTrashHandler();
            DeleteInterceptor interceptor = new DeleteInterceptor(persister, th);
            interceptor.Start();

            persister.Delete(nonThrowable);
            
            Assert.That(trash.Children.Count, Is.EqualTo(0));
        }

        [Test]
        [Obsolete]
        public void NonThrowableItem_IsNotMovedToTrashcan_LegacyAttribute()
        {
            var nonThrowable = CreateItem<NonThrowableItem>(4, "neverInTrash", root);

            var th = CreateTrashHandler();
            DeleteInterceptor interceptor = new DeleteInterceptor(persister, th);
            interceptor.Start();

            persister.Delete(nonThrowable);
            
            Assert.That(trash.Children.Count, Is.EqualTo(0));
        }

        [Test]
        public void TrashedItem_MovedFromTrashcan_IsUnexpired()
        {
            PutItemInTrash();

            var th = CreateTrashHandler();
            th.UseNavigationMode = true;
            th.RestoreValues(item);
            DeleteInterceptor interceptor = new DeleteInterceptor(persister, th);
            interceptor.Start();

            // now restore through drag&drop
            persister.Move(item, root);

            item.Parent.ShouldBe(root);
            item[TrashHandler.DeletedDate].ShouldBe(null);
        }

        [Test]
        public void TrashedItem_CopiedFromTrashcan_IsUnexpired()
        {
            PutItemInTrash();

            var th = CreateTrashHandler();
            th.UseNavigationMode = true;
            th.RestoreValues(item);
            DeleteInterceptor interceptor = new DeleteInterceptor(persister, th);
            interceptor.Start();

            var copy = persister.Copy(item, root);

            copy.Parent.ShouldBe(root);
            copy[TrashHandler.DeletedDate].ShouldBe(null);
        }

        private void PutItemInTrash()
        {
            item.AddTo(trash);
            item[TrashHandler.DeletedDate] = N2.Utility.CurrentTime();
        }

        [Test]
        public void Item_MovedIntoTrash_IsNeutralized()
        {
            var th = CreateTrashHandler();
            th.UseNavigationMode = true;
            th.ExpireTrashedItem(item);
            DeleteInterceptor interceptor = new DeleteInterceptor(persister, th);
            interceptor.Start();

            persister.Move(item, trash);

            item[TrashHandler.DeletedDate].ShouldNotBe(null);
        }
    }
}
