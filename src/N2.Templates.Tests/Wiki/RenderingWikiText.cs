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
            renderer = new WikiRenderer(pluginFinder);
        }

        [Test]
        public void CanRender_SingleTextFragment()
        {
            string html = ParseAndRenderWikiText("hello");

            Assert.That(html, Is.EqualTo("hello"));
        }

        [Test]
        public void CanRender_ExternalLink()
        {
            string html = ParseAndRenderWikiText("[http://n2cms.com]");

            Assert.That(html, Is.EqualTo("<a href=\"http://n2cms.com\">http://n2cms.com</a>"));
        }

        [Test]
        public void CanRender_InternalLink_ToExistingArticle()
        {
            string html = ParseAndRenderWikiText("[[existing-article]]");

            Assert.That(html, Is.EqualTo("<a href=\"/wiki/existing-article.aspx\">existing-article</a>"));
        }

        [Test]
        public void CanRender_InternalLink_ToNewArticle()
        {
            string html = ParseAndRenderWikiText("[[new-article]]");

            Assert.That(html, Is.EqualTo("<a href=\"/wiki/new-article.aspx\" class=\"new\">new-article</a>"));
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

        private string ParseAndRenderWikiText(string text)
        {
            var buf = new StringBuilder();
            using (var writer = new HtmlTextWriter(new StringWriter(buf)))
            {
                Page container = new Page();
                renderer.AddTo(parser.Parse(text), container, wiki, article);
                container.RenderControl(writer);
            }
            return buf.ToString();
        }
    }
}
