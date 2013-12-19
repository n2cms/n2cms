using System.Collections.Generic;
using N2.Web.Parsing;

namespace N2.Web.Wiki.Analyzers
{
    public class ExternalLinkAnalyzer : StartStopAnalyzerBase
    {
        public ExternalLinkAnalyzer()
            : base("[", "]")
        {
        }

        protected override bool IsStartToken(IList<Token> tokens, ref int currentIndex)
        {
            return base.IsStartToken(tokens, ref currentIndex) 
                || IsHttp(tokens, currentIndex);
        }

        protected override bool IsStopToken(IList<Token> tokens, int startIndex, ref int currentIndex)
        {
            if(base.IsStopToken(tokens, startIndex, ref currentIndex))
                return true;
            if(!IsHttp(tokens, startIndex))
                return false;

            if(!tokens.IsWithinBounds(currentIndex + 1))
                return true;
            if (tokens[currentIndex + 1].Type == TokenType.Whitespace)
                return true;
            if (tokens[currentIndex + 1].Type == TokenType.NewLine)
                return true;
            if (IsStopSymbol(tokens[currentIndex + 1]))
                return true;

            return false;
        }

        private bool IsStopSymbol(Token token)
        {
            if (token.Type != TokenType.Symbol)
                return false;

            switch (token.Fragment[0])
            {
                case ',':
                case '!':
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsHttp(IList<Token> tokens, int index)
        {
            return tokens[index].Type == TokenType.Text
                && tokens[index].Fragment == "http"
                && tokens.IsWithinBounds(index + 1)
                && tokens[index + 1].Type == TokenType.Symbol
                && tokens[index + 1].Fragment == ":"
                && tokens.IsWithinBounds(index + 2)
                && tokens[index + 2].Type == TokenType.Symbol
                && tokens[index + 2].Fragment == "//";
        }
    }
}
