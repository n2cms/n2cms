namespace N2.Web
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using Configuration;
    using Engine;

    /// <summary>
    /// Slug (web publishing) : a user- and SEO-friendly short text used in a URL to identify and describe a resource
    /// </summary>
    [Service]
    public class Slug
    {
        /// <summary>
        /// Patterns for replacing single characters
        /// </summary>
        private readonly PatternValueCollection replacementPatterns = new PatternValueCollection();

        /// <summary>
        /// Character that will replace whitespaces
        /// </summary>
        private readonly char whitespaceReplacement = '-';

        /// <summary>
        /// The to lower.
        /// </summary>
        private readonly bool toLower = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Slug"/> class.
        /// </summary>
        /// <param name="configuration">
        /// Configuration from web.config
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If configuration is null
        /// </exception>
        public Slug(ConfigurationManagerWrapper configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            NameEditorElement nameEditorElement = configuration.Sections.Management.NameEditor;

            // start with empty collection
            this.replacementPatterns.Clear();

            // first add any replacements from custom configuration, which will be applied first
            foreach (PatternValueElement element in nameEditorElement.Replacements.AllElements)
            {
                this.replacementPatterns.Add(element);
            }

            // then, add system replacements inserted from constructor
            foreach (PatternValueElement element in new PatternValueCollection())
            {
                this.replacementPatterns.Add(element);
            }

            this.whitespaceReplacement = nameEditorElement.WhitespaceReplacement;
            this.toLower = nameEditorElement.ToLower;
        }

        /// <summary>
        /// Creates slug from <see cref="text"/>
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// Slug created from <see cref="text"/>
        /// </returns>
        public virtual string Create(string text)
        {
            return Create(text, this.whitespaceReplacement, this.replacementPatterns, this.toLower);
        }

        /// <summary>
        /// Verifies if slug is properly formatted
        /// Implementation is combination of .Net Uri.IsWellFormedUriString used for
        /// checking of encoding, and recommendations from RFC 3986 [http://tools.ietf.org/html/rfc3986]
        /// The generic syntax uses the slash ("/"), question mark ("?"), and
        /// number sign ("#") characters to delimit components
        /// </summary>
        /// <param name="slug">Slug to be tested</param>
        /// <returns>True if slug is valid, false otherwise</returns>
        public virtual bool IsValid(string slug)
        {
            if (String.IsNullOrWhiteSpace(slug))
            {
                return false;
            }
            else
            {
                return slug.IndexOfAny(new[] { '/', '?', '#', '@', ':', '&', '+', '\'', '*', '%', '\\' }) == -1
                    && !slug.Contains("%20"); // space encoded in name is confusing N2 url parser
            }
        }

        /// <summary>
        /// Creates slug from text
        /// </summary>
        /// <param name="text">
        /// Text to be converted to slug
        /// </param>
        /// <param name="whitespaceReplacement">
        /// Whitespace replacement character
        /// </param>
        /// <param name="replacementPatterns">
        /// Set of custom patterns for replacing characters
        /// </param>
        /// <param name="toLower">
        /// True if produced slug should be lowercase
        /// </param>
        /// <returns>
        /// Slug generated from <see cref="text"/>.
        /// </returns>
        private static string Create(
            string text,
            char whitespaceReplacement,
            PatternValueCollection replacementPatterns,
            bool toLower)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            foreach (PatternValueElement pattern in replacementPatterns)
            {
                text = Regex.Replace(text, pattern.Pattern, pattern.Value);
            }

            string normalised = text.Normalize(NormalizationForm.FormKD);

            const int Maxlen = 80;
            int len = normalised.Length;
            bool prevDash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = normalised[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    if (prevDash)
                    {
                        sb.Append('-');
                        prevDash = false;
                    }

                    sb.Append(c);
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    if (prevDash)
                    {
                        sb.Append('-');
                        prevDash = false;
                    }

                    // tricky way to convert to lowercase
                    if (toLower)
                    {
                        sb.Append((char)(c | 32));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' || c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevDash && sb.Length > 0)
                    {
                        prevDash = true;
                    }
                }
                else
                {
                    string swap = ConvertEdgeCases(c, toLower);

                    if (swap != null)
                    {
                        if (prevDash)
                        {
                            sb.Append('-');
                            prevDash = false;
                        }

                        sb.Append(swap);
                    }
                }

                if (sb.Length == Maxlen)
                {
                    break;
                }
            }

            string slug = sb.ToString();

            if (whitespaceReplacement != '-')
            {
                return slug.Replace('-', whitespaceReplacement);
            }
            else
            {
                return slug;
            }
        }

        /// <summary>
        /// Helper method for converting special cases not covered by Normalize method
        /// </summary>
        /// <param name="c">
        /// Character to be converted
        /// </param>
        /// <param name="toLower">
        /// Should character be converted to lowercase
        /// </param>
        /// <returns>
        /// Converted value
        /// </returns>
        private static string ConvertEdgeCases(char c, bool toLower)
        {
            string swap = null;
            switch (c)
            {
                case 'ı':
                    swap = "i";
                    break;
                case 'ł':
                    swap = "l";
                    break;
                case 'Ł':
                    swap = toLower ? "l" : "L";
                    break;
                case 'đ':
                    swap = "d";
                    break;
                case 'Đ':
                    swap = toLower ? "d" : "D";
                    break;
                case 'ß':
                    swap = "ss";
                    break;
                case 'ø':
                    swap = "o";
                    break;
                case 'Ø':
                    swap = toLower ? "o" : "O";
                    break;
                case 'Þ':
                    swap = "th";
                    break;
            }

            return swap;
        }
    }
}
