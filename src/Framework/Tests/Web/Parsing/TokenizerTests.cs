using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Web.Parsing;
using System.IO;

namespace N2.Tests.Web.Parsing
{
	[TestFixture]
	public class TokenizerTests
	{
		Tokenizer t = new Tokenizer();

		[Test]
		public void SingleWord()
		{
			string text = "Hello";
			var token = t.Tokenize(text.ToReader()).Single();

			Assert.That(token.Fragment, Is.EqualTo(text));
			Assert.That(token.Index, Is.EqualTo(0));
			Assert.That(token.Type, Is.EqualTo(TokenType.Word));
		}

		[Test]
		public void TwoWords()
		{
			string text = "Hello World";
			var tokens = t.Tokenize(text.ToReader()).ToList();

			Assert.That(tokens[0].Fragment, Is.EqualTo("Hello"));
			Assert.That(tokens[0].Index, Is.EqualTo(0));
			Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Word));

			Assert.That(tokens[1].Fragment, Is.EqualTo(" "));
			Assert.That(tokens[1].Index, Is.EqualTo(5));
			Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Whitespace));

			Assert.That(tokens[2].Fragment, Is.EqualTo("World"));
			Assert.That(tokens[2].Index, Is.EqualTo(6));
			Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Word));
		}

		[Test]
		public void Paragraph_WithWord()
		{
			string text = "<p>Hello</p>";
			var tokens = t.Tokenize(text.ToReader()).ToList();

			Assert.That(tokens[0].Fragment, Is.EqualTo("<p>"));
			Assert.That(tokens[0].Index, Is.EqualTo(0));
			Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Element));

			Assert.That(tokens[1].Fragment, Is.EqualTo("Hello"));
			Assert.That(tokens[1].Index, Is.EqualTo(3));
			Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Word));

			Assert.That(tokens[2].Fragment, Is.EqualTo("</p>"));
			Assert.That(tokens[2].Index, Is.EqualTo(8));
			Assert.That(tokens[2].Type, Is.EqualTo(TokenType.EndElement));
		}

		[Test]
		public void Paragrap_hWithTwoWords()
		{
			string text = "<p>Hello World</p>";
			var tokens = t.Tokenize(text.ToReader()).ToList();

			Assert.That(tokens.Count, Is.EqualTo(5));

			Assert.That(tokens[0].Fragment, Is.EqualTo("<p>"));
			Assert.That(tokens[0].Index, Is.EqualTo(0));
			Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Element));

			Assert.That(tokens[1].Fragment, Is.EqualTo("Hello"));
			Assert.That(tokens[1].Index, Is.EqualTo(3));
			Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Word));

			Assert.That(tokens[2].Fragment, Is.EqualTo(" "));

			Assert.That(tokens[3].Fragment, Is.EqualTo("World"));

			Assert.That(tokens[4].Fragment, Is.EqualTo("</p>"));
			Assert.That(tokens[4].Type, Is.EqualTo(TokenType.EndElement));
		}

		[Test]
		public void NestedElements()
		{
			string text = "<p>Hello <b>World</b></p>";
			var tokens = t.Tokenize(text.ToReader()).ToList();

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
			var token = t.Tokenize(text.ToReader()).Single();

			Assert.That(token.Fragment, Is.EqualTo(text));
			Assert.That(token.Index, Is.EqualTo(0));
			Assert.That(token.Type, Is.EqualTo(TokenType.Element));
		}

		[Test]
		public void Tab_IsWhitespace()
		{
			string text = "Hello\tWorld";
			var tokens = t.Tokenize(text.ToReader()).ToList();

			Assert.That(tokens[0].Fragment, Is.EqualTo("Hello"));
			Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Word));

			Assert.That(tokens[1].Fragment, Is.EqualTo("\t"));
			Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Whitespace));

			Assert.That(tokens[2].Fragment, Is.EqualTo("World"));
			Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Word));
		}

		[Test]
		public void Newline_IsWhitespace()
		{
			string text = "Hello\nWorld";
			var tokens = t.Tokenize(text.ToReader()).ToList();

			Assert.That(tokens[0].Fragment, Is.EqualTo("Hello"));
			Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Word));

			Assert.That(tokens[1].Fragment, Is.EqualTo("\n"));
			Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Whitespace));

			Assert.That(tokens[2].Fragment, Is.EqualTo("World"));
			Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Word));
		}

		[Test]
		public void Carriagereturn_IsWhitespace()
		{
			string text = "Hello\rWorld";
			var tokens = t.Tokenize(text.ToReader()).ToList();

			Assert.That(tokens[0].Fragment, Is.EqualTo("Hello"));
			Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Word));

			Assert.That(tokens[1].Fragment, Is.EqualTo("\r"));
			Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Whitespace));

			Assert.That(tokens[2].Fragment, Is.EqualTo("World"));
			Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Word));
		}

		[Test]
		public void EnvironmentNewline_IsWhitespace()
		{
			string text = "Hello" + Environment.NewLine + "World";
			var tokens = t.Tokenize(text.ToReader()).ToList();

			Assert.That(tokens[0].Fragment, Is.EqualTo("Hello"));
			Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Word));

			Assert.That(tokens[1].Fragment, Is.EqualTo(Environment.NewLine));
			Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Whitespace));

			Assert.That(tokens[2].Fragment, Is.EqualTo("World"));
			Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Word));
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
			var tokens = t.Tokenize(text.ToReader()).ToList();

			Assert.That(tokens[1].Fragment, Is.EqualTo(symbol));
			Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Symbol));
		}

		[Test]
		public void Underscore_IsNotSymbol()
		{
			string text = "Hello_World";
			var token = t.Tokenize(text.ToReader()).Single();

			Assert.That(token.Fragment, Is.EqualTo(text));
			Assert.That(token.Type, Is.EqualTo(TokenType.Word));
		}

		[Test]
		public void LeadingWhitespace()
		{
			string text = "  Hello";
			var tokens = t.Tokenize(text.ToReader()).ToList();

			Assert.That(tokens[0].Fragment, Is.EqualTo("  "));
			Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Whitespace));
		}

		[Test]
		public void TrailingWhitespace()
		{
			string text = "Hello  ";
			var tokens = t.Tokenize(text.ToReader()).ToList();

			Assert.That(tokens[1].Fragment, Is.EqualTo("  "));
			Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Whitespace));
		}

		[Test]
		public void SameSymbols_AreGrouped()
		{
			string text = "Hello!!!";
			var tokens = t.Tokenize(text.ToReader()).ToList();

			Assert.That(tokens[1].Fragment, Is.EqualTo("!!!"));
			Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Symbol));
		}

		[Test]
		public void DifferentSymbols_AreNotGrouped()
		{
			string text = "Hello?!?";
			var tokens = t.Tokenize(text.ToReader()).ToList();

			Assert.That(tokens[1].Fragment, Is.EqualTo("?"));
			Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Symbol));

			Assert.That(tokens[2].Fragment, Is.EqualTo("!"));
			Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Symbol));

			Assert.That(tokens[3].Fragment, Is.EqualTo("?"));
			Assert.That(tokens[3].Type, Is.EqualTo(TokenType.Symbol));
		}
	}

	public static class Extensions
	{
		public static StringReader ToReader(this string text)
		{
			return new StringReader(text);
		}
	}
}
