using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using System.IO;

namespace N2.Web.Parsing
{
	public enum TokenType
	{
		Whitespace,
		Word,
		Element,
		EndElement,
		Symbol
	}

	public class Token
	{
		public int Index { get; set; }
		public TokenType Type { get; set; }
		public string Fragment { get; set; }
	}

	[Service(typeof(Tokenizer))]
	public class Tokenizer
	{
		public virtual IEnumerable<Token> Tokenize(TextReader reader)
		{
			int index = 0;

			char c;
			if (WasEof(reader, out c))
				yield break;

			while(c != '\0')
			{
				if(char.IsWhiteSpace(c))
					yield return FixIndex(ref index, ReadWhitespace(reader, ref c));
				if (IsSymbol(c))
					yield return FixIndex(ref index, ReadSymbol(reader, ref c));
				else
					yield return FixIndex(ref index, ReadText(reader, ref c));
			}
		}

		private Token ReadSymbol(TextReader reader, ref char c)
		{
			if(c == '<')
				return ReadElement(reader, ref c);

			//using (var sw = new StringWriter())
			//{
			//    while (c != 0)
			//    {
			//        sw.Write(c);

			//        if (WasEof(reader, out c))
			//            break;
			//        if(c == '<' || !IsSymbol(c))
			//            break;
			//    }

			//    return new Token { Type = TokenType.Symbol, Fragment = sw.ToString() };
			//}

			char original = c;
			using (var sw = new StringWriter())
			{
				while (c != 0)
				{
					sw.Write(c);

					if (WasEof(reader, out c))
						break;
					if (c != original)
						break;
				}

				return new Token { Type = TokenType.Symbol, Fragment = sw.ToString() };
			}

			//var t = new Token { Type = TokenType.Symbol, Fragment = c.ToString() };
			//WasEof(reader, out c);
			//return t;
		}

		private Token ReadText(TextReader reader, ref char c)
		{
			using (var sw = new StringWriter())
			{
				while (c != 0)
				{
					if(char.IsWhiteSpace(c))
						break;
					else if(c == '<')
						break;
					else if(IsSymbol(c))
						break;

					sw.Write(c);

					if (WasEof(reader, out c))
						break;
				}

				return new Token { Type = TokenType.Word, Fragment = sw.ToString() };
			}
		}

		private bool IsSymbol(char c)
		{
			switch (c)
			{
				case '§':
				case '!':
				case '"':
				case '#':
				case '¤':
				case '%':
				case '&':
				case '/':
				case '(':
				case ')':
				case '=':
				case '+':
				case '?':
				case '´':
				case '`':
				case '@':
				case '£':
				case '$':
				case '€':
				case '{':
				case '[':
				case ']':
				case '}':
				case '\\':
				case '|':
				case '¨':
				case '^':
				case '~':
				case '\'':
				case '*':
				case ',':
				case ';':
				case '.':
				case ':':
				case '-':
					return true;
				default:
					return char.IsSymbol(c);
			}
		}

		private Token ReadElement(TextReader reader, ref char c)
		{
			using (var sw = new StringWriter())
			{
				sw.Write(c);
				c = (char)reader.Read();
				TokenType tt = (c != '/') ? TokenType.Element : TokenType.EndElement;

				while (c != 0)
				{
					sw.Write(c);

					if (c == '>')
					{
						WasEof(reader, out c);
						break;
					}

					if (WasEof(reader, out c))
						break;
				}
				return new Token { Type = tt, Fragment = sw.ToString() };
			}
		}

		private Token ReadWhitespace(TextReader reader, ref char c)
		{
			using (var sw = new StringWriter())
			{
				while (c != 0)
				{
					if(char.IsWhiteSpace(c))
						sw.Write(c);
					else
						break;

					if (WasEof(reader, out c))
						break;
				}
				return new Token { Type = TokenType.Whitespace, Fragment = sw.ToString() };
			}
		}

		private bool WasEof(TextReader reader, out char c)
		{
			int val = reader.Read();

			if (val == -1)
			{
				c = '\0';
				return true;
			}
				
			c = (char)val;
			return false;
		}

		private Token FixIndex(ref int index, Token token)
		{
			token.Index = index;
			index += token.Fragment.Length;
			return token;
		}
	}
}
