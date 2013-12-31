using System.Collections.Generic;
using N2.Web.Parsing;

namespace N2.Web.Wiki.Analyzers
{
    public class UserInfoAnalyzer : AnalyzerBase
    {
        public override Component GetComponent(Parser parser, IList<Token> tokens, int index)
        {
            var token = tokens[index];
            if (token.Type == TokenType.Symbol && token.Fragment.StartsWith("~~~"))
                return new Component 
                { 
                    Command = "UserInfo",
                    Argument = token.Fragment,
                    Tokens = new List<Token> { token },
                    Components = Component.None
                };
            return null;
        }
    }
}
