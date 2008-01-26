using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

using N2;
using MbUnit.Framework;
using N2.Security;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace N2.Tests.Security
{
	[TestFixture]
	public class SecurityManagerTests : ItemTestsBase
	{
		#region Fields
		N2.Persistence.IPersister persister;
		DefaultSecurityManager security;
		N2.Web.IUrlParser parser;
		N2.Web.IWebContext context;

		private IEventRaiser moving;
		private IEventRaiser copying;
		private IEventRaiser deleting;
		private IEventRaiser saving; 
		#endregion

		#region SetUp
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			CreatePersister();
			parser = mocks.CreateMock<N2.Web.IUrlParser>();
			context = CreateWebContext(false);

			security = new DefaultSecurityManager(context);
			SecurityEnforcer enforcer = new SecurityEnforcer(persister, security, parser, context);
			enforcer.Start();
		}

		private void CreatePersister()
		{
			mocks.Record();
			persister = mocks.DynamicMock<N2.Persistence.IPersister>();

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

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAll();
		}
		#endregion

		[Test]
		public void AdminIsAdmin()
		{
			IPrincipal admin = CreatePrincipal("admin");
			mocks.ReplayAll();

			Assert.IsTrue(security.IsAdmin(admin));
		}

		[Test]
		public void AdministratorsAreAdmins()
		{
			mocks.ReplayAll();

			IPrincipal admin = CreatePrincipal("JustAUser", "Administrators");
			Assert.IsTrue(security.IsAdmin(admin));
		}

		[Test]
		public void AdminIsEditor()
		{
			mocks.ReplayAll();

			IPrincipal admin = CreatePrincipal("admin");
			Assert.IsTrue(security.IsEditor(admin));
		}

		[Test]
		public void AdministratorsAreEditors()
		{
			mocks.ReplayAll();

			IPrincipal admin = CreatePrincipal("JustAUser", "Administrators");
			Assert.IsTrue(security.IsEditor(admin));
		}

		[Test]
		public void EditorsAreEditors()
		{
			mocks.ReplayAll();

			IPrincipal editor = CreatePrincipal("JustAnyUser", "Editors");
			Assert.IsTrue(security.IsEditor(editor));
		}

		[Test]
		public void EditorsAreNotAdmins()
		{
			mocks.ReplayAll();

			IPrincipal editor = CreatePrincipal("JustAnyUser", "Editors");
			Assert.IsFalse(security.IsAdmin(editor));
		}

		[Test]
		public void UsersAreNotEditors()
		{
			mocks.ReplayAll();

			IPrincipal user = CreatePrincipal("User1");
			Assert.IsFalse(security.IsEditor(user));
		}

		[Test]
		public void UsersAreNotAdmins()
		{
			mocks.ReplayAll();

			IPrincipal user = CreatePrincipal("User1");
			Assert.IsFalse(security.IsAdmin(user));
		}


		[Test]
		public void AuthorizedRolePropertyOnItems()
		{
			mocks.ReplayAll();

			ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
			root.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(root, "Role1"));

			IPrincipal user = CreatePrincipal("User1", "Role1");
			Assert.IsTrue(security.IsAuthorized(root, user));
		}

		[Test]
		public void AuthorizedRoleDoesNotIncludeUserName()
		{
			mocks.ReplayAll();

			ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
			root.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(root, "User1"));

			IPrincipal user = CreatePrincipal("User1");
			Assert.IsFalse(security.IsAuthorized(root, user), "User1 shouldn't have has access.");
		}

		[Test]
		public void Unpublished()
		{
			mocks.ReplayAll();

			ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
			root.Published = DateTime.Now.AddDays(1);

			IPrincipal user = CreatePrincipal("User1");
			Assert.IsFalse(security.IsEditor(user), "Dang! He's editor.");
			Assert.IsFalse(security.IsAuthorized(root, user));
		}

		[Test]
		public void NonEditorCannotAccessExpired()
		{
			mocks.ReplayAll();

			ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
			root.Expires = DateTime.Now;

			IPrincipal user = CreatePrincipal("User1");
			Assert.IsFalse(security.IsAuthorized(root, user));
		}

		[Test]
		public void NonEditorCannotAccessUnpublished()
		{
			mocks.ReplayAll();

			ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
			root.Published = null;

			IPrincipal user = CreatePrincipal("User1");
			Assert.IsFalse(security.IsAuthorized(root, user));
		}

		[Test]
		public void NonEditorCannotAccessUnpublished2()
		{
			mocks.ReplayAll();

			ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
			root.Published = DateTime.Now.AddSeconds(10);

			IPrincipal user = CreatePrincipal("User1");
			Assert.IsFalse(security.IsAuthorized(root, user));
		}

		[Test]
		public void ExpiredIsNotPublished()
		{
			mocks.ReplayAll();

			ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
			root.Expires = DateTime.Now;

			bool isPublished = security.IsPublished(root);
			Assert.IsFalse(isPublished, "Item should not have been published.");
		}

		[Test]
		public void UnpublishedIsNotPublished()
		{
			mocks.ReplayAll();

			ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
			root.Published = null;

			bool isPublished = security.IsPublished(root);
			Assert.IsFalse(isPublished, "Item should not have been published.");
		}

		[Test]
		public void NotYetPublishedIsNotPublished()
		{
			mocks.ReplayAll();

			ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
			root.Published = DateTime.Now.AddDays(1);

			bool isPublished = security.IsPublished(root);
			Assert.IsFalse(isPublished);
		}

		[Test]
		public void UserCanSaveAccessibleItem()
		{
			ContentItem root = CreateUserAndItem();
			mocks.ReplayAll();

			saving.Raise(persister, new N2.Persistence.CancellableItemEventArgs(root));
		}

		[Test, ExpectedException(typeof(N2.Security.PermissionDeniedException))]
		public void UserCannotSaveInaccessibleItem()
		{
			ContentItem root = SetupNormalUserAndCreateRestrictedItem();
			mocks.ReplayAll();

			saving.Raise(persister, new N2.Persistence.CancellableItemEventArgs(root));
		}

		[Test]
		public void UserCanDeleteAccessibleItem()
		{
			ContentItem root = CreateUserAndItem();
			mocks.ReplayAll();

			deleting.Raise(persister, new N2.Persistence.CancellableItemEventArgs(root));
		}

		[Test, ExpectedException(typeof(N2.Security.PermissionDeniedException))]
		public void UserCannotDeleteInaccessibleItem()
		{
			ContentItem root = SetupNormalUserAndCreateRestrictedItem();
			mocks.ReplayAll();

			deleting.Raise(persister, new N2.Persistence.CancellableItemEventArgs(root));
		}

		[Test]
		public void UserCanMoveAccessibleItem()
		{
			ContentItem source = CreateUserAndItem();
			ContentItem destination = CreateOneItem<Items.SecurityPage>(2, "accessible page", null);
			mocks.ReplayAll();

			moving.Raise(persister, new N2.Persistence.CancellableDestinationEventArgs(source, destination));
		}

		[Test, ExpectedException(typeof(N2.Security.PermissionDeniedException))]
		public void UserCannotMoveInaccessibleItem()
		{
			ContentItem source = SetupNormalUserAndCreateRestrictedItem();
			ContentItem destination = CreateOneItem<Items.SecurityPage>(2, "accessible page", null);
			mocks.ReplayAll();

			moving.Raise(persister, new N2.Persistence.CancellableDestinationEventArgs(source, destination));
		}

		[Test, ExpectedException(typeof(N2.Security.PermissionDeniedException))]
		public void UserCannotMoveToInaccessibleItem()
		{
			ContentItem destination = SetupNormalUserAndCreateRestrictedItem();
			ContentItem source = CreateOneItem<Items.SecurityPage>(2, "accessible page", null);
			
			mocks.ReplayAll();
			moving.Raise(persister, new N2.Persistence.CancellableDestinationEventArgs(source, destination));
		}

		[Test]
		public void UserCanCopyAccessibleItem()
		{
			ContentItem source = CreateUserAndItem();
			ContentItem destination = CreateOneItem<Items.SecurityPage>(2, "accessible page", null);
			mocks.ReplayAll();

			copying.Raise(persister, new N2.Persistence.CancellableDestinationEventArgs(source, destination));
		}

		[Test, ExpectedException(typeof(N2.Security.PermissionDeniedException))]
		public void UserCannotCopyInaccessibleItem()
		{
			ContentItem source = SetupNormalUserAndCreateRestrictedItem();
			ContentItem destination = CreateOneItem<Items.SecurityPage>(2, "accessible page", null);
			mocks.ReplayAll();

			copying.Raise(persister, new N2.Persistence.CancellableDestinationEventArgs(source, destination));
		}

		[Test, ExpectedException(typeof(N2.Security.PermissionDeniedException))]
		public void UserCannotCopyToInaccessibleItem()
		{
			ContentItem destination = SetupNormalUserAndCreateRestrictedItem();
			ContentItem source = CreateOneItem<Items.SecurityPage>(2, "accessible page", null);
			mocks.ReplayAll();

			copying.Raise(persister, new N2.Persistence.CancellableDestinationEventArgs(source, destination));
		}

		[Test]
		public void CanChangeAdminUser()
		{
			IPrincipal user = CreatePrincipal("AverageJoe");
			mocks.ReplayAll();

			security.AdminNames = new string[] { "AverageJoe" };

			Assert.IsTrue(security.IsAdmin(user), "User wasn't admin.");
		}

		[Test]
		public void CanHaveMoreThanOneAdmin()
		{
			IPrincipal maria = CreatePrincipal("SpecialMaria");
			IPrincipal bill = CreatePrincipal("SmallBill");
			mocks.ReplayAll();

			security.AdminNames = new string[] { "AverageJoe", "SpecialMaria", "SmallBill" };

			Assert.IsTrue(security.IsAdmin(maria), "User wasn't admin.");
			Assert.IsTrue(security.IsAdmin(bill), "User wasn't admin.");
		}

		[Test]
		public void CanChangeAdminRoles()
		{
			IPrincipal user = CreatePrincipal("AverageJoe", "AverageUser", "FriendlyUser");
			mocks.ReplayAll();

			security.AdminRoles = new string[] { "FriendlyUser" };

			Assert.IsTrue(security.IsAdmin(user), "User wasn't admin.");
		}

		[Test]
		public void CanChangeEditorUser()
		{
			IPrincipal user = CreatePrincipal("AverageJoe");
			mocks.ReplayAll();

			security.EditorNames = new string[] { "AverageJoe" };

			Assert.IsTrue(security.IsEditor(user), "User wasn't editor.");
		}

		[Test]
		public void CanChangeEditorRoles()
		{
			IPrincipal user = CreatePrincipal("AverageJoe", "GhostWhisperer", "SiteSeeker");
			mocks.ReplayAll();

			security.EditorRoles = new string[] { "GhostWhisperer" };

			Assert.IsTrue(security.IsEditor(user), "User wasn't editor.");
		}

		private ContentItem CreateUserAndItem()
		{
			Expect.On(context).Call(context.CurrentUser).Return(CreatePrincipal("JustAnyGuy")).Repeat.Any();
			mocks.Replay(context);

			ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
			return root;
		}

		private ContentItem SetupNormalUserAndCreateRestrictedItem()
		{
			Expect.On(context).Call(context.CurrentUser).Return(CreatePrincipal("JustAnyGuy")).Repeat.Any();
			mocks.Replay(context);

			ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
			root.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(root, "Editors"));
			return root;
		}
	}
}
