using System.Collections.Generic;
using N2.Web.Parsing;

namespace N2.Web.Wiki.Analyzers
{
    public class HeadingAnalyzer : StartStopAnalyzerBase
    {
        public HeadingAnalyzer()
            : base("=")
        {
            AllowMarkup = true;
            ParseSubComponents = true;
        }

        protected override bool IsStartToken(IList<Token> tokens, ref int currentIndex)
        {
            Token token = tokens[currentIndex];
            return token.Type == StartType && token.Fragment.StartsWith(StartFragment);
        }

        protected override bool IsStopToken(IList<Token> tokens, int startIndex, ref int currentIndex)
        {
            Token token = tokens[currentIndex];
            return token.Type == StopType && token.Fragment == tokens[startIndex].Fragment;
        }

        protected override string GetComponentArgument(IList<Token> componentTokens, IEnumerable<Component> subComponents)
        {
            return componentTokens[0].Fragment;
        }
    }
}
