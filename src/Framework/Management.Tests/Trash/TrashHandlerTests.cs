using System;
using N2.Definitions;
using N2.Edit.Trash;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Engine.MediumTrust;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Web;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Tests;
using N2.Persistence.Proxying;
using N2.Tests.Fakes;

namespace N2.Edit.Tests.Trash
{
    [TestFixture]
    public class TrashHandlerTests : TrashTestBase
    {
        [Test]
        public void ThrownItem_IsMoved_ToTrashcan()
        {
			TrashHandler th = CreateTrashHandler();

			th.Throw(item);

            Assert.AreEqual(trash, item.Parent);
        }

        [Test]
        public void ThrownItem_IsExpired()
        {
            TrashHandler th = CreateTrashHandler();
            th.Throw(item);

            Assert.Less(DateTime.Now.AddSeconds(-10), item.Expires);
        }

        [Test]
        public void ChildrenOf_ThrownItem_AreExpired()
        {
            TrashHandler th = CreateTrashHandler();

            var child1 = CreateItem<ThrowableItem>(5, "child1", item);
            var child2 = CreateItem<ThrowableItem>(6, "child2", child1);

            th.Throw(item);

            Assert.That(child1.Expires, Is.Not.Null);
            Assert.That(child2.Expires, Is.Not.Null);
            Assert.That(child1.Expires, Is.GreaterThan(DateTime.Now.AddSeconds(-10)));
            Assert.That(child2.Expires, Is.GreaterThan(DateTime.Now.AddSeconds(-10)));
        }

        [Test]
        public void ChildrenOf_RestoredItems_AreUnExpired()
        {
            TrashHandler th = CreateTrashHandler();

            var child1 = CreateItem<ThrowableItem>(5, "child1", item);
            var child2 = CreateItem<ThrowableItem>(6, "child2", child1);

            th.Throw(item);

            th.Restore(item);

            Assert.That(child1.Expires, Is.Null);
            Assert.That(child2.Expires, Is.Null);
        }

        [Test]
        public void ThrownItem_NameIsCleared()
        {
            TrashHandler th = CreateTrashHandler();
            th.Throw(item);

            Assert.AreNotEqual("item", item.Name);
        }

        [Test]
        public void ThrownItem_OldValues_AreStored_InDetailBag()
        {
            TrashHandler th = CreateTrashHandler();
            th.Throw(item);

            Assert.AreEqual("item", item[TrashHandler.FormerName]);
            Assert.AreEqual(root, item[TrashHandler.FormerParent]);
            Assert.IsNull(item[TrashHandler.FormerExpires]);
            Assert.Less(DateTime.Now.AddSeconds(-10), (DateTime)item[TrashHandler.DeletedDate]);
        }

        [Test]
        public void Throwing_IsIntercepted_InMediumTrust()
        {
			IEngine engine = new ContentEngine(new MediumTrustServiceContainer(), new EventBroker(), new ContainerConfigurer());
			engine.Initialize();
			engine.Persister.Dispose();

			var schemaCreator = new SchemaExport(engine.Resolve<IConfigurationBuilder>().BuildConfiguration());
			var conn = engine.Resolve<ISessionProvider>().OpenSession.Session.Connection;
			schemaCreator.Execute(false, true, false, conn, null);

            engine.SecurityManager.Enabled = false;

            ContentItem root = new ThrowableItem();
            root.Name = "root_mediumtrust";

            ContentItem item = new ThrowableItem();
            item.Name = "bin's destiny";
            item.AddTo(root);

            engine.Persister.Save(root);
            engine.Resolve<IHost>().DefaultSite.RootItemID = root.ID;
            engine.Resolve<IHost>().DefaultSite.StartPageID = root.ID;

            engine.Persister.Delete(item);

            Assert.That(root.Children.Count, Is.EqualTo(1));
            Assert.That(root.Children[0], Is.TypeOf(typeof(TrashContainerItem)));
            Assert.That(root.Children[0].Children[0], Is.EqualTo(item));
        }

        [Test]
        public void ThrashHandler_Throw_WillInvokeEvents()
        {
			var th = CreateTrashHandler();

            bool throwingWasInvoked = false;
            bool throwedWasInvoked = false;
            th.ItemThrowing += delegate { throwingWasInvoked = true; };
            th.ItemThrowed += delegate { throwedWasInvoked = true; };
            th.Throw(item);

            Assert.That(throwingWasInvoked);
            Assert.That(throwedWasInvoked);
        }

        [Test]
        public void ThrashHandler_Throw_CanBeCancelled()
        {
			var th = CreateTrashHandler();

            th.ItemThrowing += delegate(object sender, CancellableItemEventArgs args) { args.Cancel = true; };
            th.Throw(item);

			Assert.That(item.Parent, Is.Not.EqualTo(trash));
        }

		[Test]
		public void ThrownItem_ChangesState_ToDeleted()
		{
			TrashHandler th = CreateTrashHandler();
			th.Throw(item);

			Assert.That(item.State, Is.EqualTo(ContentState.Deleted));
		}

		[Test]
		public void RestoredItem_ChangesState_ToPreviousState()
		{
			item.State = ContentState.Published;

			TrashHandler th = CreateTrashHandler();
			th.Throw(item);
			th.Restore(item);

			Assert.That(item.State, Is.EqualTo(ContentState.Published));
		}

        #region Helper methods

        private TrashHandler CreateTrashHandler()
        {
			ContentActivator activator = new ContentActivator(null, null, null);
            IPersister persister = MockPersister(root, trash, item);
			Expect.Call(persister.Repository).Return(repository).Repeat.Any();
            Expect.Call(delegate { persister.Move(null, null); }).IgnoreArguments()
                .Do(new System.Action<ContentItem, ContentItem>(delegate(ContentItem source, ContentItem destination)
                                                             {
                                                                 source.AddTo(destination);
                                                             })).Repeat.Any();
			
            mocks.ReplayAll();

			return new TrashHandler(persister, null, null, new ContainerRepository<TrashContainerItem>(persister.Repository, null, host, activator), new StateChanger(), new ThreadContext()) { UseNavigationMode = true };
        }

        private IPersister MockPersister(ContentItem root, ContentItem trash, ContentItem item)
        {
            IPersister persister = mocks.StrictMock<IPersister>();
            Expect.Call(persister.Get(1)).Return(root).Repeat.Any();
            Expect.Call(delegate { persister.Save(item); }).Repeat.Any();
            return persister;
        }

        #endregion

    }
}
