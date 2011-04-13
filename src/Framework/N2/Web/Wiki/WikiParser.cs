using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using N2.Web.Parsing;
using N2.Web.Wiki.Analyzers;

namespace N2.Web.Wiki
{
	public class WikiParser : Parser
	{
		public WikiParser()
			: base(new CommentAnalyzer(), new InternalLinkAnalyzer(), new ExternalLinkAnalyzer(), new BoldItalicsAnalyzer(), new BoldAnalyzer(), new ItalicsAnalyzer(), new H4Analyzer(), new H1Analyzer(), new H2Analyzer(), new H3Analyzer(), new UnorderedListItemAnalyzer(), new OrderedListItemAnalyzer(), new UserInfoAnalyzer(), new TemplateAnalyzer())
		{
		}
	}
}
