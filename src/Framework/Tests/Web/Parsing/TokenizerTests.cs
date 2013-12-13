using System;
using System.Linq;
using N2.Web.Parsing;
using NUnit.Framework;
using Shouldly;

namespace N2.Tests.Web.Parsing
{
    [TestFixture]
    public class TokenizerTests
    {
        [Test]
        public void SingleWord()
        {
            string text = "Hello";
            var token = text.Tokenize().Single();

            Assert.That(token.Fragment, Is.EqualTo(text));
            Assert.That(token.Index, Is.EqualTo(0));
            Assert.That(token.Type, Is.EqualTo(TokenType.Text));
        }

        [Test]
        public void TwoWords()
        {
            string text = "Hello World";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[0].Fragment, Is.EqualTo("Hello"));
            Assert.That(tokens[0].Index, Is.EqualTo(0));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Text));

            Assert.That(tokens[1].Fragment, Is.EqualTo(" "));
            Assert.That(tokens[1].Index, Is.EqualTo(5));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Whitespace));

            Assert.That(tokens[2].Fragment, Is.EqualTo("World"));
            Assert.That(tokens[2].Index, Is.EqualTo(6));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Text));
        }

        [Test]
        public void Paragraph_WithWord()
        {
            string text = "<p>Hello</p>";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[0].Fragment, Is.EqualTo("<p>"));
            Assert.That(tokens[0].Index, Is.EqualTo(0));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Element));

            Assert.That(tokens[1].Fragment, Is.EqualTo("Hello"));
            Assert.That(tokens[1].Index, Is.EqualTo(3));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Text));

            Assert.That(tokens[2].Fragment, Is.EqualTo("</p>"));
            Assert.That(tokens[2].Index, Is.EqualTo(8));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.EndElement));
        }

        [Test]
        public void Paragrap_hWithTwoWords()
        {
            string text = "<p>Hello World</p>";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens.Count, Is.EqualTo(5));

            Assert.That(tokens[0].Fragment, Is.EqualTo("<p>"));
            Assert.That(tokens[0].Index, Is.EqualTo(0));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Element));

            Assert.That(tokens[1].Fragment, Is.EqualTo("Hello"));
            Assert.That(tokens[1].Index, Is.EqualTo(3));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Text));

            Assert.That(tokens[2].Fragment, Is.EqualTo(" "));

            Assert.That(tokens[3].Fragment, Is.EqualTo("World"));

            Assert.That(tokens[4].Fragment, Is.EqualTo("</p>"));
            Assert.That(tokens[4].Type, Is.EqualTo(TokenType.EndElement));
        }

        [Test]
        public void NestedElements()
        {
            string text = "<p>Hello <b>World</b></p>";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens.Count, Is.EqualTo(7));

            Assert.That(tokens[3].Fragment, Is.EqualTo("<b>"));
            Assert.That(tokens[3].Type, Is.EqualTo(TokenType.Element));
            Assert.That(tokens[3].Index, Is.EqualTo(text.IndexOf("<b>")));

            Assert.That(tokens[5].Fragment, Is.EqualTo("</b>"));
            Assert.That(tokens[5].Type, Is.EqualTo(TokenType.EndElement));
            Assert.That(tokens[5].Index, Is.EqualTo(text.IndexOf("</b>")));
        }

        [Test]
        public void SelfClosingElement()
        {
            string text = "<hr/>";
            var token = text.Tokenize().Single();

            Assert.That(token.Fragment, Is.EqualTo(text));
            Assert.That(token.Index, Is.EqualTo(0));
            Assert.That(token.Type, Is.EqualTo(TokenType.Element));
        }

        [Test]
        public void Tab_IsWhitespace()
        {
            string text = "Hello\tWorld";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[0].Fragment, Is.EqualTo("Hello"));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Text));

            Assert.That(tokens[1].Fragment, Is.EqualTo("\t"));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Whitespace));

            Assert.That(tokens[2].Fragment, Is.EqualTo("World"));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Text));
        }

        [Test]
        public void Newline_IsNewline()
        {
            string text = "Hello\nWorld";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[0].Fragment, Is.EqualTo("Hello"));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Text));

            Assert.That(tokens[1].Fragment, Is.EqualTo("\n"));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.NewLine));

            Assert.That(tokens[2].Fragment, Is.EqualTo("World"));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Text));
        }

        [Test]
        public void Carriagereturn_IsNewline()
        {
            string text = "Hello\rWorld";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[0].Fragment, Is.EqualTo("Hello"));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Text));

            Assert.That(tokens[1].Fragment, Is.EqualTo("\r"));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.NewLine));

            Assert.That(tokens[2].Fragment, Is.EqualTo("World"));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Text));
        }

        [Test]
        public void EnvironmentNewline_IsWhitespace()
        {
            string text = "Hello" + Environment.NewLine + "World";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[0].Fragment, Is.EqualTo("Hello"));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Text));

            Assert.That(tokens[1].Fragment, Is.EqualTo(Environment.NewLine));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.NewLine));

            Assert.That(tokens[2].Fragment, Is.EqualTo("World"));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Text));
        }

        [TestCase("§")]
        [TestCase("!")]
        [TestCase("\"")]
        [TestCase("#")]
        [TestCase("¤")]
        [TestCase("%")]
        [TestCase("&")]
        [TestCase("/")]
        [TestCase("(")]
        [TestCase(")")]
        [TestCase("=")]
        [TestCase("+")]
        [TestCase("?")]
        [TestCase("´")]
        [TestCase("`")]
        [TestCase("@")]
        [TestCase("£")]
        [TestCase("$")]
        [TestCase("€")]
        [TestCase("{")]
        [TestCase("[")]
        [TestCase("]")]
        [TestCase("}")]
        [TestCase("\\")]
        [TestCase("|")]
        [TestCase("¨")]
        [TestCase("^")]
        [TestCase("~")]
        [TestCase("'")]
        [TestCase("*")]
        [TestCase(",")]
        [TestCase(";")]
        [TestCase(".")]
        [TestCase(":")]
        [TestCase("-")]
        public void IsSymbol(string symbol)
        {
            string text = "Hello" + symbol + "World";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[1].Fragment, Is.EqualTo(symbol));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Symbol));
        }

        [Test]
        public void Underscore_IsNotSymbol()
        {
            string text = "Hello_World";
            var token = text.Tokenize().Single();

            Assert.That(token.Fragment, Is.EqualTo(text));
            Assert.That(token.Type, Is.EqualTo(TokenType.Text));
        }

        [Test]
        public void LeadingWhitespace()
        {
            string text = "  Hello";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[0].Fragment, Is.EqualTo("  "));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Whitespace));
        }

        [Test]
        public void TrailingWhitespace()
        {
            string text = "Hello  ";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[1].Fragment, Is.EqualTo("  "));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Whitespace));
        }

        [Test]
        public void SameSymbols_AreGrouped()
        {
            string text = "Hello!!!";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[1].Fragment, Is.EqualTo("!!!"));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Symbol));
        }

        [Test]
        public void DifferentSymbols_AreNotGrouped()
        {
            string text = "Hello?!?";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[1].Fragment, Is.EqualTo("?"));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Symbol));

            Assert.That(tokens[2].Fragment, Is.EqualTo("!"));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Symbol));

            Assert.That(tokens[3].Fragment, Is.EqualTo("?"));
            Assert.That(tokens[3].Type, Is.EqualTo(TokenType.Symbol));
        }

        [Test]
        public void Newline_DoesntCause_TrailingEmptyToken()
        {
            string text = @"
";
            var blocks = text.Tokenize();

            Assert.That(blocks.Last().Fragment, Is.Not.EqualTo(""));
        }

        [Test]
        public void Element_WithAttribute_SingleQuote()
        {
            string text = "<p onclick='alert(1)'>Hello</p>";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[0].Fragment, Is.EqualTo("<p onclick='alert(1)'>"));
            Assert.That(tokens[0].Index, Is.EqualTo(0));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Element));
        }

        [Test]
        public void Element_WithAttribute_DoubleQuote()
        {
            string text = "<p onclick=\"alert(1)\">Hello</p>";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[0].Fragment, Is.EqualTo("<p onclick=\"alert(1)\">"));
            Assert.That(tokens[0].Index, Is.EqualTo(0));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Element));
        }

        [Test]
        public void Element_WithAttribute_GreaterThan()
        {
            string text = "<p onclick='>'>Hello</p>";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[0].Fragment, Is.EqualTo("<p onclick='>'>"));
            Assert.That(tokens[0].Index, Is.EqualTo(0));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Element));
        }

        [Test]
        public void Element_WithAttribute_SingleQuote_DoubleQuote()
        {
            string text = "<p onclick='alert(\"hello\");'>Hello</p>";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[0].Fragment, Is.EqualTo("<p onclick='alert(\"hello\");'>"));
            Assert.That(tokens[0].Index, Is.EqualTo(0));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Element));
        }

        [Test]
        public void Element_WithAttribute_DoubleQuote_SingleQuote()
        {
            string text = "<p onclick=\"alert('hello');\">Hello</p>";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[0].Fragment, Is.EqualTo("<p onclick=\"alert('hello');\">"));
            Assert.That(tokens[0].Index, Is.EqualTo(0));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Element));
        }

        [Test]
        public void Element_WithAttribute_WithEscapedQuote()
        {
            string text = "<p onclick=\"alert('\\\\');\">Hello</p>";
            var tokens = text.Tokenize().ToList();

            Assert.That(tokens[0].Fragment, Is.EqualTo("<p onclick=\"alert('\\\\');\">"));
            Assert.That(tokens[0].Index, Is.EqualTo(0));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Element));
        }

		[Test]
		public void Comment()
		{
			string text = "<!-- hello world -->";
			var tokens = text.Tokenize().ToList();

			Assert.That(tokens[0].Fragment, Is.EqualTo("<!-- hello world -->"));
			Assert.That(tokens[0].Index, Is.EqualTo(0));
			Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Comment));
		}

		[Test]
		public void Doctype()
		{
			string text = "<!DOCTYPE html>";
			var tokens = text.Tokenize().ToList();

			Assert.That(tokens[0].Fragment, Is.EqualTo("<!DOCTYPE html>"));
			Assert.That(tokens[0].Index, Is.EqualTo(0));
			Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Comment));
		}

		[Test]
		public void Cdata()
		{
			string text = @"thebeginning<![CDATA[x]]>therest";
			var tokens = text.Tokenize().ToList();

			Assert.That(tokens[1].Fragment, Is.EqualTo(@"<![CDATA[x]]>"));
			Assert.That(tokens[1].Index, Is.EqualTo(12));
			Assert.That(tokens[1].Type, Is.EqualTo(TokenType.CData));
		}

		[Test]
		public void Cdata_AtEof()
		{
			string text = @"<![CDATA[x]]>";
			var tokens = text.Tokenize().ToList();

			tokens.Count.ShouldBe(1);
			Assert.That(tokens[0].Fragment, Is.EqualTo(@"<![CDATA[x]]>"));
			Assert.That(tokens[0].Index, Is.EqualTo(0));
			Assert.That(tokens[0].Type, Is.EqualTo(TokenType.CData));
		}

		[Test]
		public void Script_Cdata()
		{
			string text = @"<script>//<![CDATA[
alert(1);
//]]></script>";
			var tokens = text.Tokenize().ToList();

			Assert.That(tokens[2].Fragment, Is.EqualTo(@"<![CDATA[
alert(1);
//]]>"));
			Assert.That(tokens[2].Index, Is.EqualTo(10));
			Assert.That(tokens[2].Type, Is.EqualTo(TokenType.CData));
		}
    }
}
