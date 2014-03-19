using N2.Web.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Wiki.Analyzers
{
    public class HtmlElementAnalyzer : AnalyzerBase
    {
        public HtmlElementAnalyzer()
        {
        }

        public override Component GetComponent(Parser parser, IList<Token> tokens, int index)
        {
            var token = tokens[index];
            if (token.Type == TokenType.Element || token.Type == TokenType.EndElement)
                return new Component
                {
                    Argument = token.Fragment,
                    Command = "HtmlElement",
                    Tokens = new List<Token> { token },
                    Components = Component.None
                };
            return null;
        }
    }
}
