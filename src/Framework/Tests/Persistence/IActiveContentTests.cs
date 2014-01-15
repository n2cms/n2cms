using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Tests.Persistence.Definitions;
using Shouldly;

namespace N2.Tests.Persistence
{
    [TestFixture]
    public class IActiveContentTests
    {
        private ContentPersister persister;
        private ActiveContentItem item;

        [SetUp]
        public void SetUp()
        {
            persister = TestSupport.SetupFakePersister();
            item = new ActiveContentItem();
        }

        [Test]
        public void Save_InvokesActiveContent()
        {
            persister.Save(item);

            item.Actions.Single().ShouldBe("Save");
        }

        [Test]
        public void Delete_InvokesActiveContent()
        {
            persister.Delete(item);

            item.Actions.Single().ShouldBe("Delete");
        }

        [Test]
        public void Move_InvokesActiveContent()
        {
            var newParent = new ActiveContentItem();
            persister.Move(item, newParent);

            item.Actions.Single().ShouldBe("MoveTo " + newParent);
        }

        [Test]
        public void Copy_InvokesActiveContent()
        {
            var newParent = new ActiveContentItem();
            persister.Copy(item, newParent);

            item.Actions.Single().ShouldBe("CopyTo " + newParent);
        }
    }
}
