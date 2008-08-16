using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Templates.Wiki;
using System.Web.UI;
using System.IO;
using NUnit.Framework.SyntaxHelpers;
using N2.Web;
using N2.Templates.Wiki.Fragmenters;
using N2.Plugin;
using N2.Tests;
using Rhino.Mocks;
using N2.Templates.Wiki.Renderers;

namespace N2.Templates.Tests.Wiki
{
    [TestFixture]
    public class RenderingWikiText : ItemTestsBase
    {
        Templates.Wiki.Items.Wiki wiki;
        Templates.Wiki.Items.WikiArticle article;
        WikiParser parser;
        WikiRenderer renderer;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var urlParser = new FakeUrlParser();
            wiki = new Templates.Wiki.Items.Wiki();
            wiki.Name = "wiki";
            ((IUrlParserDependency)wiki).SetUrlParser(urlParser);
            article = new Templates.Wiki.Items.WikiArticle();
            article.Name = "existing-article";
            article.SavedBy = "admin";
            ((IUrlParserDependency)article).SetUrlParser(urlParser);
            article.AddTo(wiki);

            parser = new WikiParser();

            var pluginFinder = mocks.Stub<IPluginFinder>();
            Expect.Call(pluginFinder.GetPlugins<ITemplateRenderer>()).Return(new ITemplateRenderer[] { new FakeTemplateRenderer() });
            mocks.ReplayAll();

            renderer = new WikiRenderer(pluginFinder, new ThreadContext());
        }

        [Test]
        public void CanRender_SingleTextFragment()
        {
            string html = ParseAndRenderWikiText("hello");

            Assert.That(html, Is.EqualTo("hello"));
        }

        [Test]
        public void Renders_ExternalLink_WithAnchor()
        {
            string html = ParseAndRenderWikiText("[http://n2cms.com]");

            Assert.That(html, Is.EqualTo("<a href=\"http://n2cms.com\" rel=\"nofollow\">http://n2cms.com</a>"));
        }

        [Test]
        public void Renders_LinkResemblingText_AsAnchor()
        {
            string html = ParseAndRenderWikiText("http://n2cms.com");

            Assert.That(html, Is.EqualTo("<a href=\"http://n2cms.com\" rel=\"nofollow\">http://n2cms.com</a>"));
        }

        [Test]
        public void Renders_ExternalLink_WithLinkAndText()
        {
            string html = ParseAndRenderWikiText("[http://n2cms.com|N2 CMS Homepage]");

            Assert.That(html, Is.EqualTo("<a href=\"http://n2cms.com\" rel=\"nofollow\">N2 CMS Homepage</a>"));
        }

        [Test]
        public void CanRender_InternalLink_ToExistingArticle()
        {
            string html = ParseAndRenderWikiText("[[existing-article]]");

            Assert.That(html, Is.EqualTo("<a href=\"/wiki/existing-article.aspx\">existing-article</a>"));
        }

        [Test]
        public void CanRender_InternalLink_ToExistingArticle_WithWhiteSpace()
        {
            string html = ParseAndRenderWikiText("[[existing article]]");

            Assert.That(html, Is.EqualTo("<a href=\"/wiki/existing-article.aspx\">existing article</a>"));
        }

        [Test]
        public void CanRender_InternalLink_ToExistingArticle_WithDifferentText()
        {
            string html = ParseAndRenderWikiText("[[existing-article|Existing Article]]");

            Assert.That(html, Is.EqualTo("<a href=\"/wiki/existing-article.aspx\">Existing Article</a>"));
        }

        [Test]
        public void CanRender_InternalLink_ToNewArticle()
        {
            string html = ParseAndRenderWikiText("[[new-article]]");

            Assert.That(html, Is.EqualTo("<a href=\"/wiki/new-article.aspx\" class=\"new\">new-article</a>"));
        }

        [Test]
        public void CanRender_InternalLink_ToNewArticle_WithDifferentText()
        {
            string html = ParseAndRenderWikiText("[[new-article|New Article]]");

            Assert.That(html, Is.EqualTo("<a href=\"/wiki/new-article.aspx\" class=\"new\">New Article</a>"));
        }

