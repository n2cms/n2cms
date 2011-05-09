using System.Collections.Generic;
using System.IO;

namespace N2.Web.Parsing
{
	//[Service(typeof(Tokenizer))]
	public class Tokenizer
	{
		public virtual IEnumerable<Token> Tokenize(TextReader reader)
		{
			int index = 0;

			char c;
			if (WasEof(reader, out c))
				yield break;

			while (c != '\0')
			{
				if(char.IsWhiteSpace(c))
					yield return FixIndex(ref index, ReadWhitespace(reader, ref c));
				else if (IsSymbol(c))
					yield return FixIndex(ref index, ReadSymbol(reader, ref c));
				else
					yield return FixIndex(ref index, ReadText(reader, ref c));
			}
		}

		private Token ReadSymbol(TextReader reader, ref char c)
		{
			if(c == '<')
				return ReadElement(reader, ref c);

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

				return new Token { Type = TokenType.Text, Fragment = sw.ToString() };
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
			if (IsNewline(c))
				return ReadNewline(reader, ref c);

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

		private Token ReadNewline(TextReader reader, ref char c)
		{
			using (var sw = new StringWriter())
			{
				while (c != 0)
				{
					if (IsNewline(c))
						sw.Write(c);
					else
						break;

					if (WasEof(reader, out c))
						break;
				}
				return new Token { Type = TokenType.NewLine, Fragment = sw.ToString() };
			}
		}

		private static bool IsNewline(char c)
		{
			return c == '\n' || c == '\r';
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
