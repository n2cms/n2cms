using System;
using System.IO;
using System.Web;
using N2.Configuration;
using N2.Engine;

namespace N2.Edit {
    /// <summary>
    /// Class responsible for making sure the content rendered to the client is not containing
    /// any illegal content like javascript or XSS codes.
    /// Securing your ASP.Net application article: http://msdn.microsoft.com/en-us/magazine/hh708755.aspx
    /// XSS cheat sheet: https://www.owasp.org/index.php/XSS_(Cross_Site_Scripting)_Prevention_Cheat_Sheet
    /// XSS examples: https://www.owasp.org/index.php/XSS_Filter_Evasion_Cheat_Sheet
    /// 
    /// </summary>
    [Service(typeof(ISafeContentRenderer))]
    public class SafeContentRenderer : ISafeContentRenderer
    {
        protected bool encodeHtml;

        public SafeContentRenderer(HostSection config)
        {
            encodeHtml = config.HtmlSanitize.EncodeHtml;
        }

        /// <summary>
        /// Html encode the supplied value.
        /// </summary>
        /// <param name="value">String to html encode</param>
        /// <returns>The html encoded value</returns>
        public virtual string HtmlEncode(string value)
        {
            return (encodeHtml ? HttpUtility.HtmlEncode(value) : value);
        }

        public virtual void HtmlEncode(string value, TextWriter writer)
        {
			writer.Write(HtmlEncode(value));
        }

        /// <summary>
        /// Strip unsafe html tags from the supplied html string. This implementation doesn't do any stripping. To secure content override this method in a subclass and perform real stripping.
        /// </summary>
        /// <param name="value">String to strip unsafe html tags from</param>
        /// <returns>A safe html string</returns>
        public virtual string GetSafeHtml(string value)
        {
            return value;
        }

        public virtual void GetSafeHtml(string value, TextWriter writer)
        {
            writer.Write(GetSafeHtml(value));
        }
    }
}
