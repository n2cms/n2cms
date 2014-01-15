using System.Security.Principal;
using N2.Security;
using N2.Tests.Security.Items;
using NUnit.Framework;

namespace N2.Tests.Security
{
    [TestFixture]
    public class DynamicPermissionMapTests : ItemTestsBase
    {
        IPrincipal user;
        ContentItem item;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            user = CreatePrincipal("username", "rolename");
            item = CreateOneItem<Items.SecurityPage>(1, "item", null);
        }

        [TestCase(Permission.None, true)]
        [TestCase(Permission.Read, true)]
        [TestCase(Permission.Write, true)]
        [TestCase(Permission.ReadWrite, true)]
        [TestCase(Permission.Publish, false)]
        [TestCase(Permission.ReadWritePublish, false)]
        [TestCase(Permission.Administer, false)]
        [TestCase(Permission.Full, false)]
        public void UsesMappedPermission_WhenItemCollection_IsNull(Permission permission, bool expectedResult)
        {
            var map = new DynamicPermissionMap { Permissions = Permission.ReadWrite, Roles = new[] { "rolename" } };

            Assert.That(map.Authorizes(user, item, permission), Is.EqualTo(expectedResult));
        }

        [Test]
        public void CanAdd_Role_ToItem()
        {
            DynamicPermissionMap.SetRoles(item, Permission.Write, new[] {"rolename"});

            var collection = item.GetDetailCollection(DynamicPermissionMap.AuthorizedRolesPrefix + Permission.Write, false);
            Assert.That(collection.Count, Is.EqualTo(1));
            Assert.That(collection[0], Is.EqualTo("rolename"));
        }

        [Test]
        public void CanAdd_MultipleRoles_ToItem()
        {
            DynamicPermissionMap.SetRoles(item, Permission.Write, new[] { "rolename", "rolename2" });

            var collection = item.GetDetailCollection(DynamicPermissionMap.AuthorizedRolesPrefix + Permission.Write, false);
            Assert.That(collection.Count, Is.EqualTo(2));
            Assert.That(collection.Contains("rolename"));
            Assert.That(collection.Contains("rolename2"));
        }

        [Test]
        public void CanAdd_MultiplePermissions_ToItem()
        {
            DynamicPermissionMap.SetRoles(item, Permission.ReadWritePublish, new[] { "rolename" });

            var writeCollection = item.GetDetailCollection(DynamicPermissionMap.AuthorizedRolesPrefix + Permission.Write, false);
            Assert.That(writeCollection.Count, Is.EqualTo(1));
            Assert.That(writeCollection.Contains("rolename"));

            var publishCollection = item.GetDetailCollection(DynamicPermissionMap.AuthorizedRolesPrefix + Permission.Publish, false);
            Assert.That(publishCollection.Count, Is.EqualTo(1));
            Assert.That(publishCollection.Contains("rolename"));
        }

        [Test]
        public void ReadRoles_AreNotStored_AsDetailCollections()
        {
            DynamicPermissionMap.SetRoles(item, Permission.ReadWritePublish, new[] { "rolename" });

            var readCollection = item.GetDetailCollection(DynamicPermissionMap.AuthorizedRolesPrefix + Permission.Read, false);
            Assert.That(readCollection, Is.Null);
        }

        [TestCase(Permission.Read)]
        [TestCase(Permission.Write)]
        [TestCase(Permission.ReadWrite)]
        [TestCase(Permission.Publish)]
        [TestCase(Permission.ReadWritePublish)]
        [TestCase(Permission.Administer)]
        [TestCase(Permission.Full)]
        public void CanRemoveRoles(Permission permission)
        {
            DynamicPermissionMap.SetRoles(item, permission, new[] { "rolename1", "rolename2" });
            DynamicPermissionMap.SetRoles(item, permission, new[] { "rolename1", "rolename3" });

            bool is1 = DynamicPermissionMap.IsPermitted("rolename1", item, permission);
            bool is2 = DynamicPermissionMap.IsPermitted("rolename2", item, permission);
            bool is3 = DynamicPermissionMap.IsPermitted("rolename3", item, permission);

            Assert.That(is1, Is.True);
            Assert.That(is2, Is.False);
            Assert.That(is3, Is.True);
        }

