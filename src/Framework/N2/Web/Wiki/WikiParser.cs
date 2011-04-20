using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using N2.Web.Parsing;
using N2.Web.Wiki.Analyzers;
using N2.Engine;

namespace N2.Web.Wiki
{
	[Service]
	public class WikiParser : Parser
	{
		public WikiParser()
			: base(new CommentAnalyzer(), new InternalLinkAnalyzer(), new ExternalLinkAnalyzer(), new BoldItalicsAnalyzer(), new BoldAnalyzer(), new ItalicsAnalyzer(), new HeadingAnalyzer(), new UnorderedListItemAnalyzer(), new OrderedListItemAnalyzer(), new UserInfoAnalyzer(), new TemplateAnalyzer())
		{
		}
	}
}
