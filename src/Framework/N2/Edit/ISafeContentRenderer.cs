using System;
using System.IO;

namespace N2.Edit
{
    public interface ISafeContentRenderer
    {
        /// <summary>
        /// Html encode the supplied value.
        /// </summary>
        /// <param name="value">String to html encode</param>
        /// <returns>The html encoded value</returns>
        string HtmlEncode(string value);

        void HtmlEncode(string value, TextWriter writer);

        /// <summary>
        /// Strip unsafe html tags from the supplied html string.
        /// </summary>
        /// <param name="value">String to strip unsafe html tags from</param>
        /// <returns>A safe html string</returns>
        string GetSafeHtml(string value);

        void GetSafeHtml(string value, TextWriter writer);
    }
}
