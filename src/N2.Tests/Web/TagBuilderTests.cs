using NUnit.Framework;
using N2.Web;

namespace N2.Tests.Web
{
    [TestFixture]
    public class TagBuilderTests
    {
        [Test]
        public void TagWithNullContent_RendersAsNull()
        {
            var tb = new TagBuilder("a", null);
            string html = tb;
            Assert.That(html, Is.Null);
        }
        [Test]
        public void TagWithEmptyContent_IsRendered()
        {
            var tb = new TagBuilder("a", string.Empty);
            string html = tb;
            Assert.That(html, Is.EqualTo("<a/>"));
        }
    }
}
