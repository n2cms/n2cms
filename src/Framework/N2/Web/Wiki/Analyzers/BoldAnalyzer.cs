using N2.Web.Parsing;

namespace N2.Web.Wiki.Analyzers
{
    public class BoldAnalyzer : StartStopAnalyzerBase
    {
        public BoldAnalyzer()
            : base("'''")
        {
            ParseSubComponents = true;
        }
    }
}
