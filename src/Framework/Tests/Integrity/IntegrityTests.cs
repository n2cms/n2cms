using System;
using System.Linq;
using N2.Configuration;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Engine;
using N2.Integrity;
using N2.Persistence;
using N2.Persistence.Proxying;
using N2.Tests.Fakes;
using N2.Tests.Integrity.Definitions;
using N2.Web;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace N2.Tests.Integrity
{
    [TestFixture]
    public class IntegrityTests : ItemTestsBase
    {
        private IPersister persister;
        private ContentActivator activator;
        private IDefinitionManager definitions;
        private IUrlParser parser;
        private IntegrityManager integrityManger;

        private IEventRaiser moving;
        private IEventRaiser copying;
        private IEventRaiser deleting;
        private IEventRaiser saving;
        private FakeContentItemRepository repository;

        #region SetUp

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            CreatePersister();

            parser = mocks.StrictMock<IUrlParser>();

            ITypeFinder typeFinder = CreateTypeFinder();
            DefinitionBuilder builder = new DefinitionBuilder(new DefinitionMap(), typeFinder, new TransformerBase<IUniquelyNamed>[0], TestSupport.SetupEngineSection());
            IItemNotifier notifier = mocks.DynamicMock<IItemNotifier>();
            mocks.Replay(notifier);
            var changer = new N2.Edit.Workflow.StateChanger();
            activator = new ContentActivator(changer, notifier, new EmptyProxyFactory());
            definitions = new DefinitionManager(new[] { new DefinitionProvider(builder) }, activator, changer, new DefinitionMap());
            integrityManger = new IntegrityManager(definitions, persister.Repository, parser);
            IntegrityEnforcer enforcer = new IntegrityEnforcer(persister, integrityManger, activator);
            enforcer.Start();
        }

        private ITypeFinder CreateTypeFinder()
        {
            return new FakeTypeFinder(
                typeof (IntegrityAlternativePage).Assembly, 
                typeof (IntegrityAlternativePage),
                typeof (IntegrityAlternativeStartPage),
                typeof (IntegrityPage),
                typeof (IntegrityRoot),
                typeof (IntegrityStartPage),
                typeof (IntegritySubPage));
        }

        private void CreatePersister()
        {
            mocks.Record();
            persister = mocks.DynamicMock<IPersister>();

            repository = new FakeContentItemRepository();

            persister.Expect(p => p.Repository).Return(repository);

            persister.ItemMoving += null;
            moving = LastCall.IgnoreArguments().Repeat.Any().GetEventRaiser();

            persister.ItemCopying += null;
            copying = LastCall.IgnoreArguments().Repeat.Any().GetEventRaiser();

            persister.ItemDeleting += null;
            deleting = LastCall.IgnoreArguments().Repeat.Any().GetEventRaiser();

            persister.ItemSaving += null;
            saving = LastCall.IgnoreArguments().Repeat.Any().GetEventRaiser();

            mocks.Replay(persister);
        }

        #endregion

        protected override T CreateOneItem<T>(int id, string name, ContentItem parent)
        {
            var item = base.CreateOneItem<T>(id, name, parent);
            repository.SaveOrUpdate(item);
            return item;
        }

        #region Move

        [Test]
        public void CanMoveItem()
        {
            IntegrityStartPage startPage = new IntegrityStartPage();
            IntegrityPage page = new IntegrityPage();
            bool canMove = integrityManger.CanMove(page, startPage);
            Assert.IsTrue(canMove, "The page couldn't be moved to the destination.");
        }

        [Test]
        public void CanMoveItemEvent()
        {
            IntegrityStartPage startPage = new IntegrityStartPage();
            IntegrityPage page = new IntegrityPage();

            moving.Raise(persister, new CancellableDestinationEventArgs(page, startPage));
        }

        [Test]
        public void CannotMoveItemOntoItself()
        {
            IntegrityPage page = new IntegrityPage();
            bool canMove = integrityManger.CanMove(page, page);
            Assert.IsFalse(canMove, "The page could be moved onto itself.");
        }

        [Test]
        public void CannotMoveItemOntoItselfEvent()
        {
            IntegrityPage page = new IntegrityPage();

            ExceptionAssert.Throws<DestinationOnOrBelowItselfException>(delegate
            {
                moving.Raise(persister, new CancellableDestinationEventArgs(page, page));
            });
        }

        [Test]
        public void CannotMoveItemBelowItself()
        {
            IntegrityPage page = new IntegrityPage();
            IntegrityPage page2 = CreateOneItem<IntegrityPage>(2, "Rutger", page);

            bool canMove = integrityManger.CanMove(page, page2);
            Assert.IsFalse(canMove, "The page could be moved below itself.");
        }

        [Test]
        public void CannotMoveItemBelowItselfEvent()
        {
            IntegrityPage page = new IntegrityPage();
            IntegrityPage page2 = CreateOneItem<IntegrityPage>(2, "Rutger", page);

            ExceptionAssert.Throws<DestinationOnOrBelowItselfException>(delegate
            {
                moving.Raise(persister, new CancellableDestinationEventArgs(page, page2));
            });
        }

        [Test]
        public void CannotMoveIfNameIsOccupied()
        {
            IntegrityStartPage startPage = CreateOneItem<IntegrityStartPage>(1, "start", null);
            IntegrityPage page2 = CreateOneItem<IntegrityPage>(2, "Sasha", startPage);
            IntegrityPage page3 = CreateOneItem<IntegrityPage>(3, "Sasha", null);

            bool canMove = integrityManger.CanMove(page3, startPage);
            Assert.IsFalse(canMove, "The page could be moved even though the name was occupied.");
        }

        [Test]
        public void CannotMoveIfNameIsOccupiedEvent()
        {
            IntegrityStartPage startPage = CreateOneItem<IntegrityStartPage>(1, "start", null);
            IntegrityPage page2 = CreateOneItem<IntegrityPage>(2, "Sasha", startPage);
            IntegrityPage page3 = CreateOneItem<IntegrityPage>(3, "Sasha", null);

            ExceptionAssert.Throws<NameOccupiedException>(delegate
            {
                moving.Raise(persister, new CancellableDestinationEventArgs(page3, startPage));
            });
        }

        [Test]
        public void CannotMoveIfTypeIsntAllowed()
        {
            IntegrityStartPage startPage = new IntegrityStartPage();
            IntegrityPage page = new IntegrityPage();

            bool canMove = integrityManger.CanMove(startPage, page);
            Assert.IsFalse(canMove, "The start page could be moved even though a page isn't an allowed destination.");
        }

        [Test]
        public void CannotMoveIfTypeIsntAllowedEvent()
        {
            IntegrityStartPage startPage = new IntegrityStartPage();
            IntegrityPage page = new IntegrityPage();

            ExceptionAssert.Throws<NotAllowedParentException>(delegate
            {
                moving.Raise(persister, new CancellableDestinationEventArgs(startPage, page));
            });
        }

        #endregion

        #region Copy

        [Test]
        public void CanCopyItem()
        {
            IntegrityStartPage startPage = new IntegrityStartPage();
            IntegrityPage page = new IntegrityPage();
            bool canCopy = integrityManger.CanCopy(page, startPage);
            Assert.IsTrue(canCopy, "The page couldn't be copied to the destination.");
        }

        [Test]
        public void CanCopyItemEvent()
        {
            IntegrityStartPage startPage = new IntegrityStartPage();
            IntegrityPage page = new IntegrityPage();

            copying.Raise(persister, new CancellableDestinationEventArgs(page, startPage));
        }

        [Test]
        public void CannotCopyIfNameIsOccupied()
        {
            IntegrityStartPage startPage = CreateOneItem<IntegrityStartPage>(1, "start", null);
            IntegrityPage page2 = CreateOneItem<IntegrityPage>(2, "Sasha", startPage);
            IntegrityPage page3 = CreateOneItem<IntegrityPage>(3, "Sasha", null);

            bool canCopy = integrityManger.CanCopy(page3, startPage);
            Assert.IsFalse(canCopy, "The page could be copied even though the name was occupied.");
        }

        [Test]
        public void CannotCopyIfNameIsOccupiedEvent()
        {
            IntegrityStartPage startPage = CreateOneItem<IntegrityStartPage>(1, "start", null);
            IntegrityPage page2 = CreateOneItem<IntegrityPage>(2, "Sasha", startPage);
            IntegrityPage page3 = CreateOneItem<IntegrityPage>(3, "Sasha", null);

            ExceptionAssert.Throws<NameOccupiedException>(delegate
            {
                copying.Raise(persister, new CancellableDestinationEventArgs(page3, startPage));
            });
        }

        [Test]
        public void CannotCopyIfTypeIsntAllowed()
        {
            IntegrityStartPage startPage = new IntegrityStartPage();
            IntegrityPage page = new IntegrityPage();

            bool canCopy = integrityManger.CanCopy(startPage, page);
            Assert.IsFalse(canCopy, "The start page could be copied even though a page isn't an allowed destination.");
        }

        [Test]
        public void CannotCopyIfTypeIsntAllowedEvent()
        {
            IntegrityStartPage startPage = new IntegrityStartPage();
            IntegrityPage page = new IntegrityPage();

            ExceptionAssert.Throws<NotAllowedParentException>(delegate
            {
                copying.Raise(persister, new CancellableDestinationEventArgs(startPage, page));
            });
        }

        #endregion

        #region Delete

        [Test]
        public void CanDelete()
        {
            IntegrityPage page = new IntegrityPage();

            mocks.Record();
            Expect.On(parser).Call(parser.IsRootOrStartPage(page)).Return(false);
            mocks.Replay(parser);

            bool canDelete = integrityManger.CanDelete(page);
            Assert.IsTrue(canDelete, "Page couldn't be deleted");

            mocks.Verify(parser);
        }

        [Test]
        public void CanDeleteEvent()
        {
            IntegrityPage page = new IntegrityPage();

            mocks.Record();
            Expect.On(parser).Call(parser.IsRootOrStartPage(page)).Return(false);
            mocks.Replay(parser);

            deleting.Raise(persister, new CancellableItemEventArgs(page));

            mocks.Verify(parser);
        }

        [Test]
        public void CannotDeleteStartPage()
        {
            IntegrityStartPage startPage = new IntegrityStartPage();

            mocks.Record();
            Expect.On(parser).Call(parser.IsRootOrStartPage(startPage)).Return(true);
            mocks.Replay(parser);

            bool canDelete = integrityManger.CanDelete(startPage);
            Assert.IsFalse(canDelete, "Start page could be deleted");

            mocks.Verify(parser);
        }

        [Test]
        public void CannotDeleteStartPageEvent()
        {
            IntegrityStartPage startPage = new IntegrityStartPage();

            mocks.Record();
            Expect.On(parser).Call(parser.IsRootOrStartPage(startPage)).Return(true);
            mocks.Replay(parser);

            ExceptionAssert.Throws<CannotDeleteRootException>(delegate
            {
                deleting.Raise(persister, new CancellableItemEventArgs(startPage));
            });
            mocks.Verify(parser);
        }

        #endregion

        #region Save

        [Test]
        public void CanSave()
        {
            IntegrityStartPage startPage = new IntegrityStartPage();

            bool canSave = integrityManger.CanSave(startPage);
            Assert.IsTrue(canSave, "Couldn't save");
        }

        [Test]
        public void CanSaveEvent()
        {
            IntegrityStartPage startPage = new IntegrityStartPage();

            saving.Raise(persister, new CancellableItemEventArgs(startPage));
        }

        [Test]
        public void CannotSaveNotLocallyUniqueItem()
        {
            IntegrityStartPage startPage = CreateOneItem<IntegrityStartPage>(1, "start", null);

            IntegrityPage page2 = CreateOneItem<IntegrityPage>(2, "Sasha", startPage);
            IntegrityPage page3 = CreateOneItem<IntegrityPage>(3, "Sasha", startPage);

            bool canSave = integrityManger.CanSave(page3);
            Assert.IsFalse(canSave, "Could save even though the item isn't the only sibling with the same name.");
        }

        [Test]
        public void LocallyUniqueItemThatWithoutNameYet()
        {
            IntegrityStartPage startPage = CreateOneItem<IntegrityStartPage>(1, "start", null);

            IntegrityPage page2 = CreateOneItem<IntegrityPage>(2, null, startPage);
            IntegrityPage page3 = CreateOneItem<IntegrityPage>(3, "Sasha", startPage);

            bool isUnique = integrityManger.IsLocallyUnique("Sasha", page2);
            Assert.IsFalse(isUnique, "Shouldn't have been locally unique.");
        }

        [Test]
        public void CannotSaveNotLocallyUniqueItemEvent()
        {
            IntegrityStartPage startPage = CreateOneItem<IntegrityStartPage>(1, "start", null);

            IntegrityPage page2 = CreateOneItem<IntegrityPage>(2, "Sasha", startPage);
            IntegrityPage page3 = CreateOneItem<IntegrityPage>(3, "Sasha", startPage);

            ExceptionAssert.Throws<NameOccupiedException>(delegate
            {
                saving.Raise(persister, new CancellableItemEventArgs(page3));
            });
        }

        [Test]
        public void CanSave_UnallowedItem()
        {
            IntegrityPage page = CreateOneItem<IntegrityPage>(1, "John", null);
            IntegrityStartPage startPage = CreateOneItem<IntegrityStartPage>(2, "Leonidas", page);

            bool canSave = integrityManger.CanSave(startPage);
            Assert.IsTrue(canSave);
        }

        [Test]
        public void CanSave_UnallowedItemEvent()
        {
            IntegrityPage page = CreateOneItem<IntegrityPage>(1, "John", null);
            IntegrityStartPage startPage = CreateOneItem<IntegrityStartPage>(2, "Leonidas", page);


            Assert.DoesNotThrow(() => saving.Raise(persister, new CancellableItemEventArgs(startPage)));
        }

        #endregion

        #region Create

        [Test]
        public void CannotCreate_ItemBelow_UnallowedParent()
        {
            var page = new IntegrityPage();

            // Doesn't throw
            activator.CreateInstance<IntegrityStartPage>(page);
        }

        [Test]
        public void AllowedItem_BelowSubClass_OfRoot_NotAllowedBelow_RootItem()
        {
            ContentItem root = CreateOneItem<Definitions.IntegrityStartPage>(0, "root", null);

            // doesn't throw
            activator.CreateInstance<Definitions.IntegrityAlternativePage>(root);
        }

        [Test]
        public void Root_IsntAllowed_BelowAllowedItem_BelowRoot()
        {
            ContentItem root = CreateOneItem<Definitions.IntegrityStartPage>(0, "page", null);

            // doesn't throw
            activator.CreateInstance<Definitions.IntegrityStartPage>(root);
        }

        [Test]
        public void UnAllowedItemBelowRoot()
        {
            ContentItem root = CreateOneItem<Definitions.IntegrityStartPage>(0, "root", null);

            // doesn't throw
            activator.CreateInstance(typeof(Definitions.IntegritySubPage), root);
        }

        #endregion
    }
}
