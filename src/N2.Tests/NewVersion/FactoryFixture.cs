using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace N2.Tests.NewVersion
{
    [TestFixture]
    public class FactoryFixture
    {
        N2.Engine.CmsEngine engine;

        [SetUp]
        public virtual void SetUp()
        {
            engine = new N2.Engine.CmsEngine();
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

		//[TearDown]
		//public virtual void TearDown()
		//{
		//    factory = null;
		//}
    }
}
