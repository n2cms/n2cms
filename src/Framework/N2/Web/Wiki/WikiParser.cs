using N2.Engine;
using N2.Web.Parsing;
using N2.Web.Wiki.Analyzers;

namespace N2.Web.Wiki
{
    [Service]
    public class WikiParser : Parser
    {
        public WikiParser()
            : base(new HtmlElementAnalyzer(), new CommentAnalyzer(), new InternalLinkAnalyzer(), new ExternalLinkAnalyzer(), new BoldItalicsAnalyzer(), new BoldAnalyzer(), new ItalicsAnalyzer(), new HeadingAnalyzer(), new UnorderedListItemAnalyzer(), new OrderedListItemAnalyzer(), new UserInfoAnalyzer(), new TemplateAnalyzer(), new NewLineAnalyzer())
        {
        }
    }
}
