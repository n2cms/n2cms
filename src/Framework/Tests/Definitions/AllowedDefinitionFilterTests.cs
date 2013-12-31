using System.Collections.Generic;
using System.Linq;
using N2.Definitions;
using N2.Integrity;
using N2.Tests.Definitions.Items;
using NUnit.Framework;
using Rhino.Mocks;

namespace N2.Tests.Definitions
{
    [TestFixture]
    public class AllowedDefinitionFilterTests
    {
        IDefinitionManager definitions;
        ItemDefinition parentDefinition;
        ItemDefinition childDefinition;

        [SetUp]
        public void SetUp()
        {
            parentDefinition = new ItemDefinition(typeof(DefinitionControllingParent)) { TemplateKey = "List" };
            childDefinition = new ItemDefinition(typeof(DefinitionOppressedChild)) { TemplateKey = "Wide" };

            definitions = MockRepository.GenerateStub<IDefinitionManager>();
            definitions.Expect(d => d.GetDefinitions()).Return(new[] { parentDefinition, childDefinition }).Repeat.Any();
        }

        [Test]
        public void RestrictChildrenAttribute_AllowsChildren_WithCorrectTemplateName()
        {
            var rca = new RestrictChildrenAttribute(typeof(DefinitionOppressedChild)) { TemplateNames = new[] { "Wide" } };
            rca.Refine(parentDefinition, new List<ItemDefinition>());

            var allowedChildren = parentDefinition.GetAllowedChildren(definitions, new DefinitionControllingParent()).ToList();

            Assert.That(allowedChildren.Single().ItemType, Is.EqualTo(typeof(DefinitionOppressedChild)));
        }

        [Test]
        public void RestrictChildrenAttribute_DisallowsChildren_WithIncorrectTemplateName()
        {
            var rca = new RestrictChildrenAttribute(typeof(DefinitionOppressedChild)) { TemplateNames = new[] { "Narrow" } };
            rca.Refine(parentDefinition, new List<ItemDefinition>());

            var allowedChildren = parentDefinition.GetAllowedChildren(definitions, new DefinitionControllingParent()).ToList();

            Assert.That(allowedChildren.Count(), Is.EqualTo(0));
        }

        [Test]
        public void RestrictParentsAttribute_AllowsChildren_WithCorrectParentTemplateName()
        {
            var rca = new RestrictParentsAttribute(typeof(DefinitionControllingParent)) { TemplateKeys = new[] { "List" } };
            rca.Refine(childDefinition, new List<ItemDefinition>());

            var allowedChildren = parentDefinition.GetAllowedChildren(definitions, new DefinitionControllingParent()).ToList();

            Assert.That(allowedChildren.Any(i => i.ItemType == typeof(DefinitionOppressedChild)));
        }

        [Test]
        public void RestrictParentsAttribute_DisallowsChildren_WithIncorrectParentTemplateName()
        {
            var rca = new RestrictParentsAttribute(typeof(DefinitionControllingParent)) { TemplateKeys = new[] { "Single" } };
            rca.Refine(childDefinition, new List<ItemDefinition>());

            var allowedChildren = parentDefinition.GetAllowedChildren(definitions, new DefinitionControllingParent()).ToList();

            Assert.That(!allowedChildren.Any(i => i.ItemType == typeof(DefinitionOppressedChild)));
        }
    }
}