        [Test]
        public void CanRender_UserInfo()
        {
            string html = ParseAndRenderWikiText("~~~");

            Assert.That(html, Is.EqualTo("admin"));
        }

        [Test]
        public void CanRender_UserInfo_AndDate()
        {
            string html = ParseAndRenderWikiText("~~~~");

            Assert.That(html, Is.EqualTo("admin, " + DateTime.Now));
        }

        [Test]
        public void CanRender_Date()
        {
            string html = ParseAndRenderWikiText("~~~~~");

            Assert.That(html, Is.EqualTo(DateTime.Now.ToString()));
        }

        [Test]
        public void CanRender_CustomControl()
        {
            string html = ParseAndRenderWikiText("{{FakeTemplate}}");

            Assert.That(html, Is.EqualTo("Template:FakeTemplate"));
        }

        [Test]
        public void CanRender_CustomControl_WithWrongCasing()
        {
            string html = ParseAndRenderWikiText("{{faketemplate}}");

            Assert.That(html, Is.EqualTo("Template:FakeTemplate"));
        }

        [Test]
        public void CanRender_Heading1()
        {
            string html = ParseAndRenderWikiText("=Heading 1=");

            Assert.That(html, Is.EqualTo("<h1>Heading 1</h1>"));
        }

        [Test]
        public void CanRender_Heading2()
        {
            string html = ParseAndRenderWikiText("==Heading 2==");

            Assert.That(html, Is.EqualTo("<h2>Heading 2</h2>"));
        }

        [Test]
        public void CanRender_Heading3()
        {
            string html = ParseAndRenderWikiText("===Heading 3===");

            Assert.That(html, Is.EqualTo("<h3>Heading 3</h3>"));
        }

        [Test]
        public void CanRender_Heading4()
        {
            string html = ParseAndRenderWikiText("====Heading 4====");

            Assert.That(html, Is.EqualTo("<h4>Heading 4</h4>"));
        }

        [Test]
        public void DoesntTreat_InlineEqualSign_AsHeading()
        {
            string html = ParseAndRenderWikiText("Something = Someotherthing");

            Assert.That(html, Is.EqualTo("Something = Someotherthing"));
        }

        [Test]
        public void RendersComments_WithoutTreatingContents_AsMarkup()
        {
            string html = ParseAndRenderWikiText("(=I'm not a heading=)");

            Assert.That(html, Is.EqualTo("=I'm not a heading="));
        }

        [Test]
        public void Renders_LinkToUpload_WhenImage_DoesNotExist()
        {
            string html = ParseAndRenderWikiText("[[Image:n2.png]]");

            Assert.That(html, Is.EqualTo("<a href=\"/wiki/Upload.aspx?parameter=n2.png&returnUrl=%2fwiki%2fexisting-article.aspx\" class=\"new\">n2.png</a>"));
        }

        [Test]
        public void Renders_ImagTag_WhenImageExists()
        {
            var temp = InternalLinkRenderer.FileExists;
            InternalLinkRenderer.FileExists = delegate { return true; };

            try
            {
                string html = ParseAndRenderWikiText("[[Image:n2.png]]");

                Assert.That(html, Is.EqualTo("<img src=\"/Upload/Wiki/n2.png\" alt=\"n2.png\" />"));
            }
            finally
            {
                InternalLinkRenderer.FileExists = temp;
            }
        }

        [Test]
        public void Renders_ImagTag_WithAltParameter()
        {
            var temp = InternalLinkRenderer.FileExists;
            InternalLinkRenderer.FileExists = delegate { return true; };

            try
            {
                string html = ParseAndRenderWikiText("[[Image:n2.png|N2 CMS Logo]]");

                Assert.That(html, Is.EqualTo("<img src=\"/Upload/Wiki/n2.png\" alt=\"N2 CMS Logo\" />"));
            }
            finally
            {
                InternalLinkRenderer.FileExists = temp;
            }
        }

        [Test]
        public void Reformats_NewLine_ToBreakElement()
        {
            string input = @"Line 1
Line 2";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("Line 1<br/>Line 2"));
        }

