using System.Collections.Generic;
using System.Linq;
using N2.Web.Parsing;

namespace N2.Web.Wiki.Analyzers
{
    public class ListItemAnalyzerBase : StartStopAnalyzerBase
    {
        public ListItemAnalyzerBase(string startFragment)
            : base(startFragment, "\n")
        {
            AllowMarkup = true;
            StopType = TokenType.NewLine;
            ParseSubComponents = true;
        }

        protected override bool IsStartToken(IList<Token> tokens, ref int currentIndex)
        {
            return tokens[currentIndex].Type == StartType && tokens[currentIndex].Fragment.StartsWith(StartFragment)
                && (currentIndex == 0 || tokens[currentIndex - 1].Type == TokenType.NewLine)
                && (currentIndex < tokens.Count - 1 && tokens[currentIndex + 1].Type == TokenType.Whitespace);
        }

        protected override bool IsStopToken(IList<Token> tokens, int startIndex, ref int currentIndex)
        {
            if (tokens[currentIndex].Type == StopType)
                return true;

            if (currentIndex < tokens.Count - 1)
                return false;

            ++currentIndex;
            return true;
        }

        protected override IEnumerable<Component> GetSubComponents(Parser parser, IList<Token> innerTokens)
        {
            return parser.Parse(innerTokens.Skip(2).Where(c => c.Type != TokenType.NewLine));
        }

        protected override string GetComponentArgument(IList<Token> componentTokens, IEnumerable<Component> subComponents)
        {
            return componentTokens[0].Fragment;
        }
    }
}
