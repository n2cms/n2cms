using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.Parsing;
using N2.Web.Wiki.Analyzers;

namespace N2.Web.Tokens
{
    public class TokenParser : Parser
    {
        public TokenParser()
            : base(new TemplateAnalyzer())
        {
        }
    }
}