        [TestCase(Permission.Read)]
        [TestCase(Permission.Write)]
        [TestCase(Permission.ReadWrite)]
        [TestCase(Permission.Publish)]
        [TestCase(Permission.ReadWritePublish)]
        [TestCase(Permission.Administer)]
        [TestCase(Permission.Full)]
        public void CanSet_AllRoles(Permission permission)
        {
            DynamicPermissionMap.SetRoles(item, permission, new[] { "rolename1", "rolename2" });
            DynamicPermissionMap.SetAllRoles(item, permission);

            bool isAllRoles = DynamicPermissionMap.IsAllRoles(item, permission);

            Assert.That(isAllRoles, Is.True);
        }

        [Test]
        public void CanAdd_AllPermissions_ToItem()
        {
            DynamicPermissionMap.SetRoles(item, Permission.ReadWrite, new[] { "rolename" });
            
            DynamicPermissionMap.SetAllRoles(item, Permission.ReadWrite);

            var readCollection = item.GetDetailCollection(DynamicPermissionMap.AuthorizedRolesPrefix + Permission.Read, false);
            var writeCollection = item.GetDetailCollection(DynamicPermissionMap.AuthorizedRolesPrefix + Permission.Write, false);
            Assert.That(readCollection, Is.Null);
            Assert.That(writeCollection, Is.Null);
        }

        [TestCase(Permission.None)]
        [TestCase(Permission.Read)]
        public void Allows_DefinedItemRoles(Permission allowedPermission)
        {
            var map = new DynamicPermissionMap { Permissions = Permission.ReadWrite };
            DynamicPermissionMap.SetRoles(item, Permission.ReadWrite, "rolename");
            
            bool isAuthorized = map.Authorizes(user, item, allowedPermission);

            Assert.That(isAuthorized, Is.True);
        }

        [TestCase(Permission.Write)]
        [TestCase(Permission.ReadWrite)]
        [TestCase(Permission.Publish)]
        [TestCase(Permission.ReadWritePublish)]
        [TestCase(Permission.Administer)]
        [TestCase(Permission.Full)]
        public void Denies_NonDefinedItemRoles(Permission deniedPermission)
        {
            var map = new DynamicPermissionMap { Permissions = Permission.ReadWrite };
            DynamicPermissionMap.SetRoles(item, Permission.ReadWrite, "somerolename");

            bool isAuthorized = map.Authorizes(user, item, deniedPermission);

            Assert.That(isAuthorized, Is.False, deniedPermission + " is allowed which it shouldn't have been.");
        }

        [TestCase(Permission.Administer)]
        [TestCase(Permission.Full)]
        public void DoesntMap_BeyondMapped_ReadWritePublishPermissios(Permission expectedPermission)
        {
            var map = new DynamicPermissionMap { Permissions = Permission.ReadWritePublish };

            Assert.That(map.MapsTo(expectedPermission), Is.False);
            Assert.That(map.Authorizes(null, null, expectedPermission), Is.False);
        }

        [TestCase(Permission.Publish)]
        [TestCase(Permission.ReadWritePublish)]
        [TestCase(Permission.Administer)]
        [TestCase(Permission.Full)]
        public void DoesntMap_BeyondMapped_ReadWritePermissios(Permission expectedPermission)
        {
            var map = new DynamicPermissionMap { Permissions = Permission.ReadWrite };

            Assert.That(map.MapsTo(expectedPermission), Is.False);
            Assert.That(map.Authorizes(null, null, expectedPermission), Is.False);
        }

        [TestCase(Permission.Write)]
        [TestCase(Permission.ReadWrite)]
        [TestCase(Permission.Publish)]
        [TestCase(Permission.ReadWritePublish)]
        [TestCase(Permission.Administer)]
        [TestCase(Permission.Full)]
        public void DoesntMap_BeyondMapped_ReadPermissios(Permission expectedPermission)
        {
            var map = new DynamicPermissionMap { Permissions = Permission.Read };

            Assert.That(map.MapsTo(expectedPermission), Is.False);
            Assert.That(map.Authorizes(null, null, expectedPermission), Is.False);
        }

        [Test]
        public void AllRoles_ArePermitted_ByDefault()
        {
            bool isPermitted = DynamicPermissionMap.IsPermitted("Somerole", item, Permission.Write);
            Assert.That(isPermitted, Is.True);
        }

        [Test]
        public void PermittedRoles_ArePermitted()
        {
            DynamicPermissionMap.SetRoles(item, Permission.Publish, "Somerole");

            bool isPermitted = DynamicPermissionMap.IsPermitted("Somerole", item, Permission.Publish);
            Assert.That(isPermitted, Is.True);
        }

        [Test]
        public void UnPermittedRoles_AreNotPermitted_WhenRolesAreSet()
        {
            DynamicPermissionMap.SetRoles(item, Permission.Administer, "AddedRole");

            bool isPermitted = DynamicPermissionMap.IsPermitted("Somerole", item, Permission.Administer);
            Assert.That(isPermitted, Is.False);
        }

