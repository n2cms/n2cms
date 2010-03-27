using NUnit.Framework;
using Rhino.Mocks;
using N2.Web;
using N2.Edit.Trash;

namespace N2.Edit.Tests.Trash
{
    public class TrashTestBase
    {
        protected ThreadContext webContext;
        protected Host host;

        protected MockRepository mocks;
        protected ThrowableItem root;
        protected ThrowableItem item;
		protected NonThrowableItem nonThrowable;
		protected LegacyNonThrowableItem nonThrowable2;
		protected TrashContainerItem trash;

        [SetUp]
        public virtual void SetUp()
        {
            mocks = new MockRepository();

            root = CreateItem<ThrowableItem>(1, "root", null);
            item = CreateItem<ThrowableItem>(2, "item", root);
            trash = CreateItem<TrashContainerItem>(3, "Trash", root);
			nonThrowable = CreateItem<NonThrowableItem>(4, "neverInTrash", root);
			nonThrowable2 = CreateItem<LegacyNonThrowableItem>(5, "neverInTrash2", root);

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
    }
}