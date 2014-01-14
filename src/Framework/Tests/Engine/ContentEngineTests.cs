using N2.Engine;
using N2.Engine.Castle;
using N2.Engine.MediumTrust;
using N2.Persistence.NH;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Engine
{
    [TestFixture]
    public class WindsorContentEngine : ContentEngineTests
    {
        [SetUp]
        public void SetUp()
        {
            engine = new ContentEngine(new WindsorServiceContainer(), EventBroker.Instance, new ContainerConfigurer());
        }
    }

    [TestFixture]
    public class MediumTrustContentEngine : ContentEngineTests
    {
        [SetUp]
        public void SetUp()
        {
            engine = new ContentEngine(new MediumTrustServiceContainer(), EventBroker.Instance, new ContainerConfigurer());
        }
    }

    public abstract class ContentEngineTests
    {
        protected ContentEngine engine;

        [Test]
        public void CanCreateWithEventBroker()
        {
            engine = new ContentEngine();
        }

        [Test]
        public void CanAssignRootAndStartPageID()
        {
            var host = engine.Resolve<IHost>();
            Assert.That(host.DefaultSite.RootItemID, Is.EqualTo(2));
            Assert.That(host.DefaultSite.StartPageID, Is.EqualTo(3));
        }

        [Test]
        public void CanAssignDefaultNHibernateProperties()
        {
            var cb = engine.Resolve<ConfigurationBuilder>();
            Assert.That(cb.Properties.Count, Is.GreaterThan(0));
        }

        [Test]
        public void SitesAreSetup()
        {
            var host = engine.Resolve<IHost>();
            Assert.That(host.Sites.Count, Is.EqualTo(3));
            Assert.That(host.Sites[0].Authority, Is.EqualTo("alpha.localhost.com"));
            Assert.That(host.Sites[0].StartPageID, Is.EqualTo(4));
        }
    }
}
