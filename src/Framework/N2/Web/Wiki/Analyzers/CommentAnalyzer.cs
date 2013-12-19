using N2.Web.Parsing;

namespace N2.Web.Wiki.Analyzers
{
    public class CommentAnalyzer : StartStopAnalyzerBase
    {
        public CommentAnalyzer()
            : base("(", ")")
        {
            AllowMarkup = true;
            AllowNewLine = true;
        }
    }
}
