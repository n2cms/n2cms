using N2.Web.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Wiki.Analyzers
{
    public class NewLineAnalyzer : AnalyzerBase
    {
        public override Component GetComponent(Parser parser, IList<Token> tokens, int index)
        {
            if (index == 0)
                return null;
            if (index == tokens.Count - 1)
                return null;
            var token = tokens[index];
            if (token.Type == TokenType.NewLine)
                return new Component
                {
                    Command = "NewLine",
                    Argument = token.Fragment,
                    Tokens = new List<Token> { token },
                    Components = Component.None
                };
            return null;
        }
    }
}
