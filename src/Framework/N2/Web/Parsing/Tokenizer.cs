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
				if (c == '!')
				{
					sw.Write(c);
					return ReadComment(reader, sw, ref c);
				}

				TokenType tt = (c != '/') ? TokenType.Element : TokenType.EndElement;
                bool isQuoting = false;
                bool isEscaping = false;
                char quoteSymbol = '\0';

				while (c != 0)
				{
					sw.Write(c);

                    if (isQuoting)
                    {
                        if (isEscaping)
                        {
                            isEscaping = false; 
                            WasEof(reader, out c);
                            continue;
                        }
                        if (c == '\\')
                        {
                            isEscaping = true;
                            WasEof(reader, out c);
                            continue;
                        }

                        if (c == quoteSymbol)
                        {
                            isQuoting = false;
                        }

                        WasEof(reader, out c);
                        continue;
                    }
                    if(c == '\'' || c == '"')
                    {
                        isQuoting = true;
                        quoteSymbol = c;
                        WasEof(reader, out c);
                        continue;
                    }
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

		private Token ReadComment(TextReader reader, StringWriter sw, ref char c)
		{
			if (WasEof(reader, out c))
				return new Token { Type = TokenType.Text, Fragment = sw.ToString() };
			sw.Write(c);

			if (c == '[')
			{
				if (!ReadExactly("CDATA[", reader, sw, ref c))
					return new Token { Type = TokenType.Text, Fragment = sw.ToString() };

				if (!ReadUntil("]]>", reader, sw, ref c))
					return new Token { Type = TokenType.Text, Fragment = sw.ToString() };

				return new Token { Type = TokenType.CData, Fragment = sw.ToString() };
			}

			if (c == 'D')
			{
				if (!ReadExactly("OCTYPE", reader, sw, ref c))
					return new Token { Type = TokenType.Text, Fragment = sw.ToString() };

				ReadUntil(">", reader, sw, ref c);
				return new Token { Type = TokenType.Comment, Fragment = sw.ToString() };
			}

			if (c != '-')
			{
				return new Token { Type = TokenType.Text, Fragment = sw.ToString() };	
			}

			if (WasEof(reader, out c))
				return new Token { Type = TokenType.Text, Fragment = sw.ToString() };
			sw.Write(c);
			if (c != '-')
				return new Token { Type = TokenType.Text, Fragment = sw.ToString() };

			ReadUntil("-->", reader, sw, ref c);
			return new Token { Type = TokenType.Comment, Fragment = sw.ToString() };
		}

		private bool ReadUntil(string ending, TextReader reader, StringWriter sw, ref char c)
		{
			if (WasEof(reader, out c))
				return false;
			
			int i = 0;
			var buffer = new char[ending.Length];
			while (true)
			{
				if (i >= ending.Length)
					return true;

				sw.Write(c);

				if (ending[i] == c)
				{
					buffer[i] = c;
					i++;
				}
				else if (i > 0 && buffer[i - 1] == c)
				{
					buffer[i] = c;

					if (!ending.StartsWith(new string(buffer, 1, i)))
						i = 0;
					else
						for (int j = 1; j < i; j++)
							buffer[j - 1] = buffer[j];
				}
				else
					i = 0;

				if (WasEof(reader, out c))
					return i == ending.Length;
			}
		}

		private bool ReadExactly(string expected, TextReader reader, StringWriter sw, ref char c)
		{
			if (WasEof(reader, out c))
				return false;

			int i = 0;
			while (true)
			{
				sw.Write(c);

				if (i > expected.Length - 1)
					return true;

				if (expected[i] != c)
					return false;
				
				i++;
				if (WasEof(reader, out c))
					return i == expected.Length;
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
