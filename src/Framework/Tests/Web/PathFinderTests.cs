using NUnit.Framework;

namespace N2.Tests.Web
{
    [TestFixture]
    public class PathFinderTests : ItemTestsBase
    {
        [Test]
        public void FindsConventionTemplate()
        {
            var item = CreateOneItem<Items.ConventionTemplatePage>(1, "item", null);

            var path = item.FindPath("");
            Assert.That(path.TemplateUrl, Is.EqualTo("~/Tests/UI/Views/ConventionTemplatePage.aspx"));
        }

        [Test]
        public void CanOverride_ConventionTemplateName()
        {
            var item = CreateOneItem<Items.ConventionTemplatePage2>(1, "item", null);

            var path = item.FindPath("");
            Assert.That(path.TemplateUrl, Is.EqualTo("~/Tests/UI/Views/SomeOtherTemplate.aspx"));
        }
    }
}
