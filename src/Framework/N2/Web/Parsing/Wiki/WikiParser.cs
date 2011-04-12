using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace N2.Web.Parsing.Wiki
{
	public class WikiParser : Parser
	{
		Tokenizer tokenizer;

		public WikiParser()
			: base(new CommentAnalyzer(), new InternalLinkAnalyzer(), new ExternalLinkAnalyzer(), new BoldItalicsAnalyzer(), new BoldAnalyzer(), new ItalicsAnalyzer(), new H4Analyzer(), new H1Analyzer(), new H2Analyzer(), new H3Analyzer(), new UnorderedListItemAnalyzer(), new OrderedListItemAnalyzer(), new UserInfoAnalyzer(), new TemplateAnalyzer())
		{
			tokenizer = new Tokenizer();
		}

		public IEnumerable<Component> Parse(string text)
		{
			return Parse(tokenizer.Tokenize(new StringReader(text)));
		}
	}
}
