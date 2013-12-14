using N2.Engine;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Tests.Fakes;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace N2.Tests
{
    public abstract class PersistenceAwareBase : ItemTestsBase
    {
        protected IEngine engine;
        protected PersistenceTestHelper helper;

        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
            helper = new PersistenceTestHelper();
            helper.TestFixtureSetUp();
            engine = helper.Engine;
        }

        protected virtual ContentEngine CreateEngine()
        {
            return new ContentEngine();
        }

        [TearDown]
        public override void TearDown()
        {
            helper.TearDown();
            base.TearDown();
        }

        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown()
        {
            helper.TestFixtureTearDown();
        }

        protected virtual void CreateDatabaseSchema()
        {
            helper.CreateDatabaseSchema();
        }

        protected virtual void DropDatabaseSchema()
        {
            helper.DropDatabaseSchema();
        }
    }
}
