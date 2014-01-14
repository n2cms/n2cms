using System;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;

namespace N2.Tests.Installation
{
    [TestFixture]
    public class InstallFixture : PersistenceAwareBase
    {
        [Test]
        public virtual void CanCreateDatabaseSchema()
        {
            CreateDatabaseSchema();
            PersistableItem testItem = new PersistableItem();
            engine.Persister.Save(testItem);
            Assert.AreEqual(1, testItem.ID);
        }

        [Test]
        public virtual void CanDropDatabaseSchema()
        {
            CreateDatabaseSchema();
            DropDatabaseSchema();

            PersistableItem testItem = new PersistableItem();
            ExceptionAssert.Throws<Exception>(
                delegate
                    {
                        engine.Persister.Save(testItem);
                    });
        }
    }
}
