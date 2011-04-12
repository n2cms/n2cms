using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Parsing
{
	public abstract class StartStopAnalyzerBase : AnalyzerBase
	{
		public StartStopAnalyzerBase()
		{
		}

		public StartStopAnalyzerBase(string fragment)
			: this(fragment, fragment)
		{
		}

		public StartStopAnalyzerBase(string startFragment, string stopFragment)
		{
			StartFragment = startFragment;
			StopFragment = stopFragment;
			StartType = TokenType.Symbol;
			StopType = TokenType.Symbol;
		}

		protected bool AllowEmpty { get; set; }
		protected bool AllowNewLine { get; set; }
		protected bool AllowMarkup { get; set; }

		public string StartFragment { get; set; }
		public string StopFragment { get; set; }
		public TokenType StartType { get; set; }
		public TokenType StopType { get; set; }

		protected virtual bool IsStartToken(IList<Token> tokens, int index)
		{
			Token token = tokens[index];
			return token.Type == StartType && token.Fragment == StartFragment;
		}

		protected virtual bool IsStopToken(IList<Token> tokens, int index)
		{
			Token token = tokens[index];
			return token.Type == StopType && token.Fragment == StopFragment;
		}

		protected virtual bool IsValidInnerToken(IList<Token> tokens, int index)
		{
			Token token = tokens[index];

			if (!AllowNewLine && token.Type == TokenType.NewLine)
				return false;
			if (!AllowMarkup && (token.Type == TokenType.Element || token.Type == TokenType.EndElement))
				return false;
			return true;
		}

		protected virtual string ExtractData(IList<Token> blockTokens)
		{
			return blockTokens.Skip(1).Take(blockTokens.Count - 2).Select(t => t.Fragment).StringJoin();
		}

		public override Component GetComponent(IList<Token> tokens, int index)
		{
			if (IsStartToken(tokens, index))
				return StartAnalyzing(tokens, index);
			return null;
		}

		private Component StartAnalyzing(IList<Token> tokens, int index)
		{
			for (int i = index + 1; i < tokens.Count; i++)
			{
				if (IsStopToken(tokens, i))
				{
					if (!AllowEmpty && i == index + 1)
						return null;

					var blockTokens = tokens.Skip(index).Take(i - index + 1).ToList();
					return new Component
					{
						Command = Name,
						Tokens = blockTokens,
						Data = ExtractData(blockTokens)
					};
				}

				if (!IsValidInnerToken(tokens, i))
					return null;
			}
			return null;
		}
	}
}
