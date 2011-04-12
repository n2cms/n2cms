using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.Parsing;

namespace N2.Web.Wiki.Analyzers
{
	public class OrderedListItemAnalyzer : StartStopAnalyzerBase
	{
		public OrderedListItemAnalyzer()
			: base("#", "\n")
		{
			AllowMarkup = true;
			StopType = TokenType.NewLine;
		}

		protected override bool IsStartToken(IList<Token> tokens, int index)
		{
			return base.IsStartToken(tokens, index)
				&& (index == 0 || tokens[index - 1].Type == TokenType.NewLine)
				&& (index < tokens.Count - 1 && tokens[index + 1].Type == TokenType.Whitespace);
		}

		protected override bool IsStopToken(IList<Token> tokens, int index)
		{
			return tokens[index].Type == StopType || index == tokens.Count - 1;
		}

		protected override string ExtractData(IList<Token> blockTokens)
		{
			return blockTokens.Skip(1).Select(t => t.Fragment).StringJoin().Trim(' ', '\t', '\r', '\n');
		}
	}
}
