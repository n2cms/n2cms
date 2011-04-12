using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Parsing.Wiki
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
