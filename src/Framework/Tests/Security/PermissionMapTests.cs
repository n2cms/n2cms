using System.Security.Principal;
using N2.Security;
using NUnit.Framework;

namespace N2.Tests.Security
{
    [TestFixture]
    public class PermissionMapTests : ItemTestsBase
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

        [TestCase(Permission.None)]
        [TestCase(Permission.Read)]
        [TestCase(Permission.Write)]
        [TestCase(Permission.ReadWrite)]
        [TestCase(Permission.Publish)]
        [TestCase(Permission.ReadWritePublish)]
        [TestCase(Permission.Administer)]
        [TestCase(Permission.Full)]
        public void CanMapFullPermission(Permission expectedPermission)
        {
            var map = new PermissionMap { Permissions = Permission.Full, Roles = new[] { "rolename" } };

            Assert.That(map.MapsTo(expectedPermission), Is.True);
            Assert.That(map.Authorizes(user, item, expectedPermission), Is.True);
        }

        [TestCase(Permission.None, true)]
        [TestCase(Permission.Read, true)]
        [TestCase(Permission.Write, true)]
        [TestCase(Permission.ReadWrite, true)]
        [TestCase(Permission.Publish, true)]
        [TestCase(Permission.ReadWritePublish, true)]
        [TestCase(Permission.Administer, false)]
        [TestCase(Permission.Full, false)]
        public void CanMapReadWritePublishPermission(Permission expectedPermission, bool expectedResult)
        {
            var map = new PermissionMap { Permissions = Permission.ReadWritePublish, Roles = new[] { "rolename" } };

            Assert.That(map.MapsTo(expectedPermission), Is.EqualTo(expectedResult));
            Assert.That(map.Authorizes(user, item, expectedPermission), Is.EqualTo(expectedResult));
        }

        [TestCase(Permission.None, true)]
        [TestCase(Permission.Read, true)]
        [TestCase(Permission.Write, true)]
        [TestCase(Permission.ReadWrite, true)]
        [TestCase(Permission.Publish, false)]
        [TestCase(Permission.ReadWritePublish, false)]
        [TestCase(Permission.Administer, false)]
        [TestCase(Permission.Full, false)]
        public void CanMapReadWritePermission(Permission expectedPermission, bool expectedResult)
        {
            var map = new PermissionMap { Permissions = Permission.ReadWrite, Roles = new[] { "rolename" } };

            Assert.That(map.MapsTo(expectedPermission), Is.EqualTo(expectedResult));
            Assert.That(map.Authorizes(user, item, expectedPermission), Is.EqualTo(expectedResult));
        }

        [TestCase(Permission.None, true)]
        [TestCase(Permission.Read, true)]
        [TestCase(Permission.Write, false)]
        [TestCase(Permission.ReadWrite, false)]
        [TestCase(Permission.Publish, false)]
        [TestCase(Permission.ReadWritePublish, false)]
        [TestCase(Permission.Administer, false)]
        [TestCase(Permission.Full, false)]
        public void CanMapReadPermission(Permission expectedPermission, bool expectedResult)
        {
            var map = new PermissionMap { Permissions = Permission.Read, Roles = new[] { "rolename" } };

            Assert.That(map.MapsTo(expectedPermission), Is.EqualTo(expectedResult));
            Assert.That(map.Authorizes(user, item, expectedPermission), Is.EqualTo(expectedResult));
        }

        [Test]
        public void CanClone()
        {
            PermissionMap original = new PermissionMap(Permission.ReadWrite, new string[] {"role1"}, new string[] {"user1"});
            PermissionMap cloned = original.Clone();

            Assert.That(original.Permissions, Is.EqualTo(cloned.Permissions));
            Assert.That(original.Users.Length, Is.EqualTo(cloned.Users.Length));
            Assert.That(original.Roles.Length, Is.EqualTo(cloned.Roles.Length));
            Assert.That(original.Users[0], Is.EqualTo(cloned.Users[0]));
            Assert.That(original.Roles[0], Is.EqualTo(cloned.Roles[0]));
        }
    }
}
