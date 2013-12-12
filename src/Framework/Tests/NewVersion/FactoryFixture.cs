using NUnit.Framework;

namespace N2.Tests.NewVersion
{
    [TestFixture]
    public class FactoryFixture
    {
        N2.Engine.ContentEngine engine;

        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
            engine = new N2.Engine.ContentEngine();
        }

        [Test]
        public void DefinitionsIsNotNull()
        {
            Assert.IsNotNull(engine.Definitions);
        }

        [Test]
        public void IntegrityIsNotNull()
        {
            Assert.IsNotNull(engine.IntegrityManager);
        }

        [Test]
        public void PersisterIsNotNull()
        {
            Assert.IsNotNull(engine.Persister);
        }

        [Test]
        public void UrlParserIsNotNull()
        {
            Assert.IsNotNull(engine.UrlParser);
        }

        [Test]
        public void SecurityIsNotNull()
        {
            Assert.IsNotNull(engine.SecurityManager);
        }

        [Test]
        public void Container_IsNotNull()
        {
            Assert.IsNotNull(engine.Container);
        }

        [Test]
        public void Content_IsNotNull()
        {
            Assert.IsNotNull(engine.Content);
        }

        [Test]
        public void ManagementPaths_IsNotNull()
        {
            Assert.IsNotNull(engine.ManagementPaths);
        }

        [Test]
        public void DatabaseConfig_IsNotNull()
        {
            Assert.IsNotNull(engine.Config.Sections.Database);
        }

        [Test]
        public void EngineConfig_IsNotNull()
        {
            Assert.IsNotNull(engine.Config.Sections.Engine);
        }

        [Test]
        public void ManagementConfig_IsNotNull()
        {
            Assert.IsNotNull(engine.Config.Sections.Management);
        }

        [Test]
        public void WebConfig_IsNotNull()
        {
            Assert.IsNotNull(engine.Config.Sections.Web);
        }
    }
}
