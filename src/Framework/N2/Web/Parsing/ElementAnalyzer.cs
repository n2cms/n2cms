using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Parsing
{
	public class ElementAnalyzer : AnalyzerBase
	{
		public ElementAnalyzer(string element, bool parseContents = false, bool xmlContents = true)
		{
			ParseContents = parseContents;
			XmlContents = xmlContents;
			Element = element;
			StartFragment = "<" + element;
			StopFragment = "</" + element + ">";
		}

		public string Element { get; set; }

		public override string Name
		{
			get { return Element; }
		}

		public string StartFragment { get; set; }

		public string StopFragment { get; set; }

		public bool ParseContents { get; set; }

		public override Component GetComponent(Parser parser, IList<Token> tokens, int index)
		{
			var token = tokens[index];
			if (IsStartToken(token))
			{
				if (!ParseContents || token.Fragment.EndsWith("/>"))
					return new Component { Command = Name, Tokens = new List<Token> { token } };

				int depth = 1;
				for (int i = index + 1; i < tokens.Count; i++)
				{
					var innerToken = tokens[i];
					if (XmlContents)
					{
						if (innerToken.Type == TokenType.Element && !innerToken.Fragment.EndsWith("/>"))
						{
							depth++;
						}
						else if (innerToken.Type == TokenType.EndElement)
						{
							depth--;
						}
					}
					else if (IsStopToken(innerToken))
					{
						depth--;
					}
					if (depth == 0)
					{
						return new Component { Command = Name, Tokens = tokens.Skip(index).Take(i - index + 1).ToList() };
					}
				}
			}
			else if (!ParseContents && IsStopToken(token))
			{
				return new Component { Command = Name + "-end", Tokens = new List<Token> { token } };
			}
			return null;
		}

		protected virtual bool IsStopToken(Token token)
		{
			return token.Type == TokenType.EndElement && token.Fragment.StartsWith(StopFragment, StringComparison.OrdinalIgnoreCase);
		}

		protected virtual bool IsStartToken(Token token)
		{
			return token.Type == TokenType.Element && token.Fragment.StartsWith(StartFragment, StringComparison.OrdinalIgnoreCase);
		}

		public bool XmlContents { get; set; }
	}
}
