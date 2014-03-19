using N2.Configuration;
using N2.Engine;
using N2.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace N2.Web
{
    /// <summary>
    /// Class responsible for making sure the content rendered to the client is not containing
    /// any illegal content like javascript or XSS codes.
    /// Securing your ASP.Net application article: http://msdn.microsoft.com/en-us/magazine/hh708755.aspx
    /// XSS cheat sheet: https://www.owasp.org/index.php/XSS_(Cross_Site_Scripting)_Prevention_Cheat_Sheet
    /// XSS examples: https://www.owasp.org/index.php/XSS_Filter_Evasion_Cheat_Sheet
    /// </summary>
    [Service]
    public class HtmlSanitizer : IAutoStart
    {
        private static HtmlSanitizer current;
        public static HtmlSanitizer Current
        {
            get { return current ?? (current = new HtmlSanitizer(new ConfigurationManagerWrapper().Sections.Web)); }
            set { current = value; }
        }

        private bool encodeHtml;

        public HtmlSanitizer(HostSection config)
        {
            encodeHtml = config.HtmlSanitize.EncodeHtml;
        }

        public void Start()
        {
            Current = this;
        }
        public void Stop()
        {
        }

        /// <summary>Html encode the supplied value according to the configured settings.</summary>
        /// <param name="value">String to html encode</param>
        /// <returns>The html encoded value</returns>
        public virtual string Encode(string value)
        {
            return (encodeHtml ? HttpUtility.HtmlEncode(value) : value);
        }

        /// <summary>
        /// Strip unsafe html tags from the supplied html string. This implementation doesn't do any stripping. To secure content override this method in a subclass and perform real stripping.
        /// </summary>
        /// <param name="value">String to strip unsafe html tags from</param>
        /// <returns>A safe html string</returns>
        /// <example>
        /// [Service(typeof(HtmlSanitizer), Replaces = typeof(HtmlSanitizer))]
        /// public class RealHtmlSanitizer : HtmlSanitizer
        /// {
        ///     public override string Clean(string value)
        ///     {
        ///         throw new NotImplementedException();
        ///     }
        /// }
        /// </example>
        public virtual string Clean(string value)
        {
            return value;
        }
    }
}
