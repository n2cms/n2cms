using System;
using N2.Web;
using NUnit.Framework;

namespace N2.Templates.Tests.Wiki
{
    [TestFixture]
    public class HaackTests
    {
        HtmlFilter Html = new HtmlFilter();
        
        [Test]
        public void NullHtml_ThrowsArgumentException()
        {
            try
            {
                Html.StripHtml(null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }
        }

        [Test]
        public void Html_WithEmptyString_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, Html.StripHtml(string.Empty));
        }

        [Test]
        public void Html_WithNoTags_ReturnsTextOnly()
        {
            string html = "This has no tags!";
            Assert.AreEqual(html, Html.StripHtml(html));
        }

        [Test]
        public void Html_WithOnlyATag_ReturnsEmptyString()
        {
            string html = "<foo>";
            Assert.AreEqual(string.Empty, Html.StripHtml(html));
        }

        [Test]
        public void Html_WithOnlyConsecutiveTags_ReturnsEmptyString()
        {
            string html = "<foo><bar><baz />";
            Assert.AreEqual(string.Empty, Html.StripHtml(html));
        }

        [Test]
        public void Html_WithTextBeforeTag_ReturnsText()
        {
            string html = "Hello<foo>";
            Assert.AreEqual("Hello", Html.StripHtml(html));
        }

        [Test]
        public void Html_WithTextAfterTag_ReturnsText()
        {
            string html = "<foo>World";
            Assert.AreEqual("World", Html.StripHtml(html));
        }

        [Test]
        public void Html_WithTextBetweenTags_ReturnsText()
        {
            string html = "<p><foo>World</foo></p>";
            Assert.AreEqual("World", Html.StripHtml(html));
        }

        [Test, Ignore]
        public void Html_WithClosingTagInAttrValue_StripsEntireTag()
        {
            string html = "<foo title=\"/>\" />";
            Assert.AreEqual(string.Empty, Html.StripHtml(html));
        }

        [Test, Ignore]
        public void Html_WithTagClosedByStartTag_StripsFirstTag()
        {
            string html = "<foo <>Test";
            Assert.AreEqual("<>Test", Html.StripHtml(html));
        }

        [Test, Ignore]
        public void Html_WithSingleQuotedAttrContainingDoubleQuotesAndEndTagChar_StripsEntireTag()
        {
            string html = @"<foo ='test""/>title' />";
            Assert.AreEqual(string.Empty, Html.StripHtml(html));
        }

        [Test, Ignore]
        public void Html_WithDoubleQuotedAttributeContainingSingleQuotesAndEndTagChar_StripsEntireTag()
        {
            string html = @"<foo =""test'/>title"" />";
            Assert.AreEqual(string.Empty, Html.StripHtml(html));
        }

        [Test]
        public void Html_WithNonQuotedAttribute_StripsEntireTagWithoutStrippingText()
        {
            string html = @"<foo title=test>title />";
            Assert.AreEqual("title />", Html.StripHtml(html));
        }

        [Test]
        public void Html_WithNonQuotedAttributeContainingDoubleQuotes_StripsEntireTagWithoutStrippingText()
        {
            string html = @"<p title = test-test""-test>title />Test</p>";
            Assert.AreEqual("title />Test", Html.StripHtml(html));
        }

        [Test]
        public void Html_WithNonQuotedAttributeContainingQuotedSection_StripsEntireTagWithoutStrippingText()
        {
            string html = @"<p title = test-test""- >""test> ""title />Test</p>";
            Assert.AreEqual(@"""test> ""title />Test", Html.StripHtml(html));
        }

        [Test, Ignore]
        public void Html_WithTagClosingCharInAttributeValueWithNoNameFollowedByText_ReturnsText()
        {
            string html = @"<foo = "" />title"" />Test";
            Assert.AreEqual("Test", Html.StripHtml(html));
        }

        [Test, Ignore]
        public void Html_WithTextThatLooksLikeTag_ReturnsText()
        {
            string html = @"<çoo = "" />title"" />Test";
            Assert.AreEqual(html, Html.StripHtml(html));
        }

        [Test]
        public void Html_WithCommentOnly_ReturnsEmptyString()
        {
            string s = "<!-- this go bye bye>";
            Assert.AreEqual(string.Empty, Html.StripHtml(s));
        }

        [Test]
        public void Html_WithNonDashDashComment_ReturnsEmptyString()
        {
            string s = "<! this go bye bye>";
            Assert.AreEqual(string.Empty, Html.StripHtml(s));
        }

        [Test]
        public void Html_WithTwoConsecutiveComments_ReturnsEmptyString()
        {
            string s = "<!-- this go bye bye><!-- another comment>";
            Assert.AreEqual(string.Empty, Html.StripHtml(s));
        }

        [Test]
        public void Html_WithTextBeforeComment_ReturnsText()
        {
            string s = "Hello<!-- this go bye bye -->";
            Assert.AreEqual("Hello", Html.StripHtml(s));
        }

        [Test]
        public void Html_WithTextAfterComment_ReturnsText()
        {
            string s = "<!-- this go bye bye -->World";
            Assert.AreEqual("World", Html.StripHtml(s));
        }

        [Test, Ignore]
        public void Html_WithAngleBracketsButNotHtml_ReturnsText()
        {
            string s = "<$)*(@&$(@*>";
            Assert.AreEqual(s, Html.StripHtml(s));
        }

        [Test]
        public void Html_WithCommentInterleavedWithText_ReturnsText()
        {
            string s = "Hello <!-- this go bye bye --> World <!--> This is fun";
            Assert.AreEqual("Hello  World  This is fun", Html.StripHtml(s));
        }

        [Test, Ignore]
        public void Html_WithCommentBetweenNonTagButLooksLikeTag_DoesStripComment()
        {
            string s = @"<ç123 title=""<!bc def>"">";
            Assert.AreEqual(@"<ç123 title="""">", Html.StripHtml(s));
        }
    }
}