        [Test]
        public void IsAllRoles_OnReadPermission_DefaultsToTrue()
        {
            bool isAllRoles = DynamicPermissionMap.IsAllRoles(item, Permission.Read);

            Assert.That(isAllRoles, Is.True);
        }

        [Test]
        public void IsAllRoles_RevertsTo_UserPermissions()
        {
            var map = new DynamicPermissionMap {Permissions = Permission.ReadWrite, Roles = new[] {"Writers"}};
            user = CreatePrincipal("joe writer", "Writers");

            bool isAuthorizedForPublish = map.Authorizes(user, item, Permission.Publish);

            Assert.That(isAuthorizedForPublish, Is.False);
        }

        [Test]
        public void IsAllRoles_OnReadPermission_UsesAuthorizedRoles()
        {
            DynamicPermissionMap.SetRoles(item, Permission.Read, "Somerole");

            bool isAllRoles = DynamicPermissionMap.IsAllRoles(item, Permission.Read);
            
            Assert.That(isAllRoles, Is.False);
        }

        [Test]
        public void IsAllRoles_OnWritePermission_DefaultsToTrue()
        {
            bool isAllRoles = DynamicPermissionMap.IsAllRoles(item, Permission.Write);
            
            Assert.That(isAllRoles, Is.True);
        }

        [Test]
        public void IsAllRoles_OnWritePermission_UsesDetailCollection()
        {
            DynamicPermissionMap.SetRoles(item, Permission.Write, "Somerole");

            bool isAllRoles = DynamicPermissionMap.IsAllRoles(item, Permission.Write);
            
            Assert.That(isAllRoles, Is.False);
        }


        [Test]
        public void Ignores_UnmappedRoles()
        {
            var map = new DynamicPermissionMap { Permissions = Permission.ReadWrite, Roles = new[] { "rolename" } };

            var isAuthorized = map.Authorizes(user, item, Permission.Publish);

            Assert.That(isAuthorized, Is.False);
        }

        [Test]
        public void Admin_IsAuthorized_ForWrite_WhileItemDisallowsRead()
        {
            var map = new DynamicPermissionMap(Permission.Full, new[] { "Administrators" }, new[] { "admin" });
            
            var item = new SecurityPage();
            DynamicPermissionMap.SetRoles(item, Permission.Read, new[] {"Administrators"});
            IPrincipal adminUser = CreatePrincipal("admin");
            
            bool isAuthorized = map.Authorizes(adminUser, item, Permission.Write);

            Assert.That(isAuthorized, Is.True);
        }

        [TestCase(Permission.None)]
        [TestCase(Permission.Read)]
        [TestCase(Permission.Write)]
        [TestCase(Permission.ReadWrite)]
        [TestCase(Permission.Publish)]
        [TestCase(Permission.ReadWritePublish)]
        [TestCase(Permission.Administer)]
        [TestCase(Permission.Full)]
        public void ChangingPermissions_ModifiesAlteredPermissions(Permission permission)
        {
            var map = new DynamicPermissionMap();
            DynamicPermissionMap.SetRoles(item, permission, "rolename");
            
            Assert.That(item.AlteredPermissions & permission, Is.EqualTo(permission));
        }

        [Test]
        public void ResettingPermissions_RestoresAlteredPermissions()
        {
            var map = new DynamicPermissionMap();
            DynamicPermissionMap.SetRoles(item, Permission.Read, "rolename");
            DynamicPermissionMap.SetRoles(item, Permission.Read);

            Assert.That(item.AlteredPermissions, Is.EqualTo(Permission.None));
        }

        [Test]
        public void SettingPermissions_AddsDetailCollection()
        {
            var map = new DynamicPermissionMap();
            DynamicPermissionMap.SetRoles(item, Permission.Write, "rolename");

            Assert.That(item.GetDetailCollection(DynamicPermissionMap.AuthorizedRolesPrefix + Permission.Write, false), Is.Not.Null);
        }

        [Test]
        public void ResettingPermissions_RemovesDetailCollection()
        {
            var map = new DynamicPermissionMap();
            DynamicPermissionMap.SetRoles(item, Permission.Write, "rolename");
            DynamicPermissionMap.SetRoles(item, Permission.Write);

            Assert.That(item.GetDetailCollection(DynamicPermissionMap.AuthorizedRolesPrefix + Permission.Write, false), Is.Null);
        }
    }
}
