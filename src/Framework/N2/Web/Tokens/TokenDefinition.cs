using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Tokens
{
    /// <summary>
    /// Describes a token and it's available options.
    /// </summary>
    public class TokenDefinition
    {
        public string Name { get; set; }
        public IEnumerable<TokenOption> Options { get; set; }
    }
}
