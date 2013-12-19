using System.Collections.Generic;
using System.Linq;
using N2.Definitions;
using N2.Integrity;
using N2.Tests.Definitions.Items;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Persistence;
using N2.Definitions.Static;
using N2.Persistence.Proxying;
using N2.Configuration;

namespace N2.Tests.Definitions
{
    [TestFixture]
    public class RestrictCardinalityTests
    {
        DefinitionManager definitions;

        [SetUp]
        public void SetUp()
        {
            var changer = new N2.Edit.Workflow.StateChanger();
            var activator = new ContentActivator(changer, MockRepository.GenerateStrictMock<IItemNotifier>(), new EmptyProxyFactory());
            DefinitionBuilder builder = new DefinitionBuilder(new DefinitionMap(), new Fakes.FakeTypeFinder(typeof(RestrictsChildWithCardinality), typeof(ChildWithCardinality)), new TransformerBase<IUniquelyNamed>[0], new EngineSection { Definitions = new DefinitionCollection { DefineUnattributedTypes = true } });
            definitions = new DefinitionManager(new[] { new DefinitionProvider(builder) }, activator, changer, new DefinitionMap());
        }

        [Test]
        public void OneChildIsAllowed()
        {
            var allowed = definitions.GetAllowedChildren(new RestrictsChildWithCardinality()).Single();

            Assert.That(allowed.ItemType, Is.EqualTo(typeof(ChildWithCardinality)));
        }

        [Test]
        public void SecondChildIsDisallowed()
        {
            var item = new RestrictsChildWithCardinality();
            item.Children.Add(new ChildWithCardinality());
            var allowedCount = definitions.GetAllowedChildren(item).Count();

            Assert.That(allowedCount, Is.EqualTo(0));
        }
    }
}
