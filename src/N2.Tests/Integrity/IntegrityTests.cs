using System;
using System.Reflection;
using N2.Configuration;
using NUnit.Framework;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Integrity;
using N2.Persistence;
using N2.Tests.Integrity.Definitions;
using N2.Web;
using N2.Web.UI;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using N2.Tests.Fakes;
using System.Linq;
using N2.Persistence.Proxying;

namespace N2.Tests.Integrity
{
	[TestFixture]
	public class IntegrityTests : ItemTestsBase
	{
		private IPersister persister;
		private IDefinitionManager definitions;
		private IUrlParser parser;
		private IntegrityManager integrityManger;
		FakeItemFinder finder;

		private IEventRaiser moving;
		private IEventRaiser copying;
		private IEventRaiser deleting;
		private IEventRaiser saving;

		#region SetUp

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			CreatePersister();

			parser = mocks.StrictMock<IUrlParser>();

			ITypeFinder typeFinder = CreateTypeFinder();
			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, new EngineSection(), new FakeEditUrlManager());
			IItemNotifier notifier = mocks.DynamicMock<IItemNotifier>();
			mocks.Replay(notifier);
			definitions = new DefinitionManager(builder, new N2.Edit.Workflow.StateChanger(), notifier, new EmptyProxyFactory());
			finder = new FakeItemFinder(definitions, () => Enumerable.Empty<ContentItem>());
			integrityManger = new IntegrityManager(definitions, finder, parser);
			IntegrityEnforcer enforcer = new IntegrityEnforcer(persister, integrityManger, definitions);
			enforcer.Start();
		}

		private ITypeFinder CreateTypeFinder()
		{
			ITypeFinder typeFinder = mocks.StrictMock<ITypeFinder>();
			Expect.On(typeFinder)
				.Call(typeFinder.GetAssemblies())
				.Return(new Assembly[] {typeof (AlternativePage).Assembly})
				.Repeat.Any();
			Expect.On(typeFinder)
				.Call(typeFinder.Find(typeof (ContentItem)))
				.Return(new Type[]
				        	{
				        		typeof (AlternativePage),
				        		typeof (AlternativeStartPage),
				        		typeof (Page),
				        		typeof (Root),
				        		typeof (StartPage),
				        		typeof (SubPage)
				        	});
			mocks.Replay(typeFinder);
			return typeFinder;
		}

		private void CreatePersister()
		{
			mocks.Record();
			persister = mocks.DynamicMock<IPersister>();

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

		#region Move

		[Test]
		public void CanMoveItem()
		{
			StartPage startPage = new StartPage();
			Page page = new Page();
			bool canMove = integrityManger.CanMove(page, startPage);
			Assert.IsTrue(canMove, "The page couldn't be moved to the destination.");
		}

		[Test]
		public void CanMoveItemEvent()
		{
			StartPage startPage = new StartPage();
			Page page = new Page();

			moving.Raise(persister, new CancellableDestinationEventArgs(page, startPage));
		}

		[Test]
		public void CannotMoveItemOntoItself()
		{
			Page page = new Page();
			bool canMove = integrityManger.CanMove(page, page);
			Assert.IsFalse(canMove, "The page could be moved onto itself.");
		}

		[Test]
		public void CannotMoveItemOntoItselfEvent()
		{
			Page page = new Page();

			ExceptionAssert.Throws<DestinationOnOrBelowItselfException>(delegate
			{
				moving.Raise(persister, new CancellableDestinationEventArgs(page, page));
			});
		}

		[Test]
		public void CannotMoveItemBelowItself()
		{
			Page page = new Page();
			Page page2 = CreateOneItem<Page>(2, "Rutger", page);

			bool canMove = integrityManger.CanMove(page, page2);
			Assert.IsFalse(canMove, "The page could be moved below itself.");
		}

		[Test]
		public void CannotMoveItemBelowItselfEvent()
		{
			Page page = new Page();
			Page page2 = CreateOneItem<Page>(2, "Rutger", page);

			ExceptionAssert.Throws<DestinationOnOrBelowItselfException>(delegate
			{
				moving.Raise(persister, new CancellableDestinationEventArgs(page, page2));
			});
		}

		[Test]
		public void CannotMoveIfNameIsOccupied()
		{
			StartPage startPage = CreateOneItem<StartPage>(1, "start", null);
			Page page2 = CreateOneItem<Page>(2, "Sasha", startPage);
			Page page3 = CreateOneItem<Page>(3, "Sasha", null);

			bool canMove = integrityManger.CanMove(page3, startPage);
			Assert.IsFalse(canMove, "The page could be moved even though the name was occupied.");
		}

		[Test]
		public void CannotMoveIfNameIsOccupiedEvent()
		{
			StartPage startPage = CreateOneItem<StartPage>(1, "start", null);
			Page page2 = CreateOneItem<Page>(2, "Sasha", startPage);
			Page page3 = CreateOneItem<Page>(3, "Sasha", null);

			ExceptionAssert.Throws<NameOccupiedException>(delegate
			{
				moving.Raise(persister, new CancellableDestinationEventArgs(page3, startPage));
			});
		}

		[Test]
		public void CannotMoveIfTypeIsntAllowed()
		{
			StartPage startPage = new StartPage();
			Page page = new Page();

			bool canMove = integrityManger.CanMove(startPage, page);
			Assert.IsFalse(canMove, "The start page could be moved even though a page isn't an allowed destination.");
		}

		[Test]
		public void CannotMoveIfTypeIsntAllowedEvent()
		{
			StartPage startPage = new StartPage();
			Page page = new Page();

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
			StartPage startPage = new StartPage();
			Page page = new Page();
			bool canCopy = integrityManger.CanCopy(page, startPage);
			Assert.IsTrue(canCopy, "The page couldn't be copied to the destination.");
		}

		[Test]
		public void CanCopyItemEvent()
		{
			StartPage startPage = new StartPage();
			Page page = new Page();

			copying.Raise(persister, new CancellableDestinationEventArgs(page, startPage));
		}

		[Test]
		public void CannotCopyIfNameIsOccupied()
		{
			StartPage startPage = CreateOneItem<StartPage>(1, "start", null);
			Page page2 = CreateOneItem<Page>(2, "Sasha", startPage);
			Page page3 = CreateOneItem<Page>(3, "Sasha", null);

			bool canCopy = integrityManger.CanCopy(page3, startPage);
			Assert.IsFalse(canCopy, "The page could be copied even though the name was occupied.");
		}

		[Test]
		public void CannotCopyIfNameIsOccupiedEvent()
		{
			StartPage startPage = CreateOneItem<StartPage>(1, "start", null);
			Page page2 = CreateOneItem<Page>(2, "Sasha", startPage);
			Page page3 = CreateOneItem<Page>(3, "Sasha", null);

			ExceptionAssert.Throws<NameOccupiedException>(delegate
			{
				copying.Raise(persister, new CancellableDestinationEventArgs(page3, startPage));
			});
		}

		[Test]
		public void CannotCopyIfTypeIsntAllowed()
		{
			StartPage startPage = new StartPage();
			Page page = new Page();

			bool canCopy = integrityManger.CanCopy(startPage, page);
			Assert.IsFalse(canCopy, "The start page could be copied even though a page isn't an allowed destination.");
		}

		[Test]
		public void CannotCopyIfTypeIsntAllowedEvent()
		{
			StartPage startPage = new StartPage();
			Page page = new Page();

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
			Page page = new Page();

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
			Page page = new Page();

			mocks.Record();
			Expect.On(parser).Call(parser.IsRootOrStartPage(page)).Return(false);
			mocks.Replay(parser);

			deleting.Raise(persister, new CancellableItemEventArgs(page));

			mocks.Verify(parser);
		}

		[Test]
		public void CannotDeleteStartPage()
		{
			StartPage startPage = new StartPage();

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
			StartPage startPage = new StartPage();

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
			StartPage startPage = new StartPage();

			bool canSave = integrityManger.CanSave(startPage);
			Assert.IsTrue(canSave, "Couldn't save");
		}

		[Test]
		public void CanSaveEvent()
		{
			StartPage startPage = new StartPage();

			saving.Raise(persister, new CancellableItemEventArgs(startPage));
		}

		[Test]
		public void CannotSaveNotLocallyUniqueItem()
		{
			StartPage startPage = CreateOneItem<StartPage>(1, "start", null);
			finder.Selector = () => startPage.Children.Where(c => c.Name.Equals("Sasha", StringComparison.InvariantCultureIgnoreCase));

			Page page2 = CreateOneItem<Page>(2, "Sasha", startPage);
			Page page3 = CreateOneItem<Page>(3, "Sasha", startPage);

			bool canSave = integrityManger.CanSave(page3);
			Assert.IsFalse(canSave, "Could save even though the item isn't the only sibling with the same name.");
		}

		[Test]
		public void LocallyUniqueItemThatWithoutNameYet()
		{
			StartPage startPage = CreateOneItem<StartPage>(1, "start", null);
			finder.Selector = () => startPage.Children.Where(c => c.Name.Equals("Sasha", StringComparison.InvariantCultureIgnoreCase));

			Page page2 = CreateOneItem<Page>(2, null, startPage);
			Page page3 = CreateOneItem<Page>(3, "Sasha", startPage);

			bool isUnique = integrityManger.IsLocallyUnique("Sasha", page2);
			Assert.IsFalse(isUnique, "Shouldn't have been locally unique.");
		}

		[Test]
		public void CannotSaveNotLocallyUniqueItemEvent()
		{
			StartPage startPage = CreateOneItem<StartPage>(1, "start", null);
			finder.Selector = () => startPage.Children.Where(c => c.Name.Equals("Sasha", StringComparison.InvariantCultureIgnoreCase));

			Page page2 = CreateOneItem<Page>(2, "Sasha", startPage);
			Page page3 = CreateOneItem<Page>(3, "Sasha", startPage);

			ExceptionAssert.Throws<NameOccupiedException>(delegate
			{
				saving.Raise(persister, new CancellableItemEventArgs(page3));
			});
		}

		[Test]
		public void CannotSaveUnallowedItem()
		{
			Page page = CreateOneItem<Page>(1, "John", null);
			StartPage startPage = CreateOneItem<StartPage>(2, "Leonidas", page);

			bool canSave = integrityManger.CanSave(startPage);
			Assert.IsFalse(canSave, "Could save even though the start page isn't below a page.");
		}

		[Test]
		public void CannotSaveUnallowedItemEvent()
		{
			Page page = CreateOneItem<Page>(1, "John", null);
			StartPage startPage = CreateOneItem<StartPage>(2, "Leonidas", page);

			ExceptionAssert.Throws<NotAllowedParentException>(delegate
			{
				saving.Raise(persister, new CancellableItemEventArgs(startPage));
			});
		}

		#endregion

		#region Security

		[Test]
		public void UserCanEditAccessibleDetail()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (Page));
			Assert.AreEqual(1,
			                definition.GetEditables(SecurityUtilities.CreatePrincipal("UserNotInTheGroup", "ACertainGroup")).
			                	Count);
		}

		[Test]
		public void UserCannotEditInaccessibleDetail()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (Page));
			Assert.AreEqual(0,
			                definition.GetEditables(SecurityUtilities.CreatePrincipal("UserNotInTheGroup", "Administrator")).
			                	Count);
		}

		#endregion

		#region Create

		[Test]
		public void CannotCreate_ItemBelow_UnallowedParent()
		{
			var page = new Page();

			ExceptionAssert.Throws<NotAllowedParentException>(delegate
			{
				var neverReturned = definitions.CreateInstance<StartPage>(page);
			});
		}

		[Test]
		public void AllowedItem_BelowSubClass_OfRoot_NotAllowedBelow_RootItem()
		{
			ContentItem root = CreateOneItem<Definitions.StartPage>(0, "root", null);

			Assert.Throws<NotAllowedParentException>(() => definitions.CreateInstance<Definitions.AlternativePage>(root));
		}

		[Test]
		public void Root_IsntAllowed_BelowAllowedItem_BelowRoot()
		{
			ContentItem root = CreateOneItem<Definitions.Page>(0, "page", null);

			Assert.Throws<NotAllowedParentException>(() => definitions.CreateInstance<Definitions.StartPage>(root));
		}

		[Test]
		public void UnAllowedItemBelowRoot()
		{
			ContentItem root = CreateOneItem<Definitions.StartPage>(0, "root", null);

			Assert.Throws<NotAllowedParentException>(() => definitions.CreateInstance(typeof(Definitions.SubPage), root));
		}

		#endregion
	}
}