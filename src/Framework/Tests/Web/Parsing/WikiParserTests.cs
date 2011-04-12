using System.Linq;
using N2.Web.Parsing;
using N2.Web.Parsing.Wiki;
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
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("Text"));
			Assert.That(block.Tokens.Count, Is.EqualTo(1));
			Assert.That(block.Data, Is.EqualTo(text));
		}

		[Test]
		public void Sentence()
		{
			string text = "hello world";
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("Text"));
			Assert.That(block.Tokens.Count, Is.EqualTo(3));
			Assert.That(block.Data, Is.EqualTo(text));
		}

		[Test]
		public void InternalLink()
		{
			string text = "[[hello]]";
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("InternalLink"));
			Assert.That(block.Tokens.Count, Is.EqualTo(3));
			Assert.That(block.Data, Is.EqualTo("hello"));
		}

		[Test]
		public void InternalLinks()
		{
			string text = "[[hello]][[world]]";
			var blocks = p.Parse(text.Tokenize()).ToList();

			Assert.That(blocks.Count, Is.EqualTo(2));

			Assert.That(blocks[0].Command, Is.EqualTo("InternalLink"));
			Assert.That(blocks[0].Data, Is.EqualTo("hello"));

			Assert.That(blocks[1].Command, Is.EqualTo("InternalLink"));
			Assert.That(blocks[1].Data, Is.EqualTo("world"));
		}

		[Test]
		public void InternalLink_WithCustomData()
		{
			string text = "[[hello|howdys]]";
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("InternalLink"));
			Assert.That(block.Data, Is.EqualTo("hello|howdys"));
		}

		[Test]
		public void InternalLink_FollowedBy_Text()
		{
			string text = "[[hello]]world";
			var blocks = p.Parse(text.Tokenize()).ToList();

			Assert.That(blocks[0].Command, Is.EqualTo("InternalLink"));
			Assert.That(blocks[0].Data, Is.EqualTo("hello"));

			Assert.That(blocks[1].Command, Is.EqualTo("Text"));
			Assert.That(blocks[1].Data, Is.EqualTo("world"));
		}

		[Test]
		public void Text_FollowedBy_InternalLink()
		{
			string text = "world[[hello]]";
			var blocks = p.Parse(text.Tokenize()).ToList();

			Assert.That(blocks[0].Command, Is.EqualTo("Text"));
			Assert.That(blocks[0].Data, Is.EqualTo("world"));

			Assert.That(blocks[1].Command, Is.EqualTo("InternalLink"));
			Assert.That(blocks[1].Data, Is.EqualTo("hello"));
		}

		[Test]
		public void InternalLink_IsInvalid_WhenContaining_EndOfLine()
		{
			string text = "[[hello\n]]";
			var block = p.Parse(text.Tokenize()).FirstOrDefault();

			Assert.That(block.Command, Is.EqualTo("Text"));
			Assert.That(block.Data, Is.EqualTo(text));
		}

		[Test]
		public void InternalLink_IsInvalid_WhenContaining_Markup()
		{
			string text = "[[hello<b>world</b>]]";
			var block = p.Parse(text.Tokenize()).FirstOrDefault();

			Assert.That(block.Command, Is.EqualTo("Text"));
			Assert.That(block.Data, Is.EqualTo(text));
		}

		[Test]
		public void ExternalLink()
		{
			string text = "[http://n2cms.com]";
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("ExternalLink"));
			Assert.That(block.Data, Is.EqualTo("http://n2cms.com"));
		}

		[Test]
		public void ExternalLink_WithCustomData()
		{
			string text = "[n2cms.com|http://n2cms.com]";
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("ExternalLink"));
			Assert.That(block.Data, Is.EqualTo("n2cms.com|http://n2cms.com"));
		}

		[Test]
		public void Bold()
		{
			string text = "'''hello'''";
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("Bold"));
			Assert.That(block.Data, Is.EqualTo("hello"));
		}

		[Test]
		public void Italics()
		{
			string text = "''hello''";
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("Italics"));
			Assert.That(block.Data, Is.EqualTo("hello"));
		}

		[Test]
		public void BoldItalics()
		{
			string text = "'''''hello'''''";
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("BoldItalics"));
			Assert.That(block.Data, Is.EqualTo("hello"));
		}

		[TestCase("=", "H1")]
		[TestCase("==", "H2")]
		[TestCase("===", "H3")]
		[TestCase("====", "H4")]
		public void Headings(string token, string expectedCommand)
		{
			string text = token + "hello" + token;
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo(expectedCommand));
			Assert.That(block.Data, Is.EqualTo("hello"));
		}

		[Test]
		public void UnorderedList()
		{
			string text = "* hello";
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("UnorderedListItem"));
			Assert.That(block.Data, Is.EqualTo("hello"));
		}

		[Test]
		public void UnorderedList_2Items()
		{
			string text = @"* hello
* world";
			var blocks = p.Parse(text.Tokenize()).ToList();

			Assert.That(blocks.Count, Is.EqualTo(2));

			Assert.That(blocks[0].Command, Is.EqualTo("UnorderedListItem"));
			Assert.That(blocks[0].Data, Is.EqualTo("hello"));

			Assert.That(blocks[1].Command, Is.EqualTo("UnorderedListItem"));
			Assert.That(blocks[1].Data, Is.EqualTo("world"));
		}

		[Test]
		public void OrderedList()
		{
			string text = "# hello";
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("OrderedListItem"));
			Assert.That(block.Data, Is.EqualTo("hello"));
		}

		[Test]
		public void OrderedList_2Items()
		{
			string text = @"# hello
# world";
			var blocks = p.Parse(text.Tokenize()).ToList();

			Assert.That(blocks.Count, Is.EqualTo(2));

			Assert.That(blocks[0].Command, Is.EqualTo("OrderedListItem"));
			Assert.That(blocks[0].Data, Is.EqualTo("hello"));

			Assert.That(blocks[1].Command, Is.EqualTo("OrderedListItem"));
			Assert.That(blocks[1].Data, Is.EqualTo("world"));
		}

		[Test]
		public void Comment()
		{
			string text = "('''hello''')";
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("Comment"));
			Assert.That(block.Data, Is.EqualTo("'''hello'''"));
		}

		[TestCase("~~~")]
		[TestCase("~~~~")]
		[TestCase("~~~~~")]
		public void UserInfo(string text)
		{
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("UserInfo"));
			Assert.That(block.Data, Is.EqualTo(text));
		}

		[Test]
		public void Template()
		{
			string text = "{{pagename}}";
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("Template"));
			Assert.That(block.Data, Is.EqualTo("pagename"));
		}

		[Test]
		public void Template_WithData()
		{
			string text = "{{hello|world}}";
			var block = p.Parse(text.Tokenize()).Single();

			Assert.That(block.Command, Is.EqualTo("Template"));
			Assert.That(block.Data, Is.EqualTo("hello|world"));
		}
	}
}
