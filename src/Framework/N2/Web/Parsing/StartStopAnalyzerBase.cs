using System.Collections.Generic;
using System.Linq;

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
		protected bool ParseSubComponents { get; set; }

		public string StartFragment { get; set; }
		public string StopFragment { get; set; }
		public TokenType StartType { get; set; }
		public TokenType StopType { get; set; }

		protected virtual bool IsStartToken(IList<Token> tokens, ref int currentIndex)
		{
			Token token = tokens[currentIndex];
			return token.Type == StartType && token.Fragment == StartFragment;
		}

		protected virtual bool IsStopToken(IList<Token> tokens, int startIndex, ref int currentIndex)
		{
			Token token = tokens[currentIndex];
			return token.Type == StopType && token.Fragment == StopFragment;
		}

		protected virtual bool IsValidInnerToken(IList<Token> tokens, int currentIndex)
		{
			Token token = tokens[currentIndex];

			if (!AllowNewLine && token.Type == TokenType.NewLine)
				return false;
			if (!AllowMarkup && (token.Type == TokenType.Element || token.Type == TokenType.EndElement))
				return false;
			return true;
		}

		public override Component GetComponent(Parser parser, IList<Token> tokens, int startIndex)
		{
			if (IsStartToken(tokens, ref startIndex))
				return StartAnalyzing(parser, tokens, startIndex);
			return null;
		}

		private Component StartAnalyzing(Parser parser, IList<Token> tokens, int startIndex)
		{
			for (int i = startIndex + 1; i < tokens.Count; i++)
			{
				if (IsStopToken(tokens, startIndex, ref i))
				{
					if (!AllowEmpty && i == startIndex + 1)
						return null;
					
					var componentTokens = tokens.Skip(startIndex).Take(i - startIndex + 1).ToList();
					var subComponents = GetSubComponents(parser, componentTokens);
					return CreateComponent(componentTokens, subComponents);
				}

				if (!IsValidInnerToken(tokens, i))
					return null;
			}
			return null;
		}

		private Component CreateComponent(IList<Token> componentTokens, IEnumerable<Component> subComponents)
		{
			return new Component
			{
				Command = Name,
				Argument = GetComponentArgument(componentTokens, subComponents),
				Tokens = componentTokens,
				Components = subComponents
			};
		}

		protected virtual string GetComponentArgument(IList<Token> componentTokens, IEnumerable<Component> subComponents)
		{
			return null;
		}

		protected virtual IEnumerable<Component> GetSubComponents(Parser parser, IList<Token> innerTokens)
		{
			if (ParseSubComponents)
				return parser.Parse(innerTokens.Skip(1).Take(innerTokens.Count - 2));
			
			return Component.None;
		}
	}
}
