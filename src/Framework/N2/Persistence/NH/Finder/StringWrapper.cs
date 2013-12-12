using System;
using System.Text;

namespace N2.Persistence.NH.Finder
{
    /// <summary>
    /// Surrounds a string builder with two strings. The prefix is appended 
    /// right away, the suffix is appended upon disposal.
    /// </summary>
    /// <example>
    /// using(new StringWrapper(myStringBuilder, "(", ")"))
    /// {
    ///     myStringBuilder.Append("something that should be between the parenthesis");
    /// }
    /// </example>
    public class StringWrapper : IDisposable
    {
        StringBuilder builder = null;
        string suffix;

        public StringWrapper()
        {
        }

        public StringWrapper(StringBuilder builder, string prefix, string suffix)
        {
            if(!string.IsNullOrEmpty(prefix))
                builder.Append(prefix);
            this.builder = builder;
            this.suffix = suffix;
        }

        public void Dispose()
        {
            if (builder != null && !string.IsNullOrEmpty(suffix))
                builder.Append(suffix);
        }
    }
}
