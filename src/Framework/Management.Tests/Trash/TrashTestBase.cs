using N2.Edit.Trash;
using N2.Web;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Persistence;
using N2.Tests;
using N2.Edit.Workflow;
using N2.Persistence.Proxying;

namespace N2.Edit.Tests.Trash
{
    public abstract class TrashTestBase
    {
        protected ThreadContext webContext;
        protected Host host;

        protected MockRepository mocks;
        protected ThrowableItem root;
        protected ThrowableItem item;
		protected TrashContainerItem trash;

        [SetUp]
        public virtual void SetUp()
        {
            mocks = new MockRepository();

            root = CreateItem<ThrowableItem>(1, "root", null);
            item = CreateItem<ThrowableItem>(2, "item", root);
            trash = CreateItem<TrashContainerItem>(3, "Trash", root);

            webContext = new ThreadContext();
            host = new Host(webContext, 1, 1);
        }

        [TearDown]
        public void TearDown()
        {
            mocks.ReplayAll();
            mocks.VerifyAll();
        }

        protected T CreateItem<T>(int id, string name, ContentItem parent) where T : ContentItem, new()
        {
            T i = new T();
            i.Name = name;
            i.ID = id;
            i.AddTo(parent);
            return i;
        }

		protected TrashHandler CreateTrashHandler()
		{
			var activator = new ContentActivator(new StateChanger(), new ItemNotifier(), new EmptyProxyFactory());
			var persister = TestSupport.SetupFakePersister();
			persister.Save(root);

			return new TrashHandler(persister, null, null, new ContainerRepository<TrashContainerItem>(persister.Repository, null, host, activator), new StateChanger()) { UseNavigationMode = true };
		}

    }
}