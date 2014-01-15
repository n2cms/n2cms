using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Parsing
{
	public class HtmlCommentAnalyzer : AnalyzerBase
	{
		public override Component GetComponent(Parser parser, System.Collections.Generic.IList<Token> tokens, int index)
		{
			if (tokens[index].Type == TokenType.Comment)
				return new Component { Command = Name, Tokens = new List<Token> { tokens[index] } };

			if (tokens[index].Fragment.StartsWith("<!--") && tokens[index].Fragment.EndsWith("-->"))
				return new Component { Command = Name, Tokens = new List<Token> { tokens[index] } };

			return null;
		}
	}
}
