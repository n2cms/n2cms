using System.Linq;
using N2.Web.Parsing;
using N2.Web.Wiki;
using NUnit.Framework;

namespace N2.Tests.Web.Parsing
{
    [TestFixture]
    public class WikiParserTests
    {
        Parser p = new WikiParser();

        [Test]
        public void Word()
        {
            string text = "hello";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("Text"));
            Assert.That(block.Tokens.Count, Is.EqualTo(1));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void Sentence()
        {
            string text = "hello world";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("Text"));
            Assert.That(block.Tokens.Count, Is.EqualTo(3));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void InternalLink()
        {
            string text = "[[hello]]";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("InternalLink"));
            Assert.That(block.Tokens.Count, Is.EqualTo(3));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void InternalLinks()
        {
            string text = "[[hello]][[world]]";
            var blocks = p.Parse(text).ToList();

            Assert.That(blocks.Count, Is.EqualTo(2));

            Assert.That(blocks[0].Command, Is.EqualTo("InternalLink"));
            Assert.That(blocks[0].ToString(), Is.EqualTo("[[hello]]"));

            Assert.That(blocks[1].Command, Is.EqualTo("InternalLink"));
            Assert.That(blocks[1].ToString(), Is.EqualTo("[[world]]"));
        }

        [Test]
        public void InternalLink_WithCustomData()
        {
            string text = "[[hello|howdys]]";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("InternalLink"));
            Assert.That(block.ToString(), Is.EqualTo("[[hello|howdys]]"));
        }

        [Test]
        public void InternalLink_FollowedBy_Text()
        {
            string text = "[[hello]]world";
            var blocks = p.Parse(text).ToList();

            Assert.That(blocks[0].Command, Is.EqualTo("InternalLink"));
            Assert.That(blocks[0].ToString(), Is.EqualTo("[[hello]]"));

            Assert.That(blocks[1].Command, Is.EqualTo("Text"));
            Assert.That(blocks[1].ToString(), Is.EqualTo("world"));
        }

        [Test]
        public void Text_FollowedBy_InternalLink()
        {
            string text = "world[[hello]]";
            var blocks = p.Parse(text).ToList();

            Assert.That(blocks[0].Command, Is.EqualTo("Text"));
            Assert.That(blocks[0].ToString(), Is.EqualTo("world"));

            Assert.That(blocks[1].Command, Is.EqualTo("InternalLink"));
            Assert.That(blocks[1].ToString(), Is.EqualTo("[[hello]]"));
        }

        [Test]
        public void InternalLink_IsInvalid_WhenContaining_EndOfLine()
        {
            string text = "[[hello\n]]";
            var block = p.Parse(text).FirstOrDefault();

            Assert.That(block.Command, Is.Not.EqualTo("InternalLink"));
        }

        [Test]
        public void InternalLink_IsInvalid_WhenContaining_Markup()
        {
            string text = "[[hello<b>world</b>]]";
            var block = p.Parse(text).FirstOrDefault();

            Assert.That(block.Command, Is.Not.EqualTo("InternalLink"));
        }

        [Test]
        public void ExternalLink()
        {
            string text = "[http://n2cms.com]";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("ExternalLink"));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void ExternalLink_WithCustomData()
        {
            string text = "[n2cms.com|http://n2cms.com]";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("ExternalLink"));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void ExternalLink_NoBrackets()
        {
            string text = "http://n2cms.com";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("ExternalLink"));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void ExternalLink_NoBrackets_EmbeddedInText()
        {
            string text = "Hello http://n2cms.com, how do you do?";
            var blocks = p.Parse(text).ToList();

            Assert.That(blocks.Count, Is.EqualTo(3));
            Assert.That(blocks[0].Command, Is.EqualTo("Text"));
            Assert.That(blocks[1].Command, Is.EqualTo("ExternalLink"));
            Assert.That(blocks[1].ToString(), Is.EqualTo("http://n2cms.com"));
            Assert.That(blocks[2].Command, Is.EqualTo("Text"));
        }

