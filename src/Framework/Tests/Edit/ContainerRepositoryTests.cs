using System.Linq;
using N2.Edit;
using N2.Edit.Workflow;
using N2.Security;
using N2.Tests.Fakes;
using N2.Web;
using N2.Edit.Settings;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Persistence.Finder;
using N2.Persistence;
using N2.Persistence.Proxying;

namespace N2.Tests.Edit
{
    [TestFixture]
    public class ContainerRepositoryTests : N2.Tests.Persistence.DatabasePreparingBase
    {
        ContentItem root;
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            root = CreateRoot("root", "root");
        }

        [Test]
        public void ContainerIsCreated()
        {
            var r = engine.Resolve<ContainerRepository<Items.NormalPage>>();
            var container = r.GetOrCreate(root, (c) => { });

            Assert.That(container, Is.Not.Null);
        }

        [Test]
        public void ExistingContainer_IsRetrieved()
        {
            var r = engine.Resolve<ContainerRepository<Items.NormalPage>>();
            var container = r.GetOrCreate(root, (c) => { });
            var container2 = r.GetOrCreate(root, (c) => { });

            Assert.That(container, Is.SameAs(container2));
        }

        [Test]
        public void Name_CanBeSet()
        {
            var r = engine.Resolve<ContainerRepository<Items.NormalPage>>();
            var container = r.GetOrCreate(root, (c) => { }, name: "hello");

            Assert.That(container.Name, Is.EqualTo("hello"));
        }

        [Test]
        public void ExistingContainer_WithSameName_IsRetrieved()
        {
            var r = engine.Resolve<ContainerRepository<Items.NormalPage>>();
            var container = r.GetOrCreate(root, (c) => { }, name: "hello");
            var container2 = r.GetOrCreate(root, (c) => { }, name: "hello");

            Assert.That(container, Is.SameAs(container2));
        }

        [Test]
        public void Container_WithDifferentName_IsCreated()
        {
            var r = engine.Resolve<ContainerRepository<Items.NormalPage>>();
            var container = r.GetOrCreate(root, (c) => { }, name: "hello");
            var container2 = r.GetOrCreate(root, (c) => { }, name: "hello2");

            Assert.That(container, Is.Not.SameAs(container2));
        }
    }
}
