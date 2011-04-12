using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.Parsing;

namespace N2.Web.Wiki.Analyzers
{
	public class UserInfoAnalyzer : AnalyzerBase
	{
		public override Component GetComponent(IList<Token> tokens, int index)
		{
			var token = tokens[index];
			if (token.Type == TokenType.Symbol && token.Fragment.StartsWith("~~~"))
				return new Component { Command = "UserInfo", Data = token.Fragment, Tokens = new List<Token> { token } };
			return null;
		}
	}
}
