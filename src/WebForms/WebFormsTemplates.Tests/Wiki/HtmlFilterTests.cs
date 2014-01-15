using N2.Web;
using NUnit.Framework;

namespace N2.Templates.Tests.Wiki
{
    [TestFixture]
    public class HtmlFilterTests
    {
        public HtmlFilter filter = new HtmlFilter();

        [Test]
        public void DoesntFilter_TextWithoutTags()
        {
            string result = filter.FilterHtml("Hello world!");
            Assert.That(result, Is.EqualTo("Hello world!"));
        }

        [Test]
        public void DoesntFilter_Paragraphs()
        {
            string result = filter.FilterHtml("<p>Hello world!</p>");
            Assert.That(result, Is.EqualTo("<p>Hello world!</p>"));
        }

        [Test]
        public void DoesntFilter_Anchors()
        {
            string result = filter.FilterHtml("<p><a href=\"/wiki/hello.aspx\" title=\"click for greeting\">Hello world!</a></p>");
            Assert.That(result, Is.EqualTo("<p><a href=\"/wiki/hello.aspx\" title=\"click for greeting\">Hello world!</a></p>"));
        }

        [Test]
        public void DoesntFilter_Images()
        {
            string result = filter.FilterHtml("<p><img src=\"tussilago.jpg\" alt=\"flower\"/>Hello world!</p>");
            Assert.That(result, Is.EqualTo("<p><img src=\"tussilago.jpg\" alt=\"flower\"/>Hello world!</p>"));
        }

        [Test]
        public void Filters_UnsafeTags()
        {
            string result = filter.FilterHtml("<p>Hello world!<script>alert('gotcha!');</script></p>");
            Assert.That(result, Is.EqualTo("<p>Hello world!alert('gotcha!');</p>"));
        }

        [Test]
        public void Filters_Tags_WithOnClick()
        {
            string result = filter.FilterHtml("<p><a onclick=\"alert('gotcha!');\" href=\"/wiki/hello.aspx\">Hello world!</a></p>");
            Assert.That(result, Is.EqualTo("<p><a href=\"/wiki/hello.aspx\">Hello world!</a></p>"));
        }
    }
}
