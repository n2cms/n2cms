using N2.Web.Parsing;

namespace N2.Web.Wiki.Analyzers
{
    public class BoldItalicsAnalyzer : StartStopAnalyzerBase
    {
        public BoldItalicsAnalyzer()
            : base("'''''")
        {
            AllowMarkup = true;
            ParseSubComponents = true;
        }
    }
}
