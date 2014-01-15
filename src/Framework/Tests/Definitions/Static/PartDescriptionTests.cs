using N2.Definitions;
using N2.Definitions.Static;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Definitions.Static
{
    [TestFixture]
    public class PartDescriptionTests
    {
        ItemDefinition definition = new DefinitionMap().GetOrCreateDefinition(typeof(DescribablePart));
        ContentItem item = new DescribablePart();

        [Test]
        public void Description_IsNotPage()
        {
            Assert.That(definition.IsPage, Is.False);
        }

        [Test]
        public void Instance_IsNotPage()
        {
            Assert.That(item.IsPage, Is.False);
        }

        [Test]
        public void Definition_HasPartIconUrl()
        {
            Assert.That(definition.IsPage, Is.False);
        }

        [Test]
        public void Definition_HasDefinitionIconUrl()
        {
            Assert.That(definition.IconUrl, Is.EqualTo("/N2/Resources/icons/page_white(description).png"));
        }

        [Test]
        public void Instance_HasPageIconUrl()
        {
            Assert.That(item.IconUrl, Is.EqualTo("/N2/Resources/icons/page_white(iconUrl).png"));
        }

        [Test]
        public void Instance_GetsTemplateUrl_FromDefinition_WhenDefined()
        {
            PathData path = item.FindPath(PathData.DefaultAction);
            Assert.That(path.TemplateUrl, Is.EqualTo("~/My/Template.ascx"));
        }

        [Test]
        public void Definition_HasDisplayables()
        {
            Assert.That(definition.Displayables.Count, Is.GreaterThanOrEqualTo(16));
        }
    }
}