        [Test]
        public void Bold()
        {
            string text = "'''hello'''";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("Bold"));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void Italics()
        {
            string text = "''hello''";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("Italics"));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void BoldItalics()
        {
            string text = "'''''hello'''''";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("BoldItalics"));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [TestCase("=")]
        [TestCase("==")]
        [TestCase("===")]
        [TestCase("====")]
        public void Headings(string token)
        {
            string text = token + "hello" + token;
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("Heading"));
            Assert.That(block.Argument, Is.EqualTo(token));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [TestCase("=")]
        [TestCase("==")]
        [TestCase("===")]
        [TestCase("====")]
        public void Headings_SubComponent(string token)
        {
            string text = token + "[[hello]]" + token;
            var block = p.Parse(text).Single();

            Assert.That(block.Components.Single().Command, Is.EqualTo("InternalLink"));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void UnorderedList()
        {
            string text = "* hello";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("UnorderedListItem"));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void UnorderedList_Nesting()
        {
            string text = @"* hello
** hello nested";
            var blocks = p.Parse(text).ToList();

            Assert.That(blocks[0].Command, Is.EqualTo("UnorderedListItem"));
            Assert.That(blocks[1].Command, Is.EqualTo("UnorderedListItem"));
            Assert.That(blocks[1].ToString(), Is.EqualTo("** hello nested"));
        }

        [Test]
        public void UnorderedList_2Items()
        {
            string text = @"* hello
* world";
            var blocks = p.Parse(text).ToList();

            Assert.That(blocks.Count, Is.EqualTo(2));

            Assert.That(blocks[0].Command, Is.EqualTo("UnorderedListItem"));
            Assert.That(blocks[0].ToString(), Is.EqualTo(@"* hello
"));

            Assert.That(blocks[1].Command, Is.EqualTo("UnorderedListItem"));
            Assert.That(blocks[1].ToString(), Is.EqualTo("* world"));
        }

        [Test]
        public void UnorderedList_2Items_DoesntIncludeNewLine_InSubComponents()
        {
            string text = @"* hello
* world
";
            var blocks = p.Parse(text).ToList();

            Assert.That(blocks[0].Components.Single().ToString(), Is.EqualTo("hello"));
            Assert.That(blocks[1].Components.Single().ToString(), Is.EqualTo("world"));
        }

        [Test]
        public void UnorderedList_InnerComponent()
        {
            string text = "* [[hello]]";
            var component = p.Parse(text).Single();

            Assert.That(component.Components.Single().Command, Is.EqualTo("InternalLink"));
            Assert.That(component.Components.Single().ToString(), Is.EqualTo("[[hello]]"));
        }

        [Test]
        public void UnorderedList_Multiple_InnerComponents()
        {
            string text = "* [[hello]] world!";
            var component = p.Parse(text).Single();
            
            var subComponents = component.Components.ToList();
            Assert.That(subComponents.Count, Is.EqualTo(2));
            Assert.That(subComponents[0].Command, Is.EqualTo("InternalLink"));
            Assert.That(subComponents[1].Command, Is.EqualTo("Text"));
        }

        [Test]
        public void UnorderedList_Bold_Link()
        {
            string text = "* '''[[hello]] world!'''";
            var component = p.Parse(text).Single();

            var bold = component.Components.Single();
            var link = bold.Components.First();
            var textC = bold.Components.Skip(1).First();

            Assert.That(bold.Command, Is.EqualTo("Bold"));
            Assert.That(link.Command, Is.EqualTo("InternalLink"));
            Assert.That(textC.Command, Is.EqualTo("Text"));
        }

        [Test]
        public void OrderedList()
        {
            string text = "# hello";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("OrderedListItem"));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void OrderedList_Nesting()
        {
            string text = @"# hello
## hello nested";
            var blocks = p.Parse(text).ToList();

            Assert.That(blocks[0].Command, Is.EqualTo("OrderedListItem"));
            Assert.That(blocks[1].Command, Is.EqualTo("OrderedListItem"));
            Assert.That(blocks[1].ToString(), Is.EqualTo("## hello nested"));
        }

        [Test]
        public void OrderedList_2Items()
        {
            string text = @"# hello
# world";
            var blocks = p.Parse(text).ToList();

            Assert.That(blocks.Count, Is.EqualTo(2));

            Assert.That(blocks[0].Command, Is.EqualTo("OrderedListItem"));
            Assert.That(blocks[0].ToString(), Is.EqualTo(@"# hello
"));

            Assert.That(blocks[1].Command, Is.EqualTo("OrderedListItem"));
            Assert.That(blocks[1].ToString(), Is.EqualTo("# world"));
        }

        [Test]
        public void Comment()
        {
            string text = "('''hello''')";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("Comment"));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [TestCase("~~~")]
        [TestCase("~~~~")]
        [TestCase("~~~~~")]
        public void UserInfo(string text)
        {
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("UserInfo"));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void Template()
        {
            string text = "{{pagename}}";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("Template"));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void Template_WithData()
        {
            string text = "{{hello|world}}";
            var block = p.Parse(text).Single();

            Assert.That(block.Command, Is.EqualTo("Template"));
            Assert.That(block.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void Html()
        {
            string text = @"<p>hello</p>";
            var blocks = p.Parse(text).ToList();

            Assert.That(blocks[0].Command, Is.EqualTo("HtmlElement"));
            Assert.That(blocks[1].Command, Is.EqualTo("Text"));
            Assert.That(blocks[1].ToString(), Is.EqualTo("hello"));
            Assert.That(blocks[2].Command, Is.EqualTo("HtmlElement"));
        }
    }
}