        [Test]
        public void Leaves_BreakElement()
        {
            string input = @"Line 1<br/>Line 2";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("Line 1<br/>Line 2"));
        }

        [Test]
        public void Leaves_ParagraphElements()
        {
            string input = @"<p>Line 1</p><p>Line 2</p>";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<p>Line 1</p><p>Line 2</p>"));
        }

        [Test]
        public void Reformats_ParagraphElement_WithExternalLineBreaks_ToTrimmedParagraphElements()
        {
            string input = @"
<p>Line 1</p>
<p>Line 2</p>
";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<p>Line 1</p><p>Line 2</p>"));
        }

        [Test]
        public void Reformats_ParagraphElement_WithInternalLineBreaks_ToTrimmedParagraphElements()
        {
            string input = @"<p>
Line 1
</p><p>
Line 2
</p>";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<p>Line 1</p><p>Line 2</p>"));
        }

        [Test]
        public void DoubleQuotes_AreReformatted_IntoItalics()
        {
            string input = @"''Text''";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<em>Text</em>"));
        }

        [Test]
        public void TripleQuotes_AreReformatted_IntoBold()
        {
            string input = @"'''Text'''";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<strong>Text</strong>"));
        }

        [Test]
        public void QuintipleQuotes_AreReformatted_IntoBoldItalics()
        {
            string input = @"'''''Text'''''";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<strong><em>Text</em></strong>"));
        }

        [Test]
        public void SingleStar_IsTreatedAs_UnorderedList()
        {
            string input = @"* Text";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<ul><li>Text</li></ul>"));
        }

        [Test]
        public void CanNest_UnorderedLists()
        {
            string input = @"* Text
** Child Text";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<ul><li>Text<ul><li>Child Text</li></ul></li></ul>"));
        }

        [Test]
        public void CanNest_AndUnnest_UnorderedLists()
        {
            string input = @"* Text 1
** Child Text
* Text 2";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<ul><li>Text 1<ul><li>Child Text</li></ul></li><li>Text 2</li></ul>"));
        }

        [Test]
        public void CanNest_AndUnnest_UnorderedLists_WithSeveralChildren()
        {
            string input = @"* Text 1
** Child Text 1
** Child Text 2
* Text 2";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<ul><li>Text 1<ul><li>Child Text 1</li><li>Child Text 2</li></ul></li><li>Text 2</li></ul>"));
        }

        [Test]
        public void CanNest_AndUnnest_UnorderedLists_WithThreeLevels()
        {
            string input = @"* Text 1
** Child Text 1
*** Grandchild Text 1
** Child Text 2
* Text 2";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<ul><li>Text 1<ul><li>Child Text 1<ul><li>Grandchild Text 1</li></ul></li><li>Child Text 2</li></ul></li><li>Text 2</li></ul>"));
        }

        [Test]
        public void OrderedLists_MayContain_ChildFragments()
        {
            string input = "* [[existing-article]] Text";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<ul><li><a href=\"/wiki/existing-article.aspx\">existing-article</a> Text</li></ul>"));
        }

        [Test]
        public void SingleHash_IsTreatedAs_OrderedList()
        {
            string input = @"# Text";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<ol><li>Text</li></ol>"));
        }

        [Test]
        public void CanNest_OrderedLists()
        {
            string input = @"# Text
## Child Text";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<ol><li>Text<ol><li>Child Text</li></ol></li></ol>"));
        }

        [Test]
        public void WillNotAdd_LineBreak_AfterHeading()
        {
            string input = @"==Heading==
";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<h2>Heading</h2>"));
        }

        [Test]
        public void WillNotAdd_LineBreak_AfterUnorderedList()
        {
            string input = @"* Heading
";
            string output = ParseAndRenderWikiText(input);
            Assert.That(output, Is.EqualTo("<ul><li>Heading</li></ul>"));
        }

        private string ParseAndRenderWikiText(string text)
        {
            var buf = new StringBuilder();
            using (var writer = new HtmlTextWriter(new StringWriter(buf)))
            {
                Page container = new Page();
                renderer.AddTo(parser.Parse(text), container, article);
                container.RenderControl(writer);
            }
            return buf.ToString();
        }
    }
}
