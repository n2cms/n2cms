using N2.Web.Parsing;

namespace N2.Web.Wiki.Analyzers
{
    public class InternalLinkAnalyzer : StartStopAnalyzerBase
    {
        public InternalLinkAnalyzer()
            : base("[[", "]]")
        {
        }
    }
}
