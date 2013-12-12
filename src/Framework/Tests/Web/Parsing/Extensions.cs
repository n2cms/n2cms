using System.Collections.Generic;
using System.IO;
using N2.Web.Parsing;

namespace N2.Tests.Web.Parsing
{
    public static class Extensions
    {
        public static StringReader ToReader(this string text)
        {
            return new StringReader(text);
        }

        public static IEnumerable<Token> Tokenize(this string text)
        {
            return new Tokenizer().Tokenize(text.ToReader());
        }
    }
}
