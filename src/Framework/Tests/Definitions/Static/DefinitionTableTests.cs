using N2.Definitions;
using N2.Definitions.Static;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Definitions.Static
{
    [TestFixture]
    public class DefinitionTableTests
    {
        ItemDefinition definition = new DefinitionMap().GetOrCreateDefinition(typeof(DescribablePage));
        ContentItem page = new DescribablePage();

        [Test]
        public void Definition_IsPage()
        {
            Assert.That(definition.IsPage, Is.True);
        }

        [Test]
        public void Instance_IsPage()
        {
            Assert.That(page.IsPage, Is.True);
        }

        [Test]
        public void Definition_HasPageIconUrl()
        {
            Assert.That(definition.IconUrl, Is.EqualTo("/N2/Resources/icons/page(description).png"));
        }

        [Test]
        public void Instance_HasPageIconUrl()
        {
            Assert.That(page.IconUrl, Is.EqualTo("/N2/Resources/icons/page(iconUrl).png"));
        }

        [Test]
        public void Instance_DoesntGet_TemplateUrl_FromDefinition_WhenEmpty()
        {
            PathData path = page.FindPath(PathData.DefaultAction);
            Assert.That(path.TemplateUrl, Is.EqualTo("~/Default.aspx"));
        }

        [Test]
        public void Definition_HasDisplayables()
        {
            Assert.That(definition.Displayables.Count, Is.GreaterThanOrEqualTo(16));
        }
    }
}
