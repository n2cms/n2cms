using System;
using System.Security.Principal;
using N2.Configuration;
using N2.Persistence;
using N2.Security;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace N2.Tests.Security
{
    [TestFixture]
    public class SecurityManagerTests : ItemTestsBase
    {
        #region Fields
        N2.Persistence.IPersister persister;
        SecurityManager security;
        SecurityEnforcer enforcer;
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
            parser = mocks.StrictMock<N2.Web.IUrlParser>();
            context = CreateWebContext(false);

            EditSection editSection = new EditSection();
            security = new SecurityManager(context, editSection);
            enforcer = new SecurityEnforcer(persister, security, new ContentActivator(null, null, null), parser, context, new HostSection());
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
        #endregion

        [Test]
        public void AdminIsAdmin()
        {
            IPrincipal admin = CreatePrincipal("admin");

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
        public void Administrator_IsAuthorized_ToPublish()
        {
            ContentItem item = CreateOneItem<Items.SecurityPage>(1, "root", null);
            IPrincipal adminUser = CreatePrincipal("admin");

            var isAuthorized = security.IsAuthorized(adminUser, item, Permission.Publish);

            Assert.That(isAuthorized, Is.True);
        }

        [Test]
        public void Writer_IsNotAuthorized_ToPublish()
        {
            ContentItem item = CreateOneItem<Items.SecurityPage>(1, "root", null);
            IPrincipal writerUser = CreatePrincipal("joe", "Writers");

            var isAuthorized = security.IsAuthorized(writerUser, item, Permission.Publish);

            Assert.That(isAuthorized, Is.False);
        }

        [Test]
        public void Writer_IsAuthorized_ToPublish_OnRemappedPage()
        {
            ContentItem remappedItem = CreateOneItem<Items.RemappedPage>(1, "root", null);
            IPrincipal writerUser = CreatePrincipal("joe", "Writers");

            var isAuthorized = security.IsAuthorized(writerUser, remappedItem, Permission.Publish);

            Assert.That(isAuthorized, Is.True);
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
            DynamicPermissionMap.SetRoles(root, Permission.Read, "User1");

            IPrincipal user = CreatePrincipal("User1");
            Assert.IsFalse(security.IsAuthorized(root, user), "User1 shouldn't have has access.");
        }

        [Test]
        public void Unpublished()
        {
            mocks.ReplayAll();

            ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
            root.Published = N2.Utility.CurrentTime().AddDays(1);

            IPrincipal user = CreatePrincipal("User1");
            Assert.IsFalse(security.IsEditor(user), "Dang! He's editor.");
            Assert.IsFalse(security.IsAuthorized(root, user));
        }

        [Test]
        public void NonEditorCannotAccessExpired()
        {
            mocks.ReplayAll();

            ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
            root.Expires = N2.Utility.CurrentTime();

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
            root.Published = N2.Utility.CurrentTime().AddSeconds(10);

            IPrincipal user = CreatePrincipal("User1");
            Assert.IsFalse(security.IsAuthorized(root, user));
        }

        [Test]
        public void ExpiredIsNotPublished()
        {
            mocks.ReplayAll();

            ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
            root.Expires = N2.Utility.CurrentTime();

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
            root.Published = N2.Utility.CurrentTime().AddDays(1);

            bool isPublished = security.IsPublished(root);
            Assert.IsFalse(isPublished);
        }

        [Test]
        public void UserCanSaveAccessibleItem()
        {
            ContentItem root = CreateUserAndItem();
            mocks.ReplayAll();

            saving.Raise(persister, new CancellableItemEventArgs(root));
        }

        [Test]
        public void UserCannotSaveInaccessibleItem()
        {
            ContentItem root = SetupNormalUserAndCreateRestrictedItem();
            mocks.ReplayAll();

            ExceptionAssert.Throws<PermissionDeniedException>(delegate
            {
                saving.Raise(persister, new CancellableItemEventArgs(root));
            });
        }

        [Test]
        public void UserCanDeleteAccessibleItem()
        {
            ContentItem root = CreateUserAndItem();
            mocks.ReplayAll();

            deleting.Raise(persister, new CancellableItemEventArgs(root));
        }

        [Test]
        public void UserCannotDeleteInaccessibleItem()
        {
            ContentItem root = SetupNormalUserAndCreateRestrictedItem();
            mocks.ReplayAll();

            ExceptionAssert.Throws<PermissionDeniedException>(delegate
            {
                deleting.Raise(persister, new CancellableItemEventArgs(root));
            });
        }

        [Test]
        public void UserCanMoveAccessibleItem()
        {
            ContentItem source = CreateUserAndItem();
            ContentItem destination = CreateOneItem<Items.SecurityPage>(2, "accessible page", null);
            mocks.ReplayAll();

            moving.Raise(persister, new CancellableDestinationEventArgs(source, destination));
        }

        [Test]
        public void UserCannotMoveInaccessibleItem()
        {
            ContentItem source = SetupNormalUserAndCreateRestrictedItem();
            ContentItem destination = CreateOneItem<Items.SecurityPage>(2, "accessible page", null);
            mocks.ReplayAll();

            ExceptionAssert.Throws<PermissionDeniedException>(delegate
            {
                moving.Raise(persister, new CancellableDestinationEventArgs(source, destination));
            });
        }

        [Test]
        public void UserCannotMoveToInaccessibleItem()
        {
            ContentItem destination = SetupNormalUserAndCreateRestrictedItem();
            ContentItem source = CreateOneItem<Items.SecurityPage>(2, "accessible page", null);
            
            mocks.ReplayAll();
            ExceptionAssert.Throws<PermissionDeniedException>(delegate
            {
                moving.Raise(persister, new CancellableDestinationEventArgs(source, destination));
            });
        }

        [Test]
        public void UserCanCopyAccessibleItem()
        {
            ContentItem source = CreateUserAndItem();
            ContentItem destination = CreateOneItem<Items.SecurityPage>(2, "accessible page", null);
            mocks.ReplayAll();

            copying.Raise(persister, new CancellableDestinationEventArgs(source, destination));
        }

        [Test]
        public void UserCannotCopyInaccessibleItem()
        {
            ContentItem source = SetupNormalUserAndCreateRestrictedItem();
            ContentItem destination = CreateOneItem<Items.SecurityPage>(2, "accessible page", null);
            mocks.ReplayAll();

            ExceptionAssert.Throws<PermissionDeniedException>(delegate
            {
                copying.Raise(persister, new CancellableDestinationEventArgs(source, destination));
            });
        }

        [Test]
        public void UserCannotCopyToInaccessibleItem()
        {
            ContentItem destination = SetupNormalUserAndCreateRestrictedItem();
            ContentItem source = CreateOneItem<Items.SecurityPage>(2, "accessible page", null);
            mocks.ReplayAll();

            ExceptionAssert.Throws<PermissionDeniedException>(delegate
            {
                copying.Raise(persister, new CancellableDestinationEventArgs(source, destination));
            });
        }

        [Test]
        public void CanChange_AdminUser()
        {
            IPrincipal user = CreatePrincipal("AverageJoe");
            mocks.ReplayAll();

            security.Administrators.Users = new[] { "AverageJoe" };

            Assert.IsTrue(security.IsAdmin(user), "User wasn't admin.");
        }

        [Test]
        public void CanHave_MoreThanOne_Admin()
        {
            IPrincipal maria = CreatePrincipal("SpecialMaria");
            IPrincipal bill = CreatePrincipal("SmallBill");
            mocks.ReplayAll();

            security.Administrators.Users = new[] { "AverageJoe", "SpecialMaria", "SmallBill" };

            Assert.IsTrue(security.IsAdmin(maria), "User wasn't admin.");
            Assert.IsTrue(security.IsAdmin(bill), "User wasn't admin.");
        }

        [Test]
        public void CanChange_AdminRoles()
        {
            IPrincipal user = CreatePrincipal("AverageJoe", "AverageUser", "FriendlyUser");
            mocks.ReplayAll();

            security.Administrators.Roles = new[] { "FriendlyUser" };

            Assert.IsTrue(security.IsAdmin(user), "User wasn't admin.");
        }

        [Test]
        public void CanChange_EditorUser()
        {
            IPrincipal user = CreatePrincipal("AverageJoe");
            mocks.ReplayAll();

            security.Editors.Users = new[] { "AverageJoe" };

            Assert.IsTrue(security.IsEditor(user), "User wasn't editor.");
        }

        [Test]
        public void CanChange_EditorRoles()
        {
            IPrincipal user = CreatePrincipal("AverageJoe", "GhostWhisperer", "SiteSeeker");
            mocks.ReplayAll();

            security.Editors.Roles = new[] { "GhostWhisperer" };

            Assert.IsTrue(security.IsEditor(user), "User wasn't editor.");
        }

        [Test]
        public void CanConfigure_Permissions()
        {
            PermissionElement element = new PermissionElement { Dynamic = true };

            var map = element.ToPermissionMap(Permission.Administer, new[] {"role"}, new[] {"user"});

            Assert.That(map.Permissions, Is.EqualTo(Permission.Administer));
            Assert.That(map.Roles.Length, Is.EqualTo(1));
            Assert.That(map.Roles[0], Is.EqualTo("role"));
            Assert.That(map.Users.Length, Is.EqualTo(1));
            Assert.That(map.Users[0], Is.EqualTo("user"));
        }

        [Test]
        public void CanConfigure_DynamiPermissions()
        {
            PermissionElement element = new PermissionElement {Dynamic = true};

            var map = element.ToPermissionMap(Permission.Administer, null, null);

            Assert.That(map, Is.TypeOf(typeof(DynamicPermissionMap)));
        }

        [Test]
        public void ReadPermission_CanBeCopied()
        {
            var roles = new string[] { "Permitted" };
            ContentItem parent = CreateOneItem<Items.SecurityPage>(0, "parent", null);
            DynamicPermissionMap.SetRoles(parent, Permission.Read, roles);
            
            ContentItem child = CreateOneItem<Items.SecurityPage>(0, "child", parent);
            security.CopyPermissions(parent, child);

            bool isPermitted = DynamicPermissionMap.IsPermitted(roles[0], child, Permission.Read);
            Assert.That(isPermitted, Is.True);
        }

        [TestCase(Permission.None)]
        [TestCase(Permission.Read)]
        [TestCase(Permission.Write)]
        [TestCase(Permission.Publish)]
        [TestCase(Permission.Administer)]
        [TestCase(Permission.ReadWrite)]
        [TestCase(Permission.ReadWritePublish)]
        [TestCase(Permission.Full)]
        public void Permissions_AreCopied(Permission permission)
        {
            var roles = new string[] { "Permitted" };
            ContentItem parent = CreateOneItem<Items.SecurityPage>(0, "parent", null);
            DynamicPermissionMap.SetRoles(parent, permission, roles);

            ContentItem child = CreateOneItem<Items.SecurityPage>(0, "child", parent);
            security.CopyPermissions(parent, child);

            bool isPermitted = DynamicPermissionMap.IsPermitted(roles[0], child, permission);
            Assert.That(isPermitted, Is.True);
        }

        private ContentItem CreateUserAndItem()
        {
            Expect.On(context).Call(context.User).Return(CreatePrincipal("JustAnyGuy")).Repeat.Any();
            mocks.Replay(context);

            ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
            return root;
        }

        private ContentItem SetupNormalUserAndCreateRestrictedItem()
        {
            Expect.On(context).Call(context.User).Return(CreatePrincipal("JustAnyGuy")).Repeat.Any();
            mocks.Replay(context);

            ContentItem root = CreateOneItem<Items.SecurityPage>(1, "root", null);
            DynamicPermissionMap.SetRoles(root, Permission.Read, "Editors");
            return root;
        }
    }
}
